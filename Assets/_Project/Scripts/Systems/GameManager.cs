using UnityEngine;

namespace GlitchWorker.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private GameplayLoopController _gameplayLoopController;

        public GameplayLoopController GameplayLoop => _gameplayLoopController;

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
    }
}
