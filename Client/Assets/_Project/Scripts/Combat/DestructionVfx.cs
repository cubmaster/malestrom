using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Client-side destruction VFX. Listens to hull value changes on the
    /// NetworkDamageableHealth component and spawns an explosion particle burst
    /// when the hull transitions to zero or below.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkDamageableHealth))]
    public sealed class DestructionVfx : MonoBehaviour
    {
        const float ParticleLifetime = 1.5f;
        const float StartSizeMin = 3f;
        const float StartSizeMax = 8f;
        const int BurstCount = 30;

        NetworkDamageableHealth _health;
        bool _hasPlayedExplosion;

        void Awake()
        {
            _health = GetComponent<NetworkDamageableHealth>();
        }

        void OnEnable()
        {
            _hasPlayedExplosion = false;
            if (_health != null)
            {
                _health.CurrentHullNetVar.OnValueChanged += OnHullChanged;
            }
        }

        void OnDisable()
        {
            if (_health != null)
            {
                _health.CurrentHullNetVar.OnValueChanged -= OnHullChanged;
            }
        }

        void OnHullChanged(float previousValue, float newValue)
        {
            if (newValue <= 0f && previousValue > 0f && !_hasPlayedExplosion)
            {
                _hasPlayedExplosion = true;
                SpawnExplosion();
            }
        }

        void SpawnExplosion()
        {
            var explosionGo = new GameObject("DestructionExplosion");
            explosionGo.transform.position = transform.position;

            var ps = explosionGo.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = ParticleLifetime;
            main.loop = false;
            main.startLifetime = ParticleLifetime;
            main.startSize = new ParticleSystem.MinMaxCurve(StartSizeMin, StartSizeMax);
            main.startSpeed = 5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = true;
            main.stopAction = ParticleSystemStopAction.Destroy;

            // Color gradient: orange -> red -> transparent
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(new Color(1f, 0.6f, 0f), 0f),    // Orange
                    new GradientColorKey(new Color(1f, 0.1f, 0f), 0.5f),  // Red
                    new GradientColorKey(new Color(0.5f, 0f, 0f), 1f)     // Dark red
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            // Sphere shape
            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.5f;

            // Burst emission
            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, BurstCount) });

            // Use default particle material
            var renderer = explosionGo.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            }

            ps.Play();
        }
    }
}
