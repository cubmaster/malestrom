using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkShipTargetingController))]
    public sealed class NetworkShipBeamWeaponController : NetworkBehaviour
    {
        [SerializeField] BeamWeaponDefinition _beamDefinition;

        readonly NetworkVariable<bool> _isFiring = new(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        NetworkShipTargetingController _targeting;
        IShipReactorPowerControl _reactorPower;
        bool _localIsFiring;

        public bool IsFiring => IsSpawned ? _isFiring.Value : _localIsFiring;
        public BeamWeaponDefinition BeamDefinition => ResolveBeamDefinition();

        void Awake()
        {
            _targeting = GetComponent<NetworkShipTargetingController>();
            _reactorPower = (IShipReactorPowerControl)GetComponent<NetworkShipReactorPowerController>() ?? GetComponent<ShipReactorPowerController>();
        }

        BeamWeaponDefinition ResolveBeamDefinition()
        {
            if (_beamDefinition != null)
            {
                return _beamDefinition;
            }

            _beamDefinition = ScriptableObject.CreateInstance<BeamWeaponDefinition>();
            return _beamDefinition;
        }

        private void SetFiringState(bool firing)
        {
            if (IsSpawned)
            {
                _isFiring.Value = firing;
            }
            else
            {
                _localIsFiring = firing;
            }
        }

        public void SetFiringOffline(bool firing)
        {
            if (IsSpawned) return;
            SetFiringState(firing);
        }

        [ServerRpc]
        public void SetFiringServerRpc(bool firing, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            _isFiring.Value = firing;
        }

        void Update()
        {
            if (IsSpawned && (!IsServer || !_isFiring.Value))
            {
                return;
            }
            if (!IsSpawned && !_localIsFiring)
            {
                return;
            }

            ApplyServerTickDamage(Time.deltaTime);
        }

        void ApplyServerTickDamage(float deltaTimeSeconds)
        {
            var definition = ResolveBeamDefinition();
            var weaponsFraction = _reactorPower != null ? _reactorPower.Current.Weapons : 1f;
            var effectiveDps = BeamWeaponMath.ComputeEffectiveDps(definition, weaponsFraction);
            var damage = BeamWeaponMath.ComputeTickDamage(effectiveDps, deltaTimeSeconds);
            if (damage <= 0f)
            {
                return;
            }

            var fireRange = BeamWeaponMath.ResolveFireRangeMeters(definition, _targeting.LockRangeMeters);

            var lockedTarget = _targeting.GetLockedTarget();
            if (lockedTarget != null)
            {
                if (!BeamWeaponMath.CanFireAtTarget(
                        _targeting.LockedTargetNetworkObjectId,
                        transform.position,
                        lockedTarget.transform.position,
                        fireRange))
                {
                    SetFiringState(false);
                    return;
                }

                ApplyDamageToTarget(lockedTarget, damage);
                return;
            }

            var hit = RaycastForTarget(fireRange);
            if (hit != null)
            {
                ApplyDamageToTarget(hit, damage);
            }
        }

        void ApplyDamageToTarget(TargetableEntity target, float damage)
        {
            var hullDamage = damage;

            var shieldController = target.GetComponent<NetworkShipShieldController>();
            if (shieldController != null)
            {
                var worldAttackDirection = (transform.position - target.transform.position).normalized;
                hullDamage = shieldController.ApplyDirectionalDamage(worldAttackDirection, damage);
            }

            if (hullDamage <= 0f)
            {
                return;
            }

            var damageable = target.GetComponent<NetworkDamageableHealth>();
            if (damageable != null)
            {
                damageable.ApplyDamage(hullDamage);
                return;
            }

            var hull = target.GetComponent<DamageableHull>();
            if (hull != null)
            {
                hull.ApplyDamage(hullDamage);
            }
            else
            {
                Debug.LogWarning($"[BeamWeapon] Target '{target.DisplayName}' has no damage receiver (no NetworkDamageableHealth or DamageableHull).");
            }
        }

        TargetableEntity RaycastForTarget(float range)
        {
            if (!Physics.Raycast(transform.position, transform.forward, out var hit, range))
            {
                return null;
            }

            return hit.collider.GetComponent<TargetableEntity>();
        }
    }
}
