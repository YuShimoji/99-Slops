using UnityEngine;

namespace GlitchWorker.Camera
{
    /// <summary>
    /// Minimal camera manager stub for Phase 2B.
    /// Wraps the existing main camera. Will be replaced with full
    /// implementation (ICameraMode, 1P/3P switching) in Phase 2A.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [SerializeField] private float _mouseSensitivity = 2f;
        [SerializeField] private float _maxLookAngle = 80f;
        [SerializeField] private Transform _cameraHolder;

        private float _cameraPitch;

        public Transform ActiveCameraTransform => _cameraHolder != null
            ? _cameraHolder
            : UnityEngine.Camera.main?.transform;

        public Vector3 Forward
        {
            get
            {
                if (ActiveCameraTransform == null) return Vector3.forward;
                Vector3 fwd = ActiveCameraTransform.forward;
                fwd.y = 0f;
                return fwd.normalized;
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

            if (_cameraHolder == null)
            {
                _cameraHolder = GetComponentInChildren<UnityEngine.Camera>()?.transform;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Handles mouse look. Called from PlayerController.Update().
        /// Uses unscaledDeltaTime for SlowMotion compatibility.
        /// </summary>
        public void HandleLook(Vector2 lookInput, Transform playerTransform)
        {
            if (_cameraHolder == null || playerTransform == null) return;

            // Compensate for timeScale so camera sensitivity stays constant during SlowMotion.
            // Input System mouse delta shrinks with timeScale; multiply by inverse to cancel.
            float timeScaleCompensation = (Time.timeScale > 0.001f) ? (1f / Time.timeScale) : 1f;
            float yaw = lookInput.x * _mouseSensitivity * timeScaleCompensation;
            float pitch = lookInput.y * _mouseSensitivity * timeScaleCompensation;

            playerTransform.Rotate(Vector3.up, yaw);

            _cameraPitch -= pitch;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -_maxLookAngle, _maxLookAngle);
            _cameraHolder.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
        }
    }
}
