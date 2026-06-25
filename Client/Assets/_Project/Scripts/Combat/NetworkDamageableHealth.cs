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
        float _localCurrentHull = BeamWeaponSettings.DefaultMaxHull;
        float _localMaxHull = BeamWeaponSettings.DefaultMaxHull;

        public float CurrentHull => IsSpawned ? _currentHull.Value : _localCurrentHull;
        public float MaxHull => IsSpawned ? _maxHull.Value : _localMaxHull;
        public float HullPercent => DamageableHealthMath.ToHullPercent(CurrentHull, MaxHull);
        public bool IsDestroyed => CurrentHull <= 0f;

        void Awake()
        {
            _targetable = GetComponent<TargetableEntity>();
            _localCurrentHull = BeamWeaponSettings.DefaultMaxHull;
            _localMaxHull = BeamWeaponSettings.DefaultMaxHull;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SyncTargetableDisplay();
        }

        public void ConfigureForServer(float maxHull, float currentHull = -1f)
        {
            if (IsSpawned && !IsServer)
            {
                return;
            }

            var clampedMax = Mathf.Max(1f, maxHull);
            if (IsSpawned)
            {
                _maxHull.Value = clampedMax;
                _currentHull.Value = currentHull < 0f ? clampedMax : Mathf.Clamp(currentHull, 0f, clampedMax);
            }
            else
            {
                _localMaxHull = clampedMax;
                _localCurrentHull = currentHull < 0f ? clampedMax : Mathf.Clamp(currentHull, 0f, clampedMax);
            }
            SyncTargetableDisplay();
        }

        public void ApplyDamage(float amount)
        {
            if (IsSpawned && (!IsServer || IsDestroyed))
            {
                return;
            }
            if (amount <= 0f || IsDestroyed)
            {
                return;
            }

            if (IsSpawned)
            {
                _currentHull.Value = DamageableHealthMath.ApplyDamage(_currentHull.Value, amount, out _);
            }
            else
            {
                _localCurrentHull = DamageableHealthMath.ApplyDamage(_localCurrentHull, amount, out _);
            }
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
