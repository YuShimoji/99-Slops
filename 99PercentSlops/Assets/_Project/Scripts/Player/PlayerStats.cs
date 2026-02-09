using System.Collections.Generic;
using UnityEngine;

namespace GlitchWorker.Player
{
    public enum StatType
    {
        // Movement
        MoveSpeed,
        Acceleration,
        Deceleration,
        AirAccelMultiplier,
        AirDragMultiplier,
        AirSpeedRatio,
        TurnSpeed,

        // Jump
        JumpForce,
        GravityScale,
        FallGravityScale,
        MaxFallSpeed,
        CoyoteTime,
        JumpBufferTime,

        // Ground
        GroundCheckRadius,
        GroundCheckOffset,
        SlopeLimit,
        SlopeSlideSpeed,
        SlopeSlideGravity,

        // Dash
        DashSpeed,
        DashDuration,
        DashCooldown,
        DashCharges,
        DashTurnRate,

        // Fast Fall
        FastFallMultiplier,
        FastFallMaxSpeed,
        FastFallEntryMinHeight,
        LandingLagDuration,

        // Slow Motion
        SlowMotionScale,
        SlowMotionMaxDuration,
        SlowMotionGauge,
        SlowMotionDrainRate,
        SlowMotionRechargeRate,
        SlowMotionRechargeDelay
    }

    public enum ModifierType
    {
        Flat,
        Percent,
        Override,
        Cap
    }

    public struct StatModifier
    {
        public ModifierType Type;
        public float Value;
        public int Priority;
        public object Source;

        public StatModifier(ModifierType type, float value, object source, int priority = 0)
        {
            Type = type;
            Value = value;
            Source = source;
            Priority = priority;
        }
    }

    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private PlayerBaseStats _baseStats;

        private readonly Dictionary<StatType, List<StatModifier>> _modifiers = new();

        public PlayerBaseStats BaseStats => _baseStats;

        /// <summary>
        /// Returns the final computed value for a stat, applying all modifiers.
        /// Supports derived stats (e.g., MaxAirSpeed = MoveSpeed * AirSpeedRatio).
        /// </summary>
        public float GetStat(StatType type)
        {
            float baseValue = _baseStats.GetBase(type);
            return ApplyModifiers(baseValue, type);
        }

        /// <summary>
        /// Derived stat: MaxAirSpeed = GetStat(MoveSpeed) * GetStat(AirSpeedRatio)
        /// </summary>
        public float GetMaxAirSpeed()
        {
            return GetStat(StatType.MoveSpeed) * GetStat(StatType.AirSpeedRatio);
        }

        public void AddModifier(StatType type, StatModifier mod)
        {
            if (!_modifiers.ContainsKey(type))
                _modifiers[type] = new List<StatModifier>();

            _modifiers[type].Add(mod);
            _modifiers[type].Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public bool RemoveModifier(StatType type, object source)
        {
            if (!_modifiers.ContainsKey(type)) return false;

            int removed = _modifiers[type].RemoveAll(m => m.Source == source);
            return removed > 0;
        }

        public void ClearModifiers(object source)
        {
            foreach (var kvp in _modifiers)
            {
                kvp.Value.RemoveAll(m => m.Source == source);
            }
        }

        private float ApplyModifiers(float baseValue, StatType type)
        {
            if (!_modifiers.ContainsKey(type) || _modifiers[type].Count == 0)
                return baseValue;

            float flat = 0f;
            float percent = 0f;
            float? overrideValue = null;
            float? capValue = null;

            foreach (var mod in _modifiers[type])
            {
                switch (mod.Type)
                {
                    case ModifierType.Flat:
                        flat += mod.Value;
                        break;
                    case ModifierType.Percent:
                        percent += mod.Value;
                        break;
                    case ModifierType.Override:
                        overrideValue = mod.Value;
                        break;
                    case ModifierType.Cap:
                        if (capValue == null || mod.Value < capValue.Value)
                            capValue = mod.Value;
                        break;
                }
            }

            if (overrideValue.HasValue)
                return overrideValue.Value;

            float result = (baseValue + flat) * (1f + percent);

            if (capValue.HasValue)
                result = Mathf.Min(result, capValue.Value);

            return result;
        }
    }
}
