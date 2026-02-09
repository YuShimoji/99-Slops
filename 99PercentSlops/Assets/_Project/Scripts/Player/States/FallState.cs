using UnityEngine;

namespace GlitchWorker.Player.States
{
    public class FallState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.Fall;

        public void Enter(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            ctx.Movement.SetMoveInput(ctx.MoveInput);

            // Transition: landed
            if (ctx.IsGrounded)
            {
                // Check input buffer for jump (landing cancel)
                if (ctx.Buffer.ConsumeInput(BufferableAction.Jump))
                {
                    if (ctx.Jump.TryExecuteJump())
                    {
                        ctx.StateMachine.TransitionTo(PlayerStateType.Jump);
                        return;
                    }
                }

                // Check input buffer for dash
                if (ctx.Buffer.ConsumeInput(BufferableAction.Dash) && ctx.DashChargesRemaining > 0)
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                    return;
                }

                // Normal landing -> Idle or Run
                if (ctx.MoveInput.sqrMagnitude > 0.01f)
                    ctx.StateMachine.TransitionTo(PlayerStateType.Run);
                else
                    ctx.StateMachine.TransitionTo(PlayerStateType.Idle);
                return;
            }

            // Transition: dash -> Dash
            if (ctx.DashRequested && ctx.DashChargesRemaining > 0)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                return;
            }

            // Transition: fast fall
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
