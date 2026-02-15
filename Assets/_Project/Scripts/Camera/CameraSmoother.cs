using UnityEngine;

namespace GlitchWorker.Camera
{
    public struct SmoothedRotationState
    {
        public float SmoothedYaw;
        public float SmoothedPitch;
        public float YawVelocity;
        public float PitchVelocity;
    }

    public static class CameraSmoother
    {
        public static void SmoothYawPitch(
            float targetYaw,
            float targetPitch,
            ref SmoothedRotationState state,
            float smoothTime,
            float deltaTime)
        {
            float st = Mathf.Max(0.0001f, smoothTime);
            state.SmoothedYaw = Mathf.SmoothDampAngle(
                state.SmoothedYaw, targetYaw,
                ref state.YawVelocity, st, Mathf.Infinity, deltaTime);
            state.SmoothedPitch = Mathf.SmoothDampAngle(
                state.SmoothedPitch, targetPitch,
                ref state.PitchVelocity, st, Mathf.Infinity, deltaTime);
        }

        public static void AccumulateLookInput(
            Vector2 lookInput,
            float sensitivityX,
            float sensitivityY,
            bool invertY,
            float maxPitch,
            float timeScaleCompensation,
            ref float targetYaw,
            ref float targetPitch)
        {
            targetYaw += lookInput.x * sensitivityX * timeScaleCompensation;
            float invert = invertY ? 1f : -1f;
            targetPitch += lookInput.y * sensitivityY * timeScaleCompensation * invert;
            targetPitch = Mathf.Clamp(targetPitch, -maxPitch, maxPitch);
        }
    }
}
