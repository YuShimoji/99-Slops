using UnityEngine;

namespace GlitchWorker.Props
{
    public class AIProp : PropBase
    {
        [Header("AI Prop Settings")]
        [SerializeField] private Material _dormantMaterial;
        [SerializeField] private Material _hostileMaterial;
        [SerializeField] private Material _normalizedMaterial;
        [SerializeField] private float _normalizeImpactThreshold = 5f;
        [SerializeField] private float _hostileScaleFrequency = 3f;
        [SerializeField] private float _hostileScaleAmplitude = 0.1f;

        private Renderer _renderer;
        private Vector3 _originalScale;

        protected override void Awake()
        {
            base.Awake();
            _renderer = GetComponent<Renderer>();
            _originalScale = transform.localScale;
        }

        private void Update()
        {
            if (CurrentState == PropState.Hostile)
            {
                AnimateHostile();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CurrentState != PropState.Hostile) return;

            // Normalize when hit hard enough by a thrown object
            if (collision.relativeVelocity.magnitude >= _normalizeImpactThreshold)
            {
                SetState(PropState.Normalized);
            }
        }

        protected override void OnStateChanged(PropState previousState, PropState newState)
        {
            switch (newState)
            {
                case PropState.Dormant:
                    ApplyMaterial(_dormantMaterial);
                    Rb.isKinematic = true;
                    transform.localScale = _originalScale;
                    break;

                case PropState.Hostile:
                    ApplyMaterial(_hostileMaterial);
                    Rb.isKinematic = false;
                    break;

                case PropState.Normalized:
                    ApplyMaterial(_normalizedMaterial);
                    Rb.isKinematic = true;
                    transform.localScale = _originalScale;
                    break;
            }
        }

        private void AnimateHostile()
        {
            float t = Time.time * _hostileScaleFrequency;
            float scaleOffset = Mathf.Sin(t) * _hostileScaleAmplitude;
            transform.localScale = _originalScale * (1f + scaleOffset);
        }

        private void ApplyMaterial(Material mat)
        {
            if (_renderer != null && mat != null)
            {
                _renderer.material = mat;
            }
        }
    }
}
