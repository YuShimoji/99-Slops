using UnityEngine;

namespace GlitchWorker.Drone
{
    public class DroneController : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform _followTarget;
        [SerializeField] private Vector3 _followOffset = new Vector3(0.8f, 0.5f, 0.3f);
        [SerializeField] private float _followSpeed = 5f;
        [SerializeField] private float _bobAmplitude = 0.1f;
        [SerializeField] private float _bobFrequency = 2f;

        private Vector3 _targetPosition;

        private void LateUpdate()
        {
            if (_followTarget == null) return;

            float bob = Mathf.Sin(Time.time * _bobFrequency) * _bobAmplitude;
            Vector3 offset = _followTarget.TransformDirection(_followOffset);
            _targetPosition = _followTarget.position + offset + Vector3.up * bob;

            transform.position = Vector3.Lerp(transform.position, _targetPosition, _followSpeed * Time.deltaTime);
            transform.LookAt(_followTarget.position + _followTarget.forward * 3f);
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
        }
    }
}
