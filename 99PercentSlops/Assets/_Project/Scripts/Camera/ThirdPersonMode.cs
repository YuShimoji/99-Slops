using UnityEngine;

namespace GlitchWorker.Camera
{
    public class ThirdPersonMode : ICameraMode
    {
        public CameraViewMode ModeType => CameraViewMode.ThirdPerson;

        public (Vector3 position, Quaternion rotation) ComputePose(
            Transform playerTransform,
            float smoothedYaw,
            float smoothedPitch,
            CameraSettings settings)
        {
            Vector3 pivot = playerTransform.position + Vector3.up * settings.ThirdPersonHeightOffset;
            Quaternion orbit = Quaternion.Euler(smoothedPitch, smoothedYaw, 0f);
            Vector3 desiredPosition = pivot - (orbit * Vector3.forward * settings.ThirdPersonDistance);

            Vector3 position = ResolveCollision(pivot, desiredPosition, settings);
            Quaternion rotation = Quaternion.LookRotation((pivot - position).normalized, Vector3.up);
            return (position, rotation);
        }

        private static Vector3 ResolveCollision(
            Vector3 pivot,
            Vector3 desiredPosition,
            CameraSettings settings)
        {
            Vector3 dir = desiredPosition - pivot;
            float dist = dir.magnitude;
            if (dist <= 0.001f) return desiredPosition;

            dir /= dist;
            if (Physics.SphereCast(
                pivot,
                settings.ThirdPersonCollisionRadius,
                dir,
                out RaycastHit hit,
                dist,
                settings.CameraCollisionMask,
                QueryTriggerInteraction.Ignore))
            {
                return pivot + dir * Mathf.Max(0.05f, hit.distance - settings.ThirdPersonCollisionRadius);
            }

            return desiredPosition;
        }
    }
}
