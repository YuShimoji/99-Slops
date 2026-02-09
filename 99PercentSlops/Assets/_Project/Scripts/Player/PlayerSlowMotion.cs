using UnityEngine;

namespace GlitchWorker.Player
{
    /// <summary>
    /// Independent slow motion component (not part of the state machine).
    /// Manipulates Time.timeScale with gauge management.
    /// Supports both toggle and hold activation modes.
    /// </summary>
    public class PlayerSlowMotion : MonoBehaviour
    {
        public enum ActivationMode { Toggle, Hold }

        [SerializeField] private ActivationMode _activationMode = ActivationMode.Toggle;

        private PlayerStats _stats;

        // Runtime state
        private float _currentGauge;
        private bool _isActive;
        private float _rechargeDelayTimer;
        private float _targetTimeScale = 1f;

        // Transition timing
        private const float ENTER_TRANSITION_TIME = 0.1f;
        private const float EXIT_TRANSITION_TIME = 0.15f;

        // Cached original values
        private float _originalFixedDeltaTime;

        // Public accessors
        public bool IsActive => _isActive;
        public float CurrentGauge => _currentGauge;
        public float MaxGauge => _stats != null ? _stats.GetStat(StatType.SlowMotionGauge) : 100f;
        public float GaugeNormalized => _currentGauge / MaxGauge;
        public ActivationMode Mode => _activationMode;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
            _originalFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Start()
        {
            _currentGauge = MaxGauge;
        }

        /// <summary>
        /// Toggle mode: flip slow motion on/off.
        /// </summary>
        public void ToggleSlowMotion()
        {
            if (_isActive)
                Deactivate();
            else if (_currentGauge > 0f)
                Activate();
        }

        /// <summary>
        /// Hold mode: activate/deactivate based on button state.
        /// </summary>
        public void HoldSlowMotion(bool pressed)
        {
            if (pressed && !_isActive && _currentGauge > 0f)
                Activate();
            else if (!pressed && _isActive)
                Deactivate();
        }

        /// <summary>
        /// Called from PlayerController.OnSlowMotion().
        /// Routes to appropriate mode.
        /// </summary>
        public void OnInput(bool pressed)
        {
            switch (_activationMode)
            {
                case ActivationMode.Toggle:
                    if (pressed) ToggleSlowMotion();
                    break;
                case ActivationMode.Hold:
                    HoldSlowMotion(pressed);
                    break;
            }
        }

        private void Activate()
        {
            _isActive = true;
            _targetTimeScale = _stats.GetStat(StatType.SlowMotionScale);
        }

        private void Deactivate()
        {
            _isActive = false;
            _targetTimeScale = 1.0f;
            _rechargeDelayTimer = _stats.GetStat(StatType.SlowMotionRechargeDelay);
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;

            // Smooth timeScale transition
            float transitionSpeed = _isActive
                ? dt / ENTER_TRANSITION_TIME
                : dt / EXIT_TRANSITION_TIME;
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, _targetTimeScale, transitionSpeed);
            Time.fixedDeltaTime = _originalFixedDeltaTime * Time.timeScale;

            if (_isActive)
            {
                // Drain gauge (using unscaled time)
                float drainRate = _stats.GetStat(StatType.SlowMotionDrainRate);
                _currentGauge -= drainRate * dt;

                if (_currentGauge <= 0f)
                {
                    _currentGauge = 0f;
                    Deactivate();
                }
            }
            else
            {
                // Recharge after delay
                if (_rechargeDelayTimer > 0f)
                {
                    _rechargeDelayTimer -= dt;
                }
                else
                {
                    float rechargeRate = _stats.GetStat(StatType.SlowMotionRechargeRate);
                    float maxGauge = MaxGauge;
                    _currentGauge = Mathf.Min(_currentGauge + rechargeRate * dt, maxGauge);
                }
            }
        }

        private void OnDisable()
        {
            // Safety: restore timeScale when component is disabled
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = _originalFixedDeltaTime;
            _isActive = false;
            _targetTimeScale = 1f;
        }

        private void OnDestroy()
        {
            // Double safety
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = _originalFixedDeltaTime;
        }
    }
}
