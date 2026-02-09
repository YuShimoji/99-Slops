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

        // Coyote time
        private float _coyoteTimer;
        private bool _coyoteAvailable;

        // Public properties
        public bool IsGrounded => _isGrounded;
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

        /// <summary>
        /// Ground detection via SphereCast. Called every Update.
        /// </summary>
        public void CheckGround()
        {
            _wasGroundedLastFrame = _isGrounded;

            float radius = _stats.GetStat(StatType.GroundCheckRadius);
            float offset = _stats.GetStat(StatType.GroundCheckOffset);
            float capsuleHalfHeight = _capsule.height * 0.5f * transform.lossyScale.y;

            // Cast from center downward
            Vector3 origin = transform.position + Vector3.up * capsuleHalfHeight;
            float castDist = capsuleHalfHeight - radius + offset;

            if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, castDist, _groundLayers))
            {
                _isGrounded = true;
                _groundNormal = hit.normal;
                _slopeAngle = Vector3.Angle(_groundNormal, Vector3.up);
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
