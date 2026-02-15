using UnityEngine;
using UnityEngine.InputSystem;
using GlitchWorker.Props;
using GlitchWorker.Systems;

namespace GlitchWorker.Drone
{
    public class DroneBeam : MonoBehaviour
    {
        [Header("Beam Settings")]
        [SerializeField] private float _grabRange = 10f;
        [SerializeField] private float _holdDistance = 3f;
        [SerializeField] private float _holdSpring = 50f;
        [SerializeField] private float _holdDamper = 5f;
        [SerializeField] private float _throwForce = 20f;
        [SerializeField] private LayerMask _grabbableLayer = ~0;

        [Header("References")]
        [SerializeField] private Transform _beamOrigin;
        [SerializeField] private UnityEngine.Camera _playerCamera;

        private Rigidbody _heldObject;
        private PropBase _heldProp;
        private SpringJoint _springJoint;

        private void Update()
        {
            if (_heldObject != null)
            {
                UpdateHeldObjectPosition();
            }
        }

        #region Input Callbacks

        // Remapped: Attack -> BeamFire (Right Click / RT)
        public void OnBeamFire(InputValue value)
        {
            if (value.isPressed)
            {
                if (_heldObject != null)
                {
                    ThrowObject();
                }
                else
                {
                    TryGrab();
                }
            }
        }

        // Legacy callback (kept for backward compatibility until InputActions are updated)
        public void OnAttack(InputValue value)
        {
            OnBeamFire(value);
        }

        // Remapped: Interact -> BeamRelease (E / X)
        public void OnBeamRelease(InputValue value)
        {
            if (value.isPressed && _heldObject != null)
            {
                ReleaseObject();
            }
        }

        // Legacy callback
        public void OnInteract(InputValue value)
        {
            OnBeamRelease(value);
        }

        #endregion

        private void TryGrab()
        {
            if (_playerCamera == null) return;

            Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _grabRange, _grabbableLayer))
            {
                Rigidbody targetRb = hit.rigidbody;
                if (targetRb == null) return;

                GrabObject(targetRb);
            }
        }

        private void GrabObject(Rigidbody target)
        {
            _heldObject = target;
            _heldProp = target.GetComponent<PropBase>();

            // Notify AI Prop it has been interacted with
            if (_heldProp != null && _heldProp is AIProp && _heldProp.CurrentState == PropState.Dormant)
            {
                _heldProp.SetState(PropState.Hostile);
            }

            _heldObject.useGravity = false;
            _heldObject.linearDamping = 5f;

            // Create spring joint for smooth holding
            _springJoint = _beamOrigin.gameObject.AddComponent<SpringJoint>();
            _springJoint.connectedBody = _heldObject;
            _springJoint.spring = _holdSpring;
            _springJoint.damper = _holdDamper;
            _springJoint.maxDistance = 0.2f;
            _springJoint.minDistance = 0f;
            _springJoint.autoConfigureConnectedAnchor = false;
            _springJoint.connectedAnchor = Vector3.zero;
            _springJoint.anchor = Vector3.zero;

            GameEventBus.RaiseBeamGrabbed(_heldObject, _heldProp);
        }

        private void ReleaseObject()
        {
            if (_heldObject == null) return;

            CleanupHold();
        }

        private void ThrowObject()
        {
            if (_heldObject == null) return;

            Rigidbody thrown = _heldObject;
            PropBase thrownProp = _heldProp;
            CleanupHold();

            Vector3 throwDirection = _playerCamera != null
                ? _playerCamera.transform.forward
                : transform.forward;

            thrown.AddForce(throwDirection * _throwForce, ForceMode.Impulse);
            GameEventBus.RaiseBeamThrown(thrown, thrownProp);
        }

        private void CleanupHold()
        {
            Rigidbody releasedBody = _heldObject;
            PropBase releasedProp = _heldProp;

            if (_springJoint != null)
            {
                Destroy(_springJoint);
                _springJoint = null;
            }

            if (_heldObject != null)
            {
                _heldObject.useGravity = true;
                _heldObject.linearDamping = 0f;
            }

            _heldObject = null;
            _heldProp = null;

            if (releasedBody != null)
            {
                GameEventBus.RaiseBeamReleased(releasedBody, releasedProp);
            }
        }

        private void UpdateHeldObjectPosition()
        {
            if (_beamOrigin == null || _playerCamera == null) return;

            Vector3 holdPosition = _playerCamera.transform.position
                + _playerCamera.transform.forward * _holdDistance;
            _beamOrigin.position = holdPosition;
        }

        public bool IsHolding => _heldObject != null;
        public Rigidbody HeldObject => _heldObject;

        /// <summary>
        /// Force-release the held object (e.g., when entering FastFall).
        /// Object inherits current velocity naturally via physics.
        /// </summary>
        public void ForceRelease()
        {
            if (_heldObject == null) return;
            CleanupHold();
        }
    }
}
