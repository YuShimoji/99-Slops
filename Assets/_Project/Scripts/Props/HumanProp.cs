using UnityEngine;

namespace GlitchWorker.Props
{
    public class HumanProp : PropBase
    {
        [Header("Human Prop Settings")]
        [SerializeField] private int _componentSlots = 3;
        [SerializeField] private float _value = 100f;

        public int ComponentSlots => _componentSlots;
        public float Value => _value;

        protected override void OnStateChanged(PropState previousState, PropState newState)
        {
            // Human Props are always stable - no visual state change needed
        }

        public void ReduceValue(float amount)
        {
            _value = Mathf.Max(0f, _value - amount);
        }
    }
}
