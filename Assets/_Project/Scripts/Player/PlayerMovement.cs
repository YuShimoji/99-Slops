using UnityEngine;
using GlitchWorker.Camera;

namespace GlitchWorker.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody _rb;
        private PlayerStats _stats;

        // Input
        private Vector2 _moveInput;

        // Slope state
        private bool _isOnSteepSlope;
        private bool _wasOnSteepSlope;

        private const float SLOPE_EXIT_HYSTERESIS = 5f; // Exit slope slide 5° below SlopeLimit

        public bool IsOnSteepSlope => _isOnSteepSlope;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _stats = GetComponent<PlayerStats>();
        }

        public void SetMoveInput(Vector2 input)
        {
            _moveInput = input;
        }

        /// <summary>
        /// Returns the world-space move direction from input, projected onto camera forward.
        /// </summary>
        public Vector3 GetMoveDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.001f) return Vector3.zero;

            Vector3 camForward = Vector3.forward;
            if (CameraManager.Instance != null)
            {
                camForward = CameraManager.Instance.Forward;
            }
            else
            {
                camForward = transform.forward;
                camForward.y = 0f;
                camForward.Normalize();
            }

            Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;
            Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;
            return moveDir;
        }

        /// <summary>
        /// Ground movement with acceleration/deceleration curves.
        /// </summary>
        public void ApplyGroundMovement()
        {
            float moveSpeed = _stats.GetStat(StatType.MoveSpeed);
            float accel = _stats.GetStat(StatType.Acceleration);
            float decel = _stats.GetStat(StatType.Deceleration);

            Vector3 moveDir = GetMoveDirection(_moveInput);
            bool hasInput = moveDir.sqrMagnitude > 0.001f;

            Vector3 desiredVel = moveDir * moveSpeed;
            Vector3 currentHorizontal = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            float rate = hasInput ? accel : decel;
            Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, desiredVel, rate * Time.fixedDeltaTime);

            _rb.linearVelocity = new Vector3(newHorizontal.x, _rb.linearVelocity.y, newHorizontal.z);
        }

        /// <summary>
        /// Air movement with reduced acceleration and speed cap.
        /// MaxAirSpeed = MoveSpeed * AirSpeedRatio (derived stat).
        /// </summary>
        public void ApplyAirMovement()
        {
            float accel = _stats.GetStat(StatType.Acceleration);
            float decel = _stats.GetStat(StatType.Deceleration);
            float airAccelMult = _stats.GetStat(StatType.AirAccelMultiplier);
            float airDragMult = _stats.GetStat(StatType.AirDragMultiplier);
            float maxAirSpeed = _stats.GetMaxAirSpeed();

            Vector3 moveDir = GetMoveDirection(_moveInput);
            bool hasInput = moveDir.sqrMagnitude > 0.001f;

            Vector3 currentHorizontal = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            if (hasInput)
            {
                Vector3 desiredVel = moveDir * maxAirSpeed;
                float airAccel = accel * airAccelMult;
                Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, desiredVel, airAccel * Time.fixedDeltaTime);

                // Clamp to max air speed
                if (newHorizontal.magnitude > maxAirSpeed)
                    newHorizontal = newHorizontal.normalized * maxAirSpeed;

                _rb.linearVelocity = new Vector3(newHorizontal.x, _rb.linearVelocity.y, newHorizontal.z);
            }
            else
            {
                // Air drag (very gentle deceleration)
                float airDecel = decel * airDragMult;
                Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, Vector3.zero, airDecel * Time.fixedDeltaTime);
                _rb.linearVelocity = new Vector3(newHorizontal.x, _rb.linearVelocity.y, newHorizontal.z);
            }
        }

        /// <summary>
        /// Apply slope sliding force when on steep ground.
        /// </summary>
        public void ApplySlopeSlide(Vector3 groundNormal, float slopeAngle)
        {
            float slopeLimit = _stats.GetStat(StatType.SlopeLimit);
            float exitAngle = slopeLimit - SLOPE_EXIT_HYSTERESIS;

            // Hysteresis: enter at slopeLimit, exit at slopeLimit - 5°
            if (!_wasOnSteepSlope && slopeAngle > slopeLimit)
                _isOnSteepSlope = true;
            else if (_wasOnSteepSlope && slopeAngle < exitAngle)
                _isOnSteepSlope = false;

            _wasOnSteepSlope = _isOnSteepSlope;

            if (!_isOnSteepSlope) return;

            float slideGravity = _stats.GetStat(StatType.SlopeSlideGravity);
            float slideSpeed = _stats.GetStat(StatType.SlopeSlideSpeed);

            // Slide direction: project gravity onto the slope plane
            Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
            Vector3 slideForce = slideDir * slideGravity;

            _rb.AddForce(slideForce, ForceMode.Acceleration);

            // Clamp horizontal slide speed
            Vector3 horizontal = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (horizontal.magnitude > slideSpeed)
            {
                horizontal = horizontal.normalized * slideSpeed;
                _rb.linearVelocity = new Vector3(horizontal.x, _rb.linearVelocity.y, horizontal.z);
            }
        }

        /// <summary>
        /// Project movement onto slope surface for smooth traversal on walkable slopes.
        /// </summary>
        public void ApplyGroundMovementOnSlope(Vector3 groundNormal)
        {
            float moveSpeed = _stats.GetStat(StatType.MoveSpeed);
            float accel = _stats.GetStat(StatType.Acceleration);
            float decel = _stats.GetStat(StatType.Deceleration);

            Vector3 moveDir = GetMoveDirection(_moveInput);
            bool hasInput = moveDir.sqrMagnitude > 0.001f;

            // Project movement direction onto slope
            Vector3 slopeMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;
            Vector3 desiredVel = hasInput ? slopeMoveDir * moveSpeed : Vector3.zero;

            Vector3 currentVel = _rb.linearVelocity;
            float rate = hasInput ? accel : decel;
            Vector3 newVel = Vector3.MoveTowards(currentVel, desiredVel, rate * Time.fixedDeltaTime);

            // Preserve vertical component from physics when decelerating
            if (!hasInput)
                newVel.y = _rb.linearVelocity.y;

            _rb.linearVelocity = newVel;
        }
    }
}
