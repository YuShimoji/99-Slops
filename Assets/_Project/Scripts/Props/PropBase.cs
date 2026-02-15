using UnityEngine;
using GlitchWorker.Systems;

namespace GlitchWorker.Props
{
    public enum PropType
    {
        AI,
        Human
    }

    public enum PropState
    {
        Dormant,
        Hostile,
        Normalized
    }

    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public abstract class PropBase : MonoBehaviour
    {
        [Header("Prop Info")]
        [SerializeField] private PropType _propType;
        [SerializeField] private PropState _currentState = PropState.Dormant;

        public PropType Type => _propType;
        public PropState CurrentState => _currentState;

        protected Rigidbody Rb { get; private set; }

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody>();
        }

        public virtual void SetState(PropState newState)
        {
            if (_currentState == newState) return;

            PropState previousState = _currentState;
            _currentState = newState;
            OnStateChanged(previousState, newState);
            GameEventBus.RaisePropStateChanged(this, previousState, newState);
        }

        protected abstract void OnStateChanged(PropState previousState, PropState newState);
    }
}
