using UnityEngine;

namespace GlitchWorker.Camera
{
    public class FirstPersonMode : ICameraMode
    {
        public CameraViewMode ModeType => CameraViewMode.FirstPerson;

        public (Vector3 position, Quaternion rotation) ComputePose(
            Transform playerTransform,
            float smoothedYaw,
            float smoothedPitch,
            CameraSettings settings)
        {
            Vector3 position = playerTransform.TransformPoint(settings.FirstPersonLocalOffset);
            Quaternion rotation = Quaternion.Euler(smoothedPitch, smoothedYaw, 0f);
            return (position, rotation);
        }
    }
}
