using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GlitchWorker.Systems;
using GlitchWorker.Gimmicks;

namespace GlitchWorker.UI
{
    public class GameplayHudPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UploadPort _uploadPort;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private TextMeshProUGUI _stateText;
        [SerializeField] private GameObject _restartHintPanel;
        [SerializeField] private TextMeshProUGUI _restartHintLabel;

        [Header("State Display")]
        [SerializeField] private string _playingStateText = "PLAYING";
        [SerializeField] private string _clearedStateText = "CLEARED!";
        [SerializeField] private string _failedStateText = "FAILED";
        [SerializeField] private string _restartHintText = "Press [R] to Restart";

        [Header("Debug")]
        [SerializeField] private bool _enableDebugLogs = true;
        private bool _missingUploadPortLogged;

        private void Awake()
        {
            ResolveRestartHintLabel();
            ApplyRestartHintText();
        }

        private void OnEnable()
        {
            GameEventBus.GameplayStateChanged += OnGameplayStateChanged;
            GameEventBus.GameplayRestarted += OnGameplayRestarted;
        }

        private void OnDisable()
        {
            GameEventBus.GameplayStateChanged -= OnGameplayStateChanged;
            GameEventBus.GameplayRestarted -= OnGameplayRestarted;
        }

        private void Start()
        {
            UpdateProgressDisplay();
            GameplayState initialState = GameplayLoopController.Instance != null
                ? GameplayLoopController.Instance.CurrentState
                : GameplayState.Playing;
            UpdateStateDisplay(initialState);
        }

        private void Update()
        {
            UpdateProgressDisplay();
        }

        private void UpdateProgressDisplay()
        {
            if (_uploadPort == null)
            {
                if (_enableDebugLogs && !_missingUploadPortLogged)
                {
                    Debug.LogWarning("[GameplayHudPresenter] UploadPort reference is missing.");
                    _missingUploadPortLogged = true;
                }
                return;
            }

            if (_progressText == null) return;

            int current = _uploadPort.CurrentProgress;
            int required = _uploadPort.RequiredCount;
            _progressText.text = $"Progress: {current}/{required}";
        }

        private void OnGameplayStateChanged(GameplayState previousState, GameplayState newState)
        {
            UpdateStateDisplay(newState);

            if (_enableDebugLogs)
            {
                Debug.Log($"[GameplayHudPresenter] State changed: {previousState} -> {newState}");
            }
        }

        private void OnGameplayRestarted()
        {
            UpdateProgressDisplay();
            UpdateStateDisplay(GameplayState.Playing);

            if (_enableDebugLogs)
            {
                Debug.Log("[GameplayHudPresenter] Gameplay restarted, UI reset.");
            }
        }

        private void ResolveRestartHintLabel()
        {
            if (_restartHintLabel != null || _restartHintPanel == null)
            {
                return;
            }

            _restartHintLabel = _restartHintPanel.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        private void ApplyRestartHintText()
        {
            if (_restartHintLabel != null)
            {
                _restartHintLabel.text = _restartHintText;
            }
        }

        private void UpdateStateDisplay(GameplayState state)
        {
            if (_stateText != null)
            {
                switch (state)
                {
                    case GameplayState.Playing:
                        _stateText.text = _playingStateText;
                        break;
                    case GameplayState.Cleared:
                        _stateText.text = _clearedStateText;
                        break;
                    case GameplayState.Failed:
                        _stateText.text = _failedStateText;
                        break;
                }
            }

            if (_restartHintPanel != null)
            {
                bool showRestartHint = (state == GameplayState.Cleared || state == GameplayState.Failed);
                _restartHintPanel.SetActive(showRestartHint);
            }
        }
    }
}
