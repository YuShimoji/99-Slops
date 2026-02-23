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
    /// Camera orchestrator: delegates pose computation to ICameraMode instances,
    /// manages mode transitions and input smoothing.
    /// Public API is unchanged from the monolithic version.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private CameraSettings _settings;

        // ── Mode instances ──
        private FirstPersonMode _firstPersonMode;
        private ThirdPersonMode _thirdPersonMode;
        private CinematicMode _cinematicMode;

        // ── Look state ──
        private float _targetYaw;
        private float _targetPitch;
        private SmoothedRotationState _smoothState;

        // ── Mode transition ──
        private float _thirdPersonWeight;
        private float _thirdPersonWeightVelocity;

        private CameraViewMode _activeMode;
        private CameraViewMode _modeBeforeCinematic;
        private bool _modeInitialized;

        // ── Public API (unchanged) ──

        public Transform ActiveCameraTransform
        {
            get
            {
                EnsureInitialized();
                return _cameraTransform != null
                    ? _cameraTransform
                    : UnityEngine.Camera.main?.transform;
            }
        }

        public CameraViewMode ActiveMode
        {
            get
            {
                EnsureInitialized();
                return _activeMode;
            }
        }

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

        // ── Lifecycle ──

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsureInitialized();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // ── Core camera update (called by PlayerController) ──

        public void HandleLook(Vector2 lookInput, Transform playerTransform)
        {
            EnsureInitialized();

            if (_cameraTransform == null || playerTransform == null)
                return;

            float dt = Time.unscaledDeltaTime;
            if (dt <= 0f) return;

            // 1. Accumulate input (skip during cinematic)
            if (_activeMode != CameraViewMode.Cinematic)
            {
                float timeScaleCompensation = Time.timeScale > 0.001f ? (1f / Time.timeScale) : 1f;
                CameraSmoother.AccumulateLookInput(
                    lookInput,
                    _settings.SensitivityX,
                    _settings.SensitivityY,
                    _settings.InvertY,
                    _settings.MaxLookAngle,
                    timeScaleCompensation,
                    ref _targetYaw,
                    ref _targetPitch);
            }

            // 2. Smooth rotation
            CameraSmoother.SmoothYawPitch(
                _targetYaw, _targetPitch,
                ref _smoothState,
                _settings.RotationSmoothTime,
                dt);

            // 3. Apply player yaw rotation (skip during cinematic)
            if (_activeMode != CameraViewMode.Cinematic)
            {
                playerTransform.rotation = Quaternion.Euler(0f, _smoothState.SmoothedYaw, 0f);
            }

            // 4. Update 1P↔3P blend weight
            float thirdPersonTarget = _activeMode == CameraViewMode.ThirdPerson ? 1f : 0f;
            _thirdPersonWeight = Mathf.SmoothDamp(
                _thirdPersonWeight,
                thirdPersonTarget,
                ref _thirdPersonWeightVelocity,
                _settings.ModeTransitionTime,
                Mathf.Infinity,
                dt);

            // 5. Cinematic override
            if (_activeMode == CameraViewMode.Cinematic)
            {
                _cinematicMode.ApplyLerp(_cameraTransform, _settings.ModeTransitionTime, dt);
                return;
            }

            // 6. Compute 1P and 3P poses, then blend
            var (fpPos, fpRot) = _firstPersonMode.ComputePose(
                playerTransform,
                _smoothState.SmoothedYaw,
                _smoothState.SmoothedPitch,
                _settings);

            var (tpPos, tpRot) = _thirdPersonMode.ComputePose(
                playerTransform,
                _smoothState.SmoothedYaw,
                _smoothState.SmoothedPitch,
                _settings);

            _cameraTransform.position = Vector3.Lerp(fpPos, tpPos, _thirdPersonWeight);
            _cameraTransform.rotation = Quaternion.Slerp(fpRot, tpRot, _thirdPersonWeight);
        }

        // ── Mode switching (unchanged signatures) ──

        public void ToggleFirstThirdPerson()
        {
            EnsureInitialized();

            if (_activeMode == CameraViewMode.Cinematic)
                return;

            var previousMode = _activeMode;
            _activeMode = _activeMode == CameraViewMode.FirstPerson
                ? CameraViewMode.ThirdPerson
                : CameraViewMode.FirstPerson;

            GlitchWorker.Systems.GameEventBus.RaiseCameraModeChanged(previousMode, _activeMode);
        }

        public void EnterCinematic(Transform cameraPoint, CinematicCameraZone zone = null)
        {
            EnsureInitialized();

            if (cameraPoint == null) return;
            if (_activeMode != CameraViewMode.Cinematic)
                _modeBeforeCinematic = _activeMode;

            var previousMode = _activeMode;
            _cinematicMode.Enter(cameraPoint, zone);
            _activeMode = CameraViewMode.Cinematic;

            GlitchWorker.Systems.GameEventBus.RaiseCinematicEntered(cameraPoint, zone);
            GlitchWorker.Systems.GameEventBus.RaiseCameraModeChanged(previousMode, _activeMode);
        }

        public void ExitCinematic(CinematicCameraZone zone = null)
        {
            EnsureInitialized();

            if (_activeMode != CameraViewMode.Cinematic)
                return;
            if (zone != null && zone != _cinematicMode.ActiveZone)
                return;

            var previousMode = _activeMode;
            _cinematicMode.Exit();
            _activeMode = _modeBeforeCinematic;

            GlitchWorker.Systems.GameEventBus.RaiseCinematicExited();
            GlitchWorker.Systems.GameEventBus.RaiseCameraModeChanged(previousMode, _activeMode);
        }

        // ── Internal helpers ──

        private static CameraSettings CreateFallbackSettings()
        {
            var settings = ScriptableObject.CreateInstance<CameraSettings>();
            // Defaults match the original hardcoded values
            return settings;
        }

        private void EnsureInitialized()
        {
            if (_cameraTransform == null)
                _cameraTransform = GetComponentInChildren<UnityEngine.Camera>()?.transform;

            if (_settings == null)
                _settings = CreateFallbackSettings();

            _firstPersonMode ??= new FirstPersonMode();
            _thirdPersonMode ??= new ThirdPersonMode();
            _cinematicMode ??= new CinematicMode();

            if (_modeInitialized) return;

            _activeMode = _settings.StartupMode;
            _thirdPersonWeight = _activeMode == CameraViewMode.ThirdPerson ? 1f : 0f;
            _modeInitialized = true;
        }
    }
}
