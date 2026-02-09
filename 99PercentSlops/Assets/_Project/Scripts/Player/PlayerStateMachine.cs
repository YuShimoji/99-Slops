using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlitchWorker.Player
{
    public enum PlayerStateType
    {
        Idle,
        Run,
        Jump,
        Fall,
        CoyoteHang,
        Landing,
        SlideOnSlope,
        Dash,
        FastFall
    }

    public interface IPlayerState
    {
        PlayerStateType StateType { get; }
        void Enter(PlayerStateContext context);
        void Exit(PlayerStateContext context);
        void Update(PlayerStateContext context);
        void FixedUpdate(PlayerStateContext context);
    }

    public class PlayerStateMachine : MonoBehaviour
    {
        private readonly Dictionary<PlayerStateType, IPlayerState> _states = new();
        private IPlayerState _currentState;
        private PlayerStateContext _context;

        public PlayerStateType CurrentStateType => _currentState?.StateType ?? PlayerStateType.Idle;
        public event Action<PlayerStateType, PlayerStateType> OnStateChanged;

        public void Initialize(PlayerStateContext context)
        {
            _context = context;
        }

        public void RegisterState(IPlayerState state)
        {
            _states[state.StateType] = state;
        }

        public void TransitionTo(PlayerStateType newState)
        {
            if (!_states.ContainsKey(newState))
            {
                Debug.LogWarning($"[PlayerStateMachine] State {newState} is not registered.");
                return;
            }

            if (_currentState?.StateType == newState) return;

            var oldType = _currentState?.StateType ?? PlayerStateType.Idle;
            _currentState?.Exit(_context);
            _currentState = _states[newState];
            _context.StateTimer = 0f;
            _currentState.Enter(_context);

            Debug.Log($"[State] {oldType} -> {newState}");
            OnStateChanged?.Invoke(oldType, newState);
        }

        public void UpdateState()
        {
            if (_currentState == null) return;
            _context.StateTimer += Time.deltaTime;
            _currentState.Update(_context);
        }

        public void FixedUpdateState()
        {
            _currentState?.FixedUpdate(_context);
        }
    }
}
