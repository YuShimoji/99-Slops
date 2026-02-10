using UnityEngine;

namespace GlitchWorker.Player.States
{
    public class IdleState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.Idle;

        public void Enter(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(Vector2.zero);
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            // Transition: move input -> Run
            if (ctx.MoveInput.sqrMagnitude > 0.01f)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Run);
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

            // Transition: left ground -> CoyoteHang or Fall
            if (!ctx.IsGrounded)
            {
                if (ctx.Rb.linearVelocity.y > -0.2f)
                {
                    return;
                }

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
            ctx.Movement.ApplyGroundMovement();
            ctx.Jump.ApplyGravity();
        }
    }
}
