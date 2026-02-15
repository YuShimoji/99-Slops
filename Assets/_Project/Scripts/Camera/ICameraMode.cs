using UnityEngine;

namespace GlitchWorker.Camera
{
    public interface ICameraMode
    {
        CameraViewMode ModeType { get; }
        (Vector3 position, Quaternion rotation) ComputePose(
            Transform playerTransform,
            float smoothedYaw,
            float smoothedPitch,
            CameraSettings settings);
    }
}
