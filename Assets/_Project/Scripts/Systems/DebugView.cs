using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GlitchWorker.Systems
{
    public class DebugView : MonoBehaviour
    {
        [Header("Debug View Settings")]
        [SerializeField] private Volume _debugVolume;
        [SerializeField] private Material _highlightMaterialAI;
        [SerializeField] private Material _highlightMaterialHuman;

        private bool _isActive;

        public bool IsActive => _isActive;

        public void OnSprint(InputValue value)
        {
            // Repurpose Sprint (Left Shift) as Debug View toggle for prototype
            if (value.isPressed)
            {
                ToggleDebugView();
            }
        }

        public void ToggleDebugView()
        {
            _isActive = !_isActive;

            if (_debugVolume != null)
            {
                _debugVolume.weight = _isActive ? 1f : 0f;
            }

            GameEventBus.RaiseDebugViewToggled(_isActive);
        }
    }
}
