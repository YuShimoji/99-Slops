using GlitchWorker.Player;
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
    /// Settings are loaded from CameraSettings ScriptableObject.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [Header("Settings")]
        [Tooltip("Camera設定アセット（未設定時はデフォルト値を使用）")]
        [SerializeField] private CameraSettings _settings;

        [Header("References")]
        [SerializeField] private Transform _cameraTransform;

        // フォールバック用の内部デフォルト設定
        private static class DefaultSettings
        {
            public const float SensitivityX = 2f;
            public const float SensitivityY = 2f;
            public const bool InvertY = false;
            public const float MaxLookAngle = 80f;
            public const float RotationSmoothTime = 0.06f;
            public static readonly Vector3 FirstPersonLocalOffset = new Vector3(0f, 0.8f, 0f);
            public const float ThirdPersonDistance = 3.2f;
            public const float ThirdPersonHeightOffset = 1.6f;
            public const float ThirdPersonCollisionRadius = 0.2f;
            public static readonly LayerMask CameraCollisionMask = ~0;
            public const float ModeTransitionTime = 0.22f;
            public const CameraViewMode StartupMode = CameraViewMode.FirstPerson;
        }

        // Settingsプロキシプロパティ（未設定時はデフォルト値を返す）
        private float SensitivityX => _settings?.SensitivityX ?? DefaultSettings.SensitivityX;
        private float SensitivityY => _settings?.SensitivityY ?? DefaultSettings.SensitivityY;
        private bool InvertY => _settings?.InvertY ?? DefaultSettings.InvertY;
        private float MaxLookAngle => _settings?.MaxLookAngle ?? DefaultSettings.MaxLookAngle;
        private float RotationSmoothTime => _settings?.RotationSmoothTime ?? DefaultSettings.RotationSmoothTime;
        private Vector3 FirstPersonLocalOffset => _settings?.FirstPersonLocalOffset ?? DefaultSettings.FirstPersonLocalOffset;
        private float ThirdPersonDistance => _settings?.ThirdPersonDistance ?? DefaultSettings.ThirdPersonDistance;
        private float ThirdPersonHeightOffset => _settings?.ThirdPersonHeightOffset ?? DefaultSettings.ThirdPersonHeightOffset;
        private float ThirdPersonCollisionRadius => _settings?.ThirdPersonCollisionRadius ?? DefaultSettings.ThirdPersonCollisionRadius;
        private LayerMask CameraCollisionMask => _settings?.CameraCollisionMask ?? DefaultSettings.CameraCollisionMask;
        private float ModeTransitionTime => _settings?.ModeTransitionTime ?? DefaultSettings.ModeTransitionTime;
        private CameraViewMode StartupMode => _settings?.StartupMode ?? DefaultSettings.StartupMode;

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

            // Settings検証
            if (_settings != null)
            {
                _settings.Validate();
            }

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
            if (_cameraTransform == null || playerTransform == null)
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
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cinematicTransform.position, t);
                _cameraTransform.rotation = Quaternion.Slerp(_cameraTransform.rotation, _cinematicTransform.rotation, t);
                return;
            }

            Vector3 firstPersonPosition = playerTransform.TransformPoint(FirstPersonLocalOffset);
            Quaternion firstPersonRotation = Quaternion.Euler(_smoothedPitch, _smoothedYaw, 0f);

            Vector3 pivot = playerTransform.position + Vector3.up * ThirdPersonHeightOffset;
            Quaternion orbit = Quaternion.Euler(_smoothedPitch, _smoothedYaw, 0f);
            Vector3 thirdPersonPosition = pivot - (orbit * Vector3.forward * ThirdPersonDistance);
            thirdPersonPosition = ResolveCameraCollision(pivot, thirdPersonPosition);
            Quaternion thirdPersonRotation = Quaternion.LookRotation((pivot - thirdPersonPosition).normalized, Vector3.up);

            _cameraTransform.position = Vector3.Lerp(firstPersonPosition, thirdPersonPosition, _thirdPersonWeight);
            _cameraTransform.rotation = Quaternion.Slerp(firstPersonRotation, thirdPersonRotation, _thirdPersonWeight);
        }

        public void ToggleFirstThirdPerson()
        {
            if (_activeMode == CameraViewMode.Cinematic)
                return;

            _activeMode = _activeMode == CameraViewMode.FirstPerson
                ? CameraViewMode.ThirdPerson
                : CameraViewMode.FirstPerson;
        }

        public void EnterCinematic(Transform cameraPoint, CinematicCameraZone zone = null)
        {
            if (cameraPoint == null) return;
            if (_activeMode != CameraViewMode.Cinematic)
                _modeBeforeCinematic = _activeMode;

            _cinematicTransform = cameraPoint;
            _activeZone = zone;
            _activeMode = CameraViewMode.Cinematic;
        }

        public void ExitCinematic(CinematicCameraZone zone = null)
        {
            if (_activeMode != CameraViewMode.Cinematic)
                return;
            if (zone != null && zone != _activeZone)
                return;

            _cinematicTransform = null;
            _activeZone = null;
            _activeMode = _modeBeforeCinematic;
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
