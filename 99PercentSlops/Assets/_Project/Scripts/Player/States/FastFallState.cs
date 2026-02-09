using UnityEngine;

namespace GlitchWorker.Player.States
{
    /// <summary>
    /// Rapid downward acceleration triggered in air. Inspired by Smash Bros.
    /// Entering this state auto-releases held drone beam objects.
    /// Landing from FastFall enters LandingState (with lag + cancel frames).
    /// </summary>
    public class FastFallState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.FastFall;

        private float _fastFallMultiplier;
        private float _fastFallMaxSpeed;

        public void Enter(PlayerStateContext ctx)
        {
            _fastFallMultiplier = ctx.Stats.GetStat(StatType.FastFallMultiplier);
            _fastFallMaxSpeed = ctx.Stats.GetStat(StatType.FastFallMaxSpeed);

            // Auto-release held objects on FastFall entry
            if (ctx.DroneBeam != null && ctx.DroneBeam.IsHolding)
            {
                ctx.DroneBeam.ForceRelease();
            }
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);

            // Transition: landed
            if (ctx.IsGrounded)
            {
                // Check cancel inputs before entering Landing
                if (ctx.Buffer.ConsumeInput(BufferableAction.Jump))
                {
                    if (ctx.Jump.TryExecuteJump())
                    {
                        ctx.StateMachine.TransitionTo(PlayerStateType.Jump);
                        return;
                    }
                }

                if (ctx.Buffer.ConsumeInput(BufferableAction.Dash) && ctx.DashChargesRemaining > 0)
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                    return;
                }

                // Enter landing lag
                ctx.StateMachine.TransitionTo(PlayerStateType.Landing);
                return;
            }

            // Transition: dash
            if (ctx.DashRequested && ctx.DashChargesRemaining > 0)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                return;
            }
        }

        public void FixedUpdate(PlayerStateContext ctx)
        {
            // Enhanced downward gravity
            float extraGravity = Physics.gravity.y * (_fastFallMultiplier - 1f);
            ctx.Rb.AddForce(Vector3.up * extraGravity, ForceMode.Acceleration);

            // Clamp to max fast fall speed
            if (ctx.Rb.linearVelocity.y < -_fastFallMaxSpeed)
            {
                Vector3 v = ctx.Rb.linearVelocity;
                v.y = -_fastFallMaxSpeed;
                ctx.Rb.linearVelocity = v;
            }

            // Minimal air control during fast fall
            ctx.Movement.ApplyAirMovement();
        }
    }
}
