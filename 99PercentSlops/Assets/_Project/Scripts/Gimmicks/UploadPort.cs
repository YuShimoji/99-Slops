using UnityEngine;
using GlitchWorker.Props;
using GlitchWorker.Systems;

namespace GlitchWorker.Gimmicks
{
    public class UploadPort : MonoBehaviour
    {
        [Header("Objective Settings")]
        [SerializeField] private int _requiredCount = 3;
        [SerializeField] private PropType _acceptedPropType = PropType.AI;
        [SerializeField] private PropState _acceptedPropState = PropState.Normalized;

        [Header("Debug")]
        [SerializeField] private bool _enableDebugLogs = true;

        private int _currentProgress = 0;
        public int CurrentProgress => _currentProgress;
        public int RequiredCount => _requiredCount;

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

        private bool IsAcceptable(PropBase prop)
        {
            return prop.Type == _acceptedPropType && prop.CurrentState == _acceptedPropState;
        }

        private void AcceptProp(PropBase prop)
        {
            _currentProgress++;

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

            if (GameplayLoopController.Instance != null)
            {
                GameplayLoopController.Instance.TriggerCleared();
            }
            else
            {
                Debug.LogWarning("[UploadPort] GameplayLoopController.Instance is null!");
            }
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
