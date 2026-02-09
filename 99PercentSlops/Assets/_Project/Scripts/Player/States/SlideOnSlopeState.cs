using UnityEngine;

namespace GlitchWorker.Player.States
{
    /// <summary>
    /// Player slides down a slope steeper than SlopeLimit.
    /// Uses hysteresis to prevent jitter at the angle boundary.
    /// </summary>
    public class SlideOnSlopeState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.SlideOnSlope;

        public void Enter(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);

            // Transition: no longer on steep slope (hysteresis handled by PlayerMovement)
            if (!ctx.Movement.IsOnSteepSlope)
            {
                if (ctx.MoveInput.sqrMagnitude > 0.01f)
                    ctx.StateMachine.TransitionTo(PlayerStateType.Run);
                else
                    ctx.StateMachine.TransitionTo(PlayerStateType.Idle);
                return;
            }

            // Transition: jump from slope (reduced force)
            if (ctx.JumpRequested)
            {
                if (ctx.Jump.TryExecuteJump())
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.Jump);
                    return;
                }
            }

            // Transition: dash to escape slope
            if (ctx.DashRequested && ctx.DashChargesRemaining > 0)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                return;
            }

            // Transition: left ground entirely
            if (!ctx.IsGrounded)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Fall);
                return;
            }
        }

        public void FixedUpdate(PlayerStateContext ctx)
        {
            ctx.Movement.ApplySlopeSlide(ctx.GroundNormal, ctx.SlopeAngle);
            ctx.Jump.ApplyGravity();
        }
    }
}
