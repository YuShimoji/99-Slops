using UnityEngine;

namespace GlitchWorker.Player
{
    [CreateAssetMenu(fileName = "PlayerBaseStats", menuName = "GlitchWorker/Player Base Stats")]
    public class PlayerBaseStats : ScriptableObject
    {
        [Header("Movement")]
        public float MoveSpeed = 6.0f;
        public float Acceleration = 40.0f;
        public float Deceleration = 50.0f;
        public float AirAccelMultiplier = 0.3f;
        public float AirDragMultiplier = 0.1f;
        public float AirSpeedRatio = 1.0f;
        public float TurnSpeed = 720.0f;

        [Header("Jump")]
        public float JumpForce = 7.0f;
        public float GravityScale = 2.5f;
        public float FallGravityScale = 3.5f;
        public float MaxFallSpeed = 30.0f;
        public float CoyoteTime = 0.15f;
        public float JumpBufferTime = 0.1f;

        [Header("Ground Check")]
        public float GroundCheckRadius = 0.3f;
        public float GroundCheckOffset = 0.05f;
        public float SlopeLimit = 45.0f;
        public float SlopeSlideSpeed = 8.0f;
        public float SlopeSlideGravity = 15.0f;

        [Header("Dash")]
        public float DashSpeed = 18.0f;
        public float DashDuration = 0.25f;
        public float DashCooldown = 0.8f;
        public float DashCharges = 1f;
        public float DashTurnRate = 0.0f;

        [Header("Fast Fall")]
        public float FastFallMultiplier = 4.0f;
        public float FastFallMaxSpeed = 50.0f;
        public float FastFallEntryMinHeight = 1.0f;
        public float LandingLagDuration = 0.133f;

        [Header("Slow Motion")]
        public float SlowMotionScale = 0.3f;
        public float SlowMotionMaxDuration = 5.0f;
        public float SlowMotionGauge = 100.0f;
        public float SlowMotionDrainRate = 20.0f;
        public float SlowMotionRechargeRate = 10.0f;
        public float SlowMotionRechargeDelay = 2.0f;

        public float GetBase(StatType type)
        {
            return type switch
            {
                StatType.MoveSpeed => MoveSpeed,
                StatType.Acceleration => Acceleration,
                StatType.Deceleration => Deceleration,
                StatType.AirAccelMultiplier => AirAccelMultiplier,
                StatType.AirDragMultiplier => AirDragMultiplier,
                StatType.AirSpeedRatio => AirSpeedRatio,
                StatType.TurnSpeed => TurnSpeed,

                StatType.JumpForce => JumpForce,
                StatType.GravityScale => GravityScale,
                StatType.FallGravityScale => FallGravityScale,
                StatType.MaxFallSpeed => MaxFallSpeed,
                StatType.CoyoteTime => CoyoteTime,
                StatType.JumpBufferTime => JumpBufferTime,

                StatType.GroundCheckRadius => GroundCheckRadius,
                StatType.GroundCheckOffset => GroundCheckOffset,
                StatType.SlopeLimit => SlopeLimit,
                StatType.SlopeSlideSpeed => SlopeSlideSpeed,
                StatType.SlopeSlideGravity => SlopeSlideGravity,

                StatType.DashSpeed => DashSpeed,
                StatType.DashDuration => DashDuration,
                StatType.DashCooldown => DashCooldown,
                StatType.DashCharges => DashCharges,
                StatType.DashTurnRate => DashTurnRate,

                StatType.FastFallMultiplier => FastFallMultiplier,
                StatType.FastFallMaxSpeed => FastFallMaxSpeed,
                StatType.FastFallEntryMinHeight => FastFallEntryMinHeight,
                StatType.LandingLagDuration => LandingLagDuration,

                StatType.SlowMotionScale => SlowMotionScale,
                StatType.SlowMotionMaxDuration => SlowMotionMaxDuration,
                StatType.SlowMotionGauge => SlowMotionGauge,
                StatType.SlowMotionDrainRate => SlowMotionDrainRate,
                StatType.SlowMotionRechargeRate => SlowMotionRechargeRate,
                StatType.SlowMotionRechargeDelay => SlowMotionRechargeDelay,

                _ => 0f
            };
        }
    }
}
