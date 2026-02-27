using UnityEngine;
using GlitchWorker.Props;
using GlitchWorker.Systems;

namespace GlitchWorker.Gimmicks
{
    public class UploadPort : MonoBehaviour
    {
        private const int MinRequiredCount = 1;

        [Header("Objective Settings")]
        [SerializeField] private int _requiredCount = 3;
        [SerializeField] private PropType _acceptedPropType = PropType.AI;
        [SerializeField] private PropState _acceptedPropState = PropState.Normalized;

        [Header("Debug")]
        [SerializeField] private bool _enableDebugLogs = true;

        private int _currentProgress = 0;
        public int CurrentProgress => _currentProgress;
        public int RequiredCount => _requiredCount;

        private void Awake()
        {
            _requiredCount = Mathf.Max(MinRequiredCount, _requiredCount);
        }

        private void OnValidate()
        {
            _requiredCount = Mathf.Max(MinRequiredCount, _requiredCount);
        }

        private void OnEnable()
        {
            GameEventBus.GameplayRestarted += OnGameplayRestarted;
        }

        private void OnDisable()
        {
            GameEventBus.GameplayRestarted -= OnGameplayRestarted;
        }

        private void OnGameplayRestarted()
        {
            ResetProgress();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!CanAcceptObjectiveProgress())
            {
                return;
            }

            PropBase prop = other.GetComponent<PropBase>();
            if (prop == null) return;

            if (IsAcceptable(prop))
            {
                AcceptProp(prop);
            }
            else
            {
                if (_enableDebugLogs)
                {
                    Debug.Log($"[UploadPort] Rejected: {prop.name} (Type={prop.Type}, State={prop.CurrentState})");
                }
            }
        }

        private bool CanAcceptObjectiveProgress()
        {
            if (GameplayLoopController.Instance == null)
            {
                if (_enableDebugLogs)
                {
                    Debug.LogWarning("[UploadPort] Ignored trigger: GameplayLoopController.Instance is null.");
                }
                return false;
            }

            bool canAccept = GameplayLoopController.Instance.CanTransitionToCleared();
            if (!canAccept && _enableDebugLogs)
            {
                Debug.LogWarning($"[UploadPort] Ignored trigger: current state is {GameplayLoopController.Instance.CurrentState}");
            }

            return canAccept;
        }

        private bool IsAcceptable(PropBase prop)
        {
            return prop.Type == _acceptedPropType && prop.CurrentState == _acceptedPropState;
        }

        private void AcceptProp(PropBase prop)
        {
            if (_currentProgress >= _requiredCount)
            {
                return;
            }

            _currentProgress = Mathf.Min(_currentProgress + 1, _requiredCount);

            if (_enableDebugLogs)
            {
                Debug.Log($"[UploadPort] Accepted: {prop.name} | Progress: {_currentProgress}/{_requiredCount}");
            }

            Destroy(prop.gameObject);

            if (_currentProgress >= _requiredCount)
            {
                NotifyObjectiveComplete();
            }
        }

        private void NotifyObjectiveComplete()
        {
            if (_enableDebugLogs)
            {
                Debug.Log($"[UploadPort] Objective Complete! Triggering Cleared.");
            }

            if (GameplayLoopController.Instance == null)
            {
                Debug.LogWarning("[UploadPort] GameplayLoopController.Instance is null!");
                return;
            }

            if (!GameplayLoopController.Instance.CanTransitionToCleared())
            {
                if (_enableDebugLogs)
                {
                    Debug.LogWarning($"[UploadPort] Cannot trigger Cleared: current state is {GameplayLoopController.Instance.CurrentState}");
                }
                return;
            }

            GameplayLoopController.Instance.TriggerCleared();
        }

        public void ResetProgress()
        {
            _currentProgress = 0;
            if (_enableDebugLogs)
            {
                Debug.Log("[UploadPort] Progress reset.");
            }
        }
    }
}
