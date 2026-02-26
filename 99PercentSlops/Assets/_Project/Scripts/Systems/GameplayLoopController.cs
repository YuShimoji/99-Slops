using UnityEngine;

namespace GlitchWorker.Systems
{
    public class GameplayLoopController : MonoBehaviour
    {
        public static GameplayLoopController Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool _enableDebugLogs = true;

        private GameplayState _currentState = GameplayState.Playing;
        public GameplayState CurrentState => _currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            SetState(GameplayState.Playing);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }

            if (_enableDebugLogs)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    TriggerCleared();
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    TriggerFailed();
                }
            }
        }

        public void SetState(GameplayState newState)
        {
            if (_currentState == newState) return;

            GameplayState previousState = _currentState;
            _currentState = newState;

            if (_enableDebugLogs)
            {
                Debug.Log($"[GameplayLoopController] State: {previousState} -> {newState}");
            }

            GameEventBus.RaiseGameplayStateChanged(previousState, newState);
        }

        public void TriggerCleared()
        {
            if (_currentState == GameplayState.Playing)
            {
                SetState(GameplayState.Cleared);
            }
        }

        public void TriggerFailed()
        {
            if (_currentState == GameplayState.Playing)
            {
                SetState(GameplayState.Failed);
            }
        }

        public void Restart()
        {
            if (_currentState == GameplayState.Cleared || _currentState == GameplayState.Failed)
            {
                SetState(GameplayState.Playing);
            }
        }
    }
}
