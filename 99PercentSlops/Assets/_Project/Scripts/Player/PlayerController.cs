using UnityEngine;
using UnityEngine.InputSystem;
using GlitchWorker.Camera;
using GlitchWorker.Drone;
using GlitchWorker.Player.States;

namespace GlitchWorker.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Ground Check")]
        [SerializeField] private LayerMask _groundLayer = ~0;

        // Sibling component references
        private Rigidbody _rb;
        private PlayerMovement _movement;
        private PlayerJump _jump;
        private PlayerStateMachine _stateMachine;
        private PlayerStats _stats;
        private PlayerSlowMotion _slowMotion;

        // Input state
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _jumpPressed;
        private bool _dashPressed;
        private bool _fastFallPressed;

        // Input buffer
        private InputBuffer _inputBuffer;

        // State context shared with all states
        private PlayerStateContext _context;

        // Public accessors
        public PlayerStateType CurrentStateType => _stateMachine != null
            ? _stateMachine.CurrentStateType
            : PlayerStateType.Idle;

        public LayerMask GroundLayer => _groundLayer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            _movement = GetComponent<PlayerMovement>();
            _jump = GetComponent<PlayerJump>();
            _stateMachine = GetComponent<PlayerStateMachine>();
            _stats = GetComponent<PlayerStats>();
            _slowMotion = GetComponent<PlayerSlowMotion>();

            _inputBuffer = new InputBuffer(8);

            // DroneBeam may live on Drone object; find it in scene
            var droneBeam = FindFirstObjectByType<DroneBeam>();

            _context = new PlayerStateContext
            {
                Rb = _rb,
                Movement = _movement,
                Jump = _jump,
                Stats = _stats,
                StateMachine = _stateMachine,
                Buffer = _inputBuffer,
                DroneBeam = droneBeam
            };

            if (_stateMachine != null)
            {
                _stateMachine.Initialize(_context);
                RegisterStates();
                _stateMachine.TransitionTo(PlayerStateType.Idle);
            }

            // Initialize dash charges
            if (_stats != null)
            {
                _context.DashChargesRemaining = Mathf.RoundToInt(_stats.GetStat(StatType.DashCharges));
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // Tick input buffer
            _inputBuffer.Tick();

            // Camera look (delegated to CameraManager stub)
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.HandleLook(_lookInput, transform);
            }

            // Ground check
            if (_jump != null)
            {
                _jump.CheckGround();
            }

            // Update context input state
            _context.MoveInput = _moveInput;
            _context.JumpRequested = _jumpPressed;
            _context.DashRequested = _dashPressed;
            _context.FastFallRequested = _fastFallPressed;

            // Dash cooldown management
            if (_context.DashCooldownTimer > 0f)
            {
                _context.DashCooldownTimer -= Time.deltaTime;
                if (_context.DashCooldownTimer <= 0f)
                {
                    int maxCharges = Mathf.RoundToInt(_stats.GetStat(StatType.DashCharges));
                    if (_context.DashChargesRemaining < maxCharges)
                        _context.DashChargesRemaining = maxCharges;
                }
            }

            // Delegate to state machine
            if (_stateMachine != null)
            {
                _stateMachine.UpdateState();
            }
            else
            {
                // Fallback: direct movement when state machine not yet wired
                FallbackUpdate();
            }

            // Reset one-shot input flags
            _jumpPressed = false;
            _dashPressed = false;
            _fastFallPressed = false;
        }

        private void FixedUpdate()
        {
            if (_stateMachine != null)
            {
                _stateMachine.FixedUpdateState();
            }
            else
            {
                FallbackFixedUpdate();
            }
        }

        private void RegisterStates()
        {
            _stateMachine.RegisterState(new IdleState());
            _stateMachine.RegisterState(new RunState());
            _stateMachine.RegisterState(new JumpState());
            _stateMachine.RegisterState(new FallState());
            _stateMachine.RegisterState(new CoyoteHangState());
            _stateMachine.RegisterState(new LandingState());
            _stateMachine.RegisterState(new SlideOnSlopeState());
            _stateMachine.RegisterState(new DashState());
            _stateMachine.RegisterState(new FastFallState());
        }

        #region Input Callbacks (PlayerInput component events)

        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            _lookInput = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _jumpPressed = true;
                _inputBuffer.RecordInput(BufferableAction.Jump);
            }
        }

        public void OnDash(InputValue value)
        {
            if (value.isPressed)
            {
                _dashPressed = true;
                _inputBuffer.RecordInput(BufferableAction.Dash);
            }
        }

        public void OnFastFall(InputValue value)
        {
            if (value.isPressed)
            {
                _fastFallPressed = true;
            }
        }

        public void OnSlowMotion(InputValue value)
        {
            if (_slowMotion != null)
            {
                _slowMotion.OnInput(value.isPressed);
            }
        }

        #endregion

        #region Fallback (pre-state-machine direct control)

        private void FallbackUpdate()
        {
            // Movement input to PlayerMovement
            if (_movement != null)
            {
                _movement.SetMoveInput(_moveInput);
            }
        }

        private void FallbackFixedUpdate()
        {
            bool isGrounded = _jump != null && _jump.IsGrounded;

            if (_movement != null)
            {
                if (isGrounded)
                    _movement.ApplyGroundMovement();
                else
                    _movement.ApplyAirMovement();
            }

            if (_jump != null)
            {
                _jump.ApplyGravity();

                if (_jumpPressed && isGrounded)
                {
                    _jump.TryExecuteJump();
                }
            }
        }

        #endregion
    }
}
