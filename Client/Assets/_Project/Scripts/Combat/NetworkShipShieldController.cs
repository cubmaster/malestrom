using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class NetworkShipShieldController : NetworkBehaviour
    {
        readonly NetworkVariable<ShieldNetworkState> _shieldState = new(
            ShieldNetworkState.Full(ShieldSettings.DefaultMaxShieldPerFacing),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        float _maxShieldPerFacing = ShieldSettings.DefaultMaxShieldPerFacing;
        readonly float[] _lastDamageTime = new float[4];

        IShipReactorPowerControl _reactorPower;

        public ShieldNetworkState Current => _shieldState.Value;
        public float MaxShieldPerFacing => _maxShieldPerFacing;

        public float GetFacingPercent(ShieldFacing facing)
        {
            if (_maxShieldPerFacing <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(_shieldState.Value[facing] / _maxShieldPerFacing) * 100f;
        }

        void Awake()
        {
            _reactorPower = GetComponent<IShipReactorPowerControl>();
        }

        public void ConfigureForServer(float maxPerFacing)
        {
            _maxShieldPerFacing = Mathf.Max(1f, maxPerFacing);

            if (IsSpawned && IsServer)
            {
                _shieldState.Value = ShieldNetworkState.Full(_maxShieldPerFacing);
            }

            for (var i = 0; i < _lastDamageTime.Length; i++)
            {
                _lastDamageTime[i] = -ShieldSettings.RegenCooldownSeconds;
            }
        }

        public float ApplyDirectionalDamage(Vector3 worldAttackDirection, float damage)
        {
            if (damage <= 0f)
            {
                return 0f;
            }

            var localDirection = transform.InverseTransformDirection(worldAttackDirection);
            var facing = ShieldMath.DetermineFacing(localDirection);

            var state = _shieldState.Value;
            var currentHp = state[facing];

            var (absorbed, overflow) = ShieldMath.ComputeAbsorption(currentHp, damage);

            state[facing] = currentHp - absorbed;

            if (IsSpawned && IsServer)
            {
                _shieldState.Value = state;
            }

            _lastDamageTime[(int)facing] = Time.time;

            if (CompareTag("Player"))
            {
                var attackerPos = transform.position + worldAttackDirection * 100f;
                LocalPlayerSystemsEvents.NotifyLocalPlayerHit(attackerPos);
            }

            return overflow;
        }

        void Update()
        {
            if (!IsSpawned || !IsServer)
            {
                return;
            }

            var shieldPowerFraction = _reactorPower != null ? _reactorPower.Current.Shields : 0f;
            var state = _shieldState.Value;
            var changed = false;

            for (var i = 0; i < 4; i++)
            {
                var facing = (ShieldFacing)i;
                var currentHp = state[facing];

                if (currentHp >= _maxShieldPerFacing)
                {
                    continue;
                }

                if (Time.time - _lastDamageTime[i] < ShieldSettings.RegenCooldownSeconds)
                {
                    continue;
                }

                var regen = ShieldMath.ComputeRegenPerTick(
                    ShieldSettings.DefaultRegenRatePerSecond,
                    shieldPowerFraction,
                    Time.deltaTime);

                state[facing] = Mathf.Min(currentHp + regen, _maxShieldPerFacing);
                changed = true;
            }

            if (changed)
            {
                _shieldState.Value = state;
            }
        }
    }
}
