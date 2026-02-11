using GlitchWorker.Player;
using UnityEngine;

namespace GlitchWorker.Camera
{
    [RequireComponent(typeof(Collider))]
    public class CinematicCameraZone : MonoBehaviour
    {
        [SerializeField] private Transform _cinematicPoint;
        [SerializeField] private bool _returnOnExit = true;
        [SerializeField] private bool _drawGizmo = true;
        [SerializeField] private Color _gizmoColor = new Color(1f, 0.8f, 0.2f, 0.25f);

        private Collider _triggerCollider;

        private void Awake()
        {
            _triggerCollider = GetComponent<Collider>();
            _triggerCollider.isTrigger = true;
            if (_cinematicPoint == null)
                _cinematicPoint = transform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CameraManager.Instance == null || !IsPlayer(other))
                return;
            CameraManager.Instance.EnterCinematic(_cinematicPoint, this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_returnOnExit || CameraManager.Instance == null || !IsPlayer(other))
                return;
            CameraManager.Instance.ExitCinematic(this);
        }

        private static bool IsPlayer(Collider other)
        {
            return other.GetComponentInParent<PlayerController>() != null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmo) return;

            var col = GetComponent<Collider>();
            if (col == null) return;

            Gizmos.color = _gizmoColor;
            Matrix4x4 old = Gizmos.matrix;
            Gizmos.matrix = col.transform.localToWorldMatrix;

            if (col is BoxCollider box)
            {
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }

            Gizmos.matrix = old;
        }
    }
}
