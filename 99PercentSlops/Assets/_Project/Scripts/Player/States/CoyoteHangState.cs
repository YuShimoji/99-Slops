using UnityEngine;

namespace GlitchWorker.Player.States
{
    /// <summary>
    /// Brief grace period after walking off a ledge. Allows late jump input.
    /// Independent state (peer to Fall), not a sub-state.
    /// </summary>
    public class CoyoteHangState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.CoyoteHang;

        public void Enter(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);

            // Transition: jump input -> Jump (this is the whole point)
            if (ctx.JumpRequested)
            {
                if (ctx.Jump.TryExecuteJump())
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.Jump);
                    return;
                }
            }

            // Transition: coyote timer expired -> Fall
            if (!ctx.Jump.IsCoyoteAvailable)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Fall);
                return;
            }

            // Transition: landed somehow (edge case)
            if (ctx.IsGrounded)
            {
                if (ctx.MoveInput.sqrMagnitude > 0.01f)
                    ctx.StateMachine.TransitionTo(PlayerStateType.Run);
                else
                    ctx.StateMachine.TransitionTo(PlayerStateType.Idle);
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
            ctx.Movement.ApplyAirMovement();
            // Slightly reduced gravity during coyote hang for "cartoon hang" feel
            ctx.Jump.ApplyGravity();
        }
    }
}
