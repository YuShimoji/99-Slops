using UnityEngine;

namespace GlitchWorker.Player.States
{
    public class JumpState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.Jump;

        public void Enter(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);

            // Transition: reached apex (velocity going down) -> Fall
            if (ctx.Rb.linearVelocity.y <= 0f)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Fall);
                return;
            }

            // Transition: dash -> Dash
            if (ctx.DashRequested && ctx.DashChargesRemaining > 0)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                return;
            }

            // Transition: fast fall input + sufficient height
            if (ctx.FastFallRequested)
            {
                float minHeight = ctx.Stats.GetStat(StatType.FastFallEntryMinHeight);
                float height = ctx.Jump.GetHeightAboveGround();
                if (height >= minHeight)
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.FastFall);
                    return;
                }
            }
        }

        public void FixedUpdate(PlayerStateContext ctx)
        {
            ctx.Movement.ApplyAirMovement();
            ctx.Jump.ApplyGravity();
        }
    }
}
