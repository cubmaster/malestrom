using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class DamageableHull : MonoBehaviour
    {
        float _currentHull;
        float _maxHull;
        TargetableEntity _targetable;

        public float CurrentHull => _currentHull;
        public float MaxHull => _maxHull;
        public float HullPercent => _maxHull > 0f ? (_currentHull / _maxHull) * 100f : 0f;
        public bool IsDestroyed => _currentHull <= 0f;

        public void Configure(float maxHull)
        {
            _maxHull = Mathf.Max(1f, maxHull);
            _currentHull = _maxHull;
            _targetable = GetComponent<TargetableEntity>();
        }

        public void ApplyDamage(float amount)
        {
            if (amount <= 0f || IsDestroyed)
            {
                return;
            }

            _currentHull = Mathf.Max(0f, _currentHull - amount);
            Debug.Log($"[DamageableHull] {gameObject.name} took {amount:F1} damage. Hull: {_currentHull:F0}/{_maxHull:F0} ({HullPercent:F1}%)");

            if (_targetable != null)
            {
                _targetable.SetHullPercent(HullPercent);
            }
            else
            {
                Debug.LogWarning($"[DamageableHull] {gameObject.name} has no TargetableEntity reference!");
            }

            if (IsDestroyed)
            {
                SpawnExplosion();
                Destroy(gameObject);
            }
        }
        void SpawnExplosion()
        {
            var explosion = new GameObject("Explosion");
            explosion.transform.position = transform.position;

            var ps = explosion.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 1.5f;
            main.startLifetime = 1.2f;
            main.startSpeed = 8f;
            main.startSize = 2f;
            main.startColor = new ParticleSystem.MinMaxGradient(
                new Color(1f, 0.6f, 0.1f, 1f),
                new Color(1f, 0.2f, 0.05f, 1f));
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Destroy;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 30) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 1f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));
        }
    }
}
