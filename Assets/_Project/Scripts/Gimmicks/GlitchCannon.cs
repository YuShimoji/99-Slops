using UnityEngine;

namespace GlitchWorker.Gimmicks
{
    public class GlitchCannon : MonoBehaviour
    {
        [Header("Launch Settings")]
        [SerializeField] private float _launchForce = 80f;
        [SerializeField] private Vector3 _launchDirection = Vector3.up + Vector3.forward;
        [SerializeField] private float _triggerRadius = 2f;

        [Header("Visual")]
        [SerializeField] private Color _gizmoColor = Color.magenta;

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb == null) return;

            LaunchObject(rb);
        }

        private void LaunchObject(Rigidbody rb)
        {
            rb.isKinematic = false;

            // Reset velocity before launch for consistent behavior
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 direction = transform.TransformDirection(_launchDirection.normalized);
            rb.AddForce(direction * _launchForce, ForceMode.Impulse);

            // Add some tumble for visual comedy
            rb.AddTorque(Random.insideUnitSphere * _launchForce * 0.3f, ForceMode.Impulse);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawWireSphere(transform.position, _triggerRadius);

            Vector3 direction = transform.TransformDirection(_launchDirection.normalized);
            Gizmos.DrawRay(transform.position, direction * 5f);
        }
    }
}
