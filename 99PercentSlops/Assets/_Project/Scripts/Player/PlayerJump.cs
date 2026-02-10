using UnityEngine;

namespace GlitchWorker.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayers = ~0;

        private Rigidbody _rb;
        private CapsuleCollider _capsule;
        private PlayerStats _stats;

        // Ground check state
        private bool _isGrounded;
        private bool _wasGroundedLastFrame;
        private Vector3 _groundNormal = Vector3.up;
        private float _slopeAngle;
        private bool _collisionGrounded;
        private Vector3 _collisionGroundNormal = Vector3.up;

        // Coyote time
        private float _coyoteTimer;
        private bool _coyoteAvailable;

        // Public properties
        public bool IsGrounded => _isGrounded || IsNearGroundFallback();
        public bool WasGroundedLastFrame => _wasGroundedLastFrame;
        public Vector3 GroundNormal => _groundNormal;
        public float SlopeAngle => _slopeAngle;
        public bool IsCoyoteAvailable => _coyoteTimer > 0f && _coyoteAvailable;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            _stats = GetComponent<PlayerStats>();
        }

        private void FixedUpdate()
        {
            // Recomputed every physics step via OnCollisionStay.
            _collisionGrounded = false;
            _collisionGroundNormal = Vector3.up;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!IsInLayerMask(collision.gameObject.layer, _groundLayers))
                return;

            int contactCount = collision.contactCount;
            for (int i = 0; i < contactCount; i++)
            {
                Vector3 n = collision.GetContact(i).normal;
                if (n.y > 0.2f)
                {
                    _collisionGrounded = true;
                    _collisionGroundNormal = n;
                    return;
                }
            }
        }

        /// <summary>
        /// Ground detection via SphereCast. Called every Update.
        /// </summary>
        public void CheckGround()
        {
            _wasGroundedLastFrame = _isGrounded;

            if (_collisionGrounded)
            {
                _isGrounded = true;
                _groundNormal = _collisionGroundNormal;
                _slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);
                UpdateCoyoteTime();
                return;
            }

            float radius = _stats.GetStat(StatType.GroundCheckRadius);
            float offset = _stats.GetStat(StatType.GroundCheckOffset);
            Vector3 worldCenter = transform.TransformPoint(_capsule.center);
            float scaledHeight = Mathf.Max(_capsule.height * transform.lossyScale.y, radius * 2f);
            float footToCenter = (scaledHeight * 0.5f) - radius;
            Vector3 footSphereCenter = worldCenter + Vector3.down * footToCenter;

            // Primary check: robustly detect contact even when already touching/interpenetrating the floor.
            Vector3 probeCenter = footSphereCenter + Vector3.down * Mathf.Max(0.0f, offset);
            bool isGroundedNow = Physics.CheckSphere(
                probeCenter,
                radius,
                _groundLayers,
                QueryTriggerInteraction.Ignore);

            if (isGroundedNow)
            {
                _isGrounded = true;

                // Secondary cast for slope information.
                if (Physics.SphereCast(
                    worldCenter,
                    radius * 0.5f,
                    Vector3.down,
                    out RaycastHit hit,
                    scaledHeight,
                    _groundLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    _groundNormal = hit.normal;
                    _slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);
                }
                else
                {
                    _groundNormal = Vector3.up;
                    _slopeAngle = 0f;
                }
            }
            else
            {
                _isGrounded = false;
                _groundNormal = Vector3.up;
                _slopeAngle = 0f;
            }

            // Coyote time logic
            UpdateCoyoteTime();
        }

        private static bool IsInLayerMask(int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        private bool IsNearGroundFallback()
        {
            if (_rb == null) return false;
            if (Mathf.Abs(_rb.linearVelocity.y) > 0.05f) return false;

            return Physics.Raycast(
                transform.position + Vector3.up * 0.05f,
                Vector3.down,
                1.2f,
                _groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void UpdateCoyoteTime()
        {
            float moveSpeed = _stats.GetStat(StatType.MoveSpeed);

            if (_wasGroundedLastFrame && !_isGrounded)
            {
                // Just left ground: check if walked off (not jumped or launched)
                float horizontalSpeed = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z).magnitude;

                if (_rb.linearVelocity.y <= 0.1f && horizontalSpeed <= moveSpeed * 2f)
                {
                    // Walked off a ledge: enable coyote time
                    _coyoteAvailable = true;
                    _coyoteTimer = _stats.GetStat(StatType.CoyoteTime);
                }
                else
                {
                    // Jumped or launched by external force: no coyote time
                    _coyoteAvailable = false;
                    _coyoteTimer = 0f;
                }
            }

            if (_isGrounded)
            {
                _coyoteAvailable = false;
                _coyoteTimer = 0f;
            }

            if (_coyoteTimer > 0f)
            {
                _coyoteTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Apply asymmetric gravity. Rising = GravityScale, Falling = FallGravityScale.
        /// Clamp fall speed to MaxFallSpeed.
        /// </summary>
        public void ApplyGravity()
        {
            float gravityScale = _rb.linearVelocity.y > 0.01f
                ? _stats.GetStat(StatType.GravityScale)
                : _stats.GetStat(StatType.FallGravityScale);

            // Apply extra gravity (beyond Unity's default 1x)
            float extraGravity = (gravityScale - 1f) * Physics.gravity.y;
            _rb.AddForce(Vector3.up * extraGravity, ForceMode.Acceleration);

            // Clamp fall speed
            float maxFall = _stats.GetStat(StatType.MaxFallSpeed);
            if (_rb.linearVelocity.y < -maxFall)
            {
                Vector3 v = _rb.linearVelocity;
                v.y = -maxFall;
                _rb.linearVelocity = v;
            }
        }

        /// <summary>
        /// Execute a jump if grounded or coyote time is available.
        /// Returns true if jump was executed.
        /// </summary>
        public bool TryExecuteJump()
        {
            bool canJump = _isGrounded || IsCoyoteAvailable;
            if (!canJump) return false;

            // Reset vertical velocity before jump for consistent height
            Vector3 v = _rb.linearVelocity;
            v.y = 0f;
            _rb.linearVelocity = v;

            float jumpForce = _stats.GetStat(StatType.JumpForce);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Consume coyote time
            _coyoteAvailable = false;
            _coyoteTimer = 0f;

            return true;
        }

        /// <summary>
        /// Check if the player is high enough above ground for fast fall.
        /// </summary>
        public float GetHeightAboveGround()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, _groundLayers))
                return hit.distance;

            return float.MaxValue;
        }
    }
}
