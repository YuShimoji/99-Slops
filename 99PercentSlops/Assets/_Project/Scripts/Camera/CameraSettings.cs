using UnityEngine;

namespace GlitchWorker.Camera
{
    [CreateAssetMenu(fileName = "CameraSettings_Default", menuName = "GlitchWorker/Camera Settings")]
    public class CameraSettings : ScriptableObject
    {
        [Header("Look")]
        public float SensitivityX = 2f;
        public float SensitivityY = 2f;
        public bool InvertY = false;
        public float MaxLookAngle = 80f;
        [Min(0f)] public float RotationSmoothTime = 0.06f;

        [Header("First Person")]
        public Vector3 FirstPersonLocalOffset = new Vector3(0f, 0.8f, 0f);

        [Header("Third Person")]
        [Min(0.1f)] public float ThirdPersonDistance = 3.2f;
        [Min(0f)] public float ThirdPersonHeightOffset = 1.6f;
        [Min(0f)] public float ThirdPersonCollisionRadius = 0.2f;
        public LayerMask CameraCollisionMask = ~0;

        [Header("Mode Transition")]
        [Min(0.01f)] public float ModeTransitionTime = 0.22f;
        public CameraViewMode StartupMode = CameraViewMode.FirstPerson;
    }
}
