using UnityEngine;
using GlitchWorker.Camera;

namespace GlitchWorker.Player.States
{
    public class DashState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.Dash;

        private enum DashPhase { Active, Recovery }

        private DashPhase _phase;
        private float _dashTimer;
        private float _dashDuration;
        private float _dashSpeed;
        private Vector3 _dashDirection;

        private const float RECOVERY_DURATION = 0.1f;

        public void Enter(PlayerStateContext ctx)
        {
            // Consume a charge
            ctx.DashChargesRemaining--;
            ctx.DashCooldownTimer = ctx.Stats.GetStat(StatType.DashCooldown);

            _dashSpeed = ctx.Stats.GetStat(StatType.DashSpeed);
            _dashDuration = ctx.Stats.GetStat(StatType.DashDuration);
            _dashTimer = 0f;
            _phase = DashPhase.Active;

            // Direction: camera forward (Y zeroed) or move input direction
            if (ctx.MoveInput.sqrMagnitude > 0.01f && ctx.Movement != null)
            {
                _dashDirection = ctx.Movement.GetMoveDirection(ctx.MoveInput);
            }
            else if (CameraManager.Instance != null)
            {
                _dashDirection = CameraManager.Instance.Forward;
            }
            else
            {
                _dashDirection = ctx.Rb.transform.forward;
                _dashDirection.y = 0f;
                _dashDirection.Normalize();
            }

            if (_dashDirection.sqrMagnitude < 0.001f)
                _dashDirection = ctx.Rb.transform.forward;

            // Snap player rotation to dash direction
            ctx.Rb.transform.forward = _dashDirection;
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            _dashTimer += Time.deltaTime;

            if (_phase == DashPhase.Active)
            {
                if (_dashTimer >= _dashDuration)
                {
                    _phase = DashPhase.Recovery;
                    _dashTimer = 0f;

                    // Cap velocity to normal move speed
                    float moveSpeed = ctx.Stats.GetStat(StatType.MoveSpeed);
                    Vector3 horizontal = new Vector3(ctx.Rb.linearVelocity.x, 0f, ctx.Rb.linearVelocity.z);
                    if (horizontal.magnitude > moveSpeed)
                    {
                        horizontal = horizontal.normalized * moveSpeed;
                        ctx.Rb.linearVelocity = new Vector3(horizontal.x, ctx.Rb.linearVelocity.y, horizontal.z);
                    }
                }
            }
            else // Recovery
            {
                // Cancel frame: jump
                if (ctx.JumpRequested || ctx.Buffer.ConsumeInput(BufferableAction.Jump))
                {
                    if (ctx.Jump.TryExecuteJump())
                    {
                        ctx.StateMachine.TransitionTo(PlayerStateType.Jump);
                        return;
                    }
                }

                // Cancel frame: dash (if charges available)
                if ((ctx.DashRequested || ctx.Buffer.ConsumeInput(BufferableAction.Dash))
                    && ctx.DashChargesRemaining > 0)
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                    return;
                }

                if (_dashTimer >= RECOVERY_DURATION)
                {
                    if (ctx.IsGrounded)
                    {
                        if (ctx.MoveInput.sqrMagnitude > 0.01f)
                            ctx.StateMachine.TransitionTo(PlayerStateType.Run);
                        else
                            ctx.StateMachine.TransitionTo(PlayerStateType.Idle);
                    }
                    else
                    {
                        ctx.StateMachine.TransitionTo(PlayerStateType.Fall);
                    }
                }
            }
        }

        public void FixedUpdate(PlayerStateContext ctx)
        {
            if (_phase == DashPhase.Active)
            {
                // Override velocity for consistent dash speed, zero gravity
                ctx.Rb.linearVelocity = _dashDirection * _dashSpeed;
            }
            else
            {
                // During recovery, let normal physics take over with deceleration
                ctx.Jump.ApplyGravity();
            }
        }
    }
}
