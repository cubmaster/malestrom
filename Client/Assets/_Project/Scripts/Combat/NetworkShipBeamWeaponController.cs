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
            var lockedTarget = _targeting.GetLockedTarget();
            if (lockedTarget == null)
            {
                SetFiringState(false);
                return;
            }

            var fireRange = BeamWeaponMath.ResolveFireRangeMeters(definition, _targeting.LockRangeMeters);
            if (!BeamWeaponMath.CanFireAtTarget(
                    _targeting.LockedTargetNetworkObjectId,
                    transform.position,
                    lockedTarget.transform.position,
                    fireRange))
            {
                SetFiringState(false);
                return;
            }

            var weaponsFraction = _reactorPower != null ? _reactorPower.Current.Weapons : 1f;
            var effectiveDps = BeamWeaponMath.ComputeEffectiveDps(definition, weaponsFraction);
            var damage = BeamWeaponMath.ComputeTickDamage(effectiveDps, deltaTimeSeconds);
            if (damage <= 0f)
            {
                return;
            }

            var targetTransform = lockedTarget.transform;
            var hullDamage = damage;

            var shieldController = lockedTarget.GetComponent<NetworkShipShieldController>();
            if (shieldController != null)
            {
                var worldAttackDirection = (transform.position - targetTransform.position).normalized;
                hullDamage = shieldController.ApplyDirectionalDamage(worldAttackDirection, damage);
            }

            if (hullDamage > 0f)
            {
                var damageable = lockedTarget.GetComponent<NetworkDamageableHealth>();
                damageable?.ApplyDamage(hullDamage);
            }
        }
    }
}
