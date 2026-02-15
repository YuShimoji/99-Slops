using UnityEngine;

namespace GlitchWorker.Camera
{
    public class CinematicMode
    {
        public Transform CinematicTransform { get; private set; }
        public CinematicCameraZone ActiveZone { get; private set; }

        public void Enter(Transform cameraPoint, CinematicCameraZone zone)
        {
            CinematicTransform = cameraPoint;
            ActiveZone = zone;
        }

        public void Exit()
        {
            CinematicTransform = null;
            ActiveZone = null;
        }

        public void ApplyLerp(Transform cameraTransform, float transitionTime, float dt)
        {
            if (CinematicTransform == null || cameraTransform == null) return;

            float t = 1f - Mathf.Exp(-dt / Mathf.Max(0.0001f, transitionTime));
            cameraTransform.position = Vector3.Lerp(
                cameraTransform.position, CinematicTransform.position, t);
            cameraTransform.rotation = Quaternion.Slerp(
                cameraTransform.rotation, CinematicTransform.rotation, t);
        }
    }
}
