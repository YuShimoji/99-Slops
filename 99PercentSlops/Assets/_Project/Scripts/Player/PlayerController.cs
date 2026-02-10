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
        [Header("Experimental One-Hand Mouse Move")]
        [SerializeField] private bool _enableMouseOneHandMove;
        [SerializeField, Range(0f, 32f)] private float _mouseMoveDeadZone = 6f;
        [SerializeField, Range(0.001f, 0.1f)] private float _mouseForwardScale = 0.015f;
        [SerializeField, Range(0.001f, 0.1f)] private float _mouseStrafeScale = 0.02f;

        // Sibling component references
        private Rigidbody _rb;
        private PlayerMovement _movement;
        private PlayerJump _jump;
        private PlayerStateMachine _stateMachine;
        private PlayerStats _stats;
        private PlayerSlowMotion _slowMotion;
        private PlayerInput _playerInput;

        // Input state
        private Vector2 _moveInput;
        private Vector2 _resolvedMoveInput;
        private Vector2 _lookInput;
        private bool _jumpPressed;
        private bool _dashPressed;
        private bool _fastFallPressed;

        // Input buffer
        private InputBuffer _inputBuffer;

        // State context shared with all states
        private PlayerStateContext _context;
        private bool _isCursorLocked;

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
            _playerInput = GetComponent<PlayerInput>();

            _inputBuffer = new InputBuffer(8);
            EnsureInputIsActive();

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

            SetCursorLocked(true);
        }

        private void Update()
        {
            HandleCursorToggle();

            // Tick input buffer
            _inputBuffer.Tick();

            // Camera look (delegated to CameraManager stub)
            if (_isCursorLocked && CameraManager.Instance != null)
            {
                CameraManager.Instance.HandleLook(_lookInput, transform);
            }

            // Ground check
            if (_jump != null)
            {
                _jump.CheckGround();
            }

            // Update context input state
            _resolvedMoveInput = ResolveMoveInput();
            _context.MoveInput = _resolvedMoveInput;
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

        private void EnsureInputIsActive()
        {
            if (_playerInput == null) return;

            if (_playerInput.actions != null)
            {
                var playerMap = _playerInput.actions.FindActionMap("Player", throwIfNotFound: false);
                if (playerMap != null)
                {
                    if (!playerMap.enabled)
                    {
                        playerMap.Enable();
                    }

                    if (_playerInput.currentActionMap == null || _playerInput.currentActionMap.name != playerMap.name)
                    {
                        _playerInput.SwitchCurrentActionMap(playerMap.name);
                    }

                    if (string.IsNullOrEmpty(_playerInput.defaultActionMap))
                    {
                        _playerInput.defaultActionMap = playerMap.name;
                    }
                }
            }

            if (!_playerInput.inputIsActive)
            {
                _playerInput.ActivateInput();
            }
        }

        private void HandleCursorToggle()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetCursorLocked(false);
                return;
            }

            if (!_isCursorLocked && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                SetCursorLocked(true);
            }
        }

        private void SetCursorLocked(bool locked)
        {
            _isCursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
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

        public void OnTogglePerspective(InputValue value)
        {
            if (!value.isPressed || CameraManager.Instance == null) return;
            CameraManager.Instance.ToggleFirstThirdPerson();
        }

        #endregion

        #region Fallback (pre-state-machine direct control)

        private void FallbackUpdate()
        {
            // Movement input to PlayerMovement
            if (_movement != null)
            {
                _movement.SetMoveInput(_resolvedMoveInput);
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

        private Vector2 ResolveMoveInput()
        {
            Vector2 resolved = _moveInput;

            // Fallback keyboard polling if PlayerInput callbacks are not firing.
            if (resolved.sqrMagnitude < 0.0001f && Keyboard.current != null)
            {
                float x = 0f;
                float y = 0f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
                resolved = Vector2.ClampMagnitude(new Vector2(x, y), 1f);
            }

            if (!_enableMouseOneHandMove || Mouse.current == null)
                return resolved;

            bool left = Mouse.current.leftButton.isPressed;
            bool right = Mouse.current.rightButton.isPressed;
            if (!left && !right)
                return resolved;

            Vector2 delta = _lookInput;
            if (delta.magnitude < _mouseMoveDeadZone)
                return resolved;

            float forward = Mathf.Clamp01((Mathf.Abs(delta.x) + Mathf.Abs(delta.y)) * _mouseForwardScale);
            float strafe = Mathf.Clamp(delta.x * _mouseStrafeScale, -1f, 1f);
            return new Vector2(strafe, Mathf.Max(resolved.y, forward));
        }

        #endregion
    }
}
