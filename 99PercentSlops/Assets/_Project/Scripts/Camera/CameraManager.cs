using GlitchWorker.Player;
using GlitchWorker.Systems;
using UnityEngine;

namespace GlitchWorker.Camera
{
    public enum CameraViewMode
    {
        FirstPerson = 0,
        ThirdPerson = 1,
        Cinematic = 2
    }

    /// <summary>
    /// Runtime camera controller for prototype validation:
    /// - smoothed look
    /// - 1P/3P transition
    /// - cinematic override by trigger zone
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private CameraSettings _settings;

        [Header("Look")]
        [SerializeField] private float _sensitivityX = 2f;
        [SerializeField] private float _sensitivityY = 2f;
        [SerializeField] private bool _invertY;
        [SerializeField] private float _maxLookAngle = 80f;
        [SerializeField, Min(0f)] private float _rotationSmoothTime = 0.06f;

        [Header("First Person")]
        [SerializeField] private Vector3 _firstPersonLocalOffset = new Vector3(0f, 0.8f, 0f);

        [Header("Third Person")]
        [SerializeField, Min(0.1f)] private float _thirdPersonDistance = 3.2f;
        [SerializeField, Min(0f)] private float _thirdPersonHeightOffset = 1.6f;
        [SerializeField, Min(0f)] private float _thirdPersonCollisionRadius = 0.2f;
        [SerializeField] private LayerMask _cameraCollisionMask = ~0;

        [Header("Mode Transition")]
        [SerializeField, Min(0.01f)] private float _modeTransitionTime = 0.22f;
        [SerializeField] private CameraViewMode _startupMode = CameraViewMode.FirstPerson;

        private float _targetYaw;
        private float _targetPitch;
        private float _smoothedYaw;
        private float _smoothedPitch;
        private float _yawVelocity;
        private float _pitchVelocity;
        private float _thirdPersonWeight;
        private float _thirdPersonWeightVelocity;

        private CameraViewMode _activeMode;
        private CameraViewMode _modeBeforeCinematic;
        private Transform _cinematicTransform;
        private CinematicCameraZone _activeZone;

        public Transform ActiveCameraTransform => _cameraTransform != null
            ? _cameraTransform
            : UnityEngine.Camera.main?.transform;

        public CameraViewMode ActiveMode => _activeMode;
        public CameraSettings Settings => _settings;

        private float SensitivityX => _settings != null ? _settings.SensitivityX : _sensitivityX;
        private float SensitivityY => _settings != null ? _settings.SensitivityY : _sensitivityY;
        private bool InvertY => _settings != null ? _settings.InvertY : _invertY;
        private float MaxLookAngle => _settings != null ? _settings.MaxLookAngle : _maxLookAngle;
        private float RotationSmoothTime => _settings != null ? _settings.RotationSmoothTime : _rotationSmoothTime;
        private Vector3 FirstPersonLocalOffset => _settings != null ? _settings.FirstPersonLocalOffset : _firstPersonLocalOffset;
        private float ThirdPersonDistance => _settings != null ? _settings.ThirdPersonDistance : _thirdPersonDistance;
        private float ThirdPersonHeightOffset => _settings != null ? _settings.ThirdPersonHeightOffset : _thirdPersonHeightOffset;
        private float ThirdPersonCollisionRadius => _settings != null ? _settings.ThirdPersonCollisionRadius : _thirdPersonCollisionRadius;
        private LayerMask CameraCollisionMask => _settings != null ? _settings.CameraCollisionMask : _cameraCollisionMask;
        private float ModeTransitionTime => _settings != null ? _settings.ModeTransitionTime : _modeTransitionTime;
        private CameraViewMode StartupMode => _settings != null ? _settings.StartupMode : _startupMode;

