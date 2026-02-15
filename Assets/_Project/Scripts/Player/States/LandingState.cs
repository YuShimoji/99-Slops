using UnityEngine;

namespace GlitchWorker.Player.States
{
    /// <summary>
    /// Post-FastFall landing lag. Only entered from FastFall, not normal falls.
    /// Supports cancel frames via InputBuffer.
    /// </summary>
    public class LandingState : IPlayerState
    {
        public PlayerStateType StateType => PlayerStateType.Landing;

        private float _lagDuration;
        private float _lagTimer;

        public void Enter(PlayerStateContext ctx)
        {
            _lagDuration = ctx.Stats.GetStat(StatType.LandingLagDuration);
            _lagTimer = 0f;
        }

        public void Exit(PlayerStateContext ctx) { }

        public void Update(PlayerStateContext ctx)
        {
            _lagTimer += Time.deltaTime;

            // Cancel frames: check buffer for jump/dash during landing lag
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

            // Also check fresh input (not just buffered)
            if (ctx.JumpRequested)
            {
                if (ctx.Jump.TryExecuteJump())
                {
                    ctx.StateMachine.TransitionTo(PlayerStateType.Jump);
                    return;
                }
            }

            if (ctx.DashRequested && ctx.DashChargesRemaining > 0)
            {
                ctx.StateMachine.TransitionTo(PlayerStateType.Dash);
                return;
            }

            // Lag finished: transition to Idle or Run
            if (_lagTimer >= _lagDuration)
            {
                if (ctx.MoveInput.sqrMagnitude > 0.01f)
                    ctx.StateMachine.TransitionTo(PlayerStateType.Run);
                else
                    ctx.StateMachine.TransitionTo(PlayerStateType.Idle);
            }
        }

        public void FixedUpdate(PlayerStateContext ctx)
        {
            // Rapid deceleration during landing lag
            float decel = ctx.Stats.GetStat(StatType.Deceleration) * 2f;
            Vector3 horizontal = new Vector3(ctx.Rb.linearVelocity.x, 0f, ctx.Rb.linearVelocity.z);
            Vector3 newHorizontal = Vector3.MoveTowards(horizontal, Vector3.zero, decel * Time.fixedDeltaTime);
            ctx.Rb.linearVelocity = new Vector3(newHorizontal.x, ctx.Rb.linearVelocity.y, newHorizontal.z);

            ctx.Jump.ApplyGravity();
        }
    }
}
