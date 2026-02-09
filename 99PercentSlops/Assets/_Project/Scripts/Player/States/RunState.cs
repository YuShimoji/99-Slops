using UnityEngine;

namespace GlitchWorker.Player.States
{
    public class RunState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.Run;

        public void Enter(PlayerStateContext ctx) { }
        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);

            // Transition: no input -> Idle
            if (ctx.MoveInput.sqrMagnitude < 0.01f)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Idle);
                return;
            }

            // Transition: jump -> Jump
            if (ctx.JumpRequested)
            {
                if (ctx.Jump.TryExecuteJump())
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.Jump);
                    return;
                }
            }

            // Transition: dash -> Dash
            if (ctx.DashRequested && ctx.DashChargesRemaining > 0)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                return;
            }

            // Transition: left ground -> CoyoteHang or Fall
            if (!ctx.IsGrounded)
            {
                if (ctx.Jump.IsCoyoteAvailable)
                    ctx.StateMachine.TransitionTo(PlayerStateType.CoyoteHang);
                else
                    ctx.StateMachine.TransitionTo(PlayerStateType.Fall);
                return;
            }

            // Transition: steep slope -> SlideOnSlope
            if (ctx.Movement.IsOnSteepSlope)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.SlideOnSlope);
                return;
            }
        }

        public void FixedUpdate(PlayerStateContext ctx)
        {
            // Use slope-projected movement on mild slopes
            if (ctx.IsGrounded && ctx.SlopeAngle > 1f)
                ctx.Movement.ApplyGroundMovementOnSlope(ctx.GroundNormal);
            else
                ctx.Movement.ApplyGroundMovement();

            ctx.Jump.ApplyGravity();
        }
    }
}
