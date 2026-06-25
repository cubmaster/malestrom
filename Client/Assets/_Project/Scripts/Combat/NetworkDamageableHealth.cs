using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    public static class DamageableHealthMath
    {
        public static float ApplyDamage(float currentHull, float damageAmount, out bool destroyed)
        {
            var next = Mathf.Max(0f, currentHull - Mathf.Max(0f, damageAmount));
            destroyed = next <= 0f;
            return next;
        }

        public static float ToHullPercent(float currentHull, float maxHull)
        {
            if (maxHull <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(currentHull / maxHull) * 100f;
        }
    }

    [DisallowMultipleComponent]
    public sealed class NetworkDamageableHealth : NetworkBehaviour
    {
        readonly NetworkVariable<float> _currentHull = new(
            BeamWeaponSettings.DefaultMaxHull,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        readonly NetworkVariable<float> _maxHull = new(
            BeamWeaponSettings.DefaultMaxHull,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        TargetableEntity _targetable;

        public float CurrentHull => _currentHull.Value;
        public float MaxHull => _maxHull.Value;
        public float HullPercent => DamageableHealthMath.ToHullPercent(_currentHull.Value, _maxHull.Value);
        public bool IsDestroyed => _currentHull.Value <= 0f;

        void Awake()
        {
            _targetable = GetComponent<TargetableEntity>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SyncTargetableDisplay();
        }

        public void ConfigureForServer(float maxHull, float currentHull = -1f)
        {
            if (!IsServer)
            {
                return;
            }

            var clampedMax = Mathf.Max(1f, maxHull);
            _maxHull.Value = clampedMax;
            _currentHull.Value = currentHull < 0f ? clampedMax : Mathf.Clamp(currentHull, 0f, clampedMax);
            SyncTargetableDisplay();
        }

        public void ApplyDamage(float amount)
        {
            if (!IsServer || amount <= 0f || IsDestroyed)
            {
                return;
            }

            _currentHull.Value = DamageableHealthMath.ApplyDamage(_currentHull.Value, amount, out _);
            SyncTargetableDisplay();
        }

        void SyncTargetableDisplay()
        {
            if (_targetable != null)
            {
                _targetable.SetHullPercent(HullPercent);
            }
        }
    }
}