        public Vector3 Forward
        {
            get
            {
                if (ActiveCameraTransform == null) return Vector3.forward;
                Vector3 forward = ActiveCameraTransform.forward;
                forward.y = 0f;
                return forward.sqrMagnitude > 0.001f ? forward.normalized : Vector3.forward;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (_cameraTransform == null)
                _cameraTransform = GetComponentInChildren<UnityEngine.Camera>()?.transform;

            _activeMode = StartupMode;
            _thirdPersonWeight = _activeMode == CameraViewMode.ThirdPerson ? 1f : 0f;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void HandleLook(Vector2 lookInput, Transform playerTransform)
        {
            Transform cameraTf = ActiveCameraTransform;
            if (cameraTf == null || playerTransform == null)
                return;

            float dt = Time.unscaledDeltaTime;
            if (dt <= 0f) return;

            if (_activeMode != CameraViewMode.Cinematic)
            {
                float timeScaleCompensation = Time.timeScale > 0.001f ? (1f / Time.timeScale) : 1f;
                _targetYaw += lookInput.x * SensitivityX * timeScaleCompensation;

                float invert = InvertY ? 1f : -1f;
                _targetPitch += lookInput.y * SensitivityY * timeScaleCompensation * invert;
                _targetPitch = Mathf.Clamp(_targetPitch, -MaxLookAngle, MaxLookAngle);
            }

            float rotSmooth = Mathf.Max(0.0001f, RotationSmoothTime);
            _smoothedYaw = Mathf.SmoothDampAngle(_smoothedYaw, _targetYaw, ref _yawVelocity, rotSmooth, Mathf.Infinity, dt);
            _smoothedPitch = Mathf.SmoothDampAngle(_smoothedPitch, _targetPitch, ref _pitchVelocity, rotSmooth, Mathf.Infinity, dt);

            if (_activeMode != CameraViewMode.Cinematic)
            {
                playerTransform.rotation = Quaternion.Euler(0f, _smoothedYaw, 0f);
            }

            float thirdPersonTarget = _activeMode == CameraViewMode.ThirdPerson ? 1f : 0f;
            _thirdPersonWeight = Mathf.SmoothDamp(
                _thirdPersonWeight,
                thirdPersonTarget,
                ref _thirdPersonWeightVelocity,
                ModeTransitionTime,
                Mathf.Infinity,
                dt);

            if (_activeMode == CameraViewMode.Cinematic && _cinematicTransform != null)
            {
                float t = 1f - Mathf.Exp(-dt / Mathf.Max(0.0001f, ModeTransitionTime));
                cameraTf.position = Vector3.Lerp(cameraTf.position, _cinematicTransform.position, t);
                cameraTf.rotation = Quaternion.Slerp(cameraTf.rotation, _cinematicTransform.rotation, t);
                return;
            }

            Vector3 firstPersonPosition = playerTransform.TransformPoint(FirstPersonLocalOffset);
            Quaternion firstPersonRotation = Quaternion.Euler(_smoothedPitch, _smoothedYaw, 0f);

            Vector3 pivot = playerTransform.position + Vector3.up * ThirdPersonHeightOffset;
            Quaternion orbit = Quaternion.Euler(_smoothedPitch, _smoothedYaw, 0f);
            Vector3 thirdPersonPosition = pivot - (orbit * Vector3.forward * ThirdPersonDistance);
            thirdPersonPosition = ResolveCameraCollision(pivot, thirdPersonPosition);
            Quaternion thirdPersonRotation = Quaternion.LookRotation((pivot - thirdPersonPosition).normalized, Vector3.up);

            cameraTf.position = Vector3.Lerp(firstPersonPosition, thirdPersonPosition, _thirdPersonWeight);
            cameraTf.rotation = Quaternion.Slerp(firstPersonRotation, thirdPersonRotation, _thirdPersonWeight);
        }

        public void ToggleFirstThirdPerson()
        {
            if (_activeMode == CameraViewMode.Cinematic)
                return;

            SetActiveMode(_activeMode == CameraViewMode.FirstPerson
                ? CameraViewMode.ThirdPerson
                : CameraViewMode.FirstPerson);
        }

        public void EnterCinematic(Transform cameraPoint, CinematicCameraZone zone = null)
        {
            if (cameraPoint == null) return;
            if (_activeMode != CameraViewMode.Cinematic)
                _modeBeforeCinematic = _activeMode;

            _cinematicTransform = cameraPoint;
            _activeZone = zone;
            if (SetActiveMode(CameraViewMode.Cinematic))
            {
                GameEventBus.RaiseCinematicEntered(cameraPoint);
            }
        }

        public void ExitCinematic(CinematicCameraZone zone = null)
        {
            if (_activeMode != CameraViewMode.Cinematic)
                return;
            if (zone != null && zone != _activeZone)
                return;

            CameraViewMode restoredMode = _modeBeforeCinematic;
            _cinematicTransform = null;
            _activeZone = null;
            if (SetActiveMode(restoredMode))
            {
                GameEventBus.RaiseCinematicExited(restoredMode);
            }
        }

        private bool SetActiveMode(CameraViewMode nextMode)
        {
            if (_activeMode == nextMode)
                return false;

            CameraViewMode previousMode = _activeMode;
            _activeMode = nextMode;
            GameEventBus.RaiseCameraViewModeChanged(previousMode, nextMode);
            return true;
        }

        private Vector3 ResolveCameraCollision(Vector3 pivot, Vector3 desiredPosition)
        {
            Vector3 dir = desiredPosition - pivot;
            float dist = dir.magnitude;
            if (dist <= 0.001f) return desiredPosition;

            dir /= dist;
            if (Physics.SphereCast(
                pivot,
                ThirdPersonCollisionRadius,
                dir,
                out RaycastHit hit,
                dist,
                CameraCollisionMask,
                QueryTriggerInteraction.Ignore))
            {
                return pivot + dir * Mathf.Max(0.05f, hit.distance - ThirdPersonCollisionRadius);
            }

            return desiredPosition;
        }
    }

    [RequireComponent(typeof(Collider))]
    public class CinematicCameraZone : MonoBehaviour
    {
        [SerializeField] private Transform _cinematicPoint;
        [SerializeField] private bool _returnOnExit = true;
        [SerializeField] private bool _drawGizmo = true;
        [SerializeField] private Color _gizmoColor = new Color(1f, 0.8f, 0.2f, 0.25f);

        private Collider _triggerCollider;

        private void Awake()
        {
            _triggerCollider = GetComponent<Collider>();
            _triggerCollider.isTrigger = true;
            if (_cinematicPoint == null)
                _cinematicPoint = transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CameraManager.Instance == null || !IsPlayer(other))
                return;
            CameraManager.Instance.EnterCinematic(_cinematicPoint, this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_returnOnExit || CameraManager.Instance == null || !IsPlayer(other))
                return;
            CameraManager.Instance.ExitCinematic(this);
        }

        private static bool IsPlayer(Collider other)
        {
            return other.GetComponentInParent<PlayerController>() != null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmo) return;

            var col = GetComponent<Collider>();
            if (col == null) return;

            Gizmos.color = _gizmoColor;
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = col.transform.localToWorldMatrix;

            if (col is BoxCollider box)
            {
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }

            Gizmos.matrix = old;
        }
    }
}
