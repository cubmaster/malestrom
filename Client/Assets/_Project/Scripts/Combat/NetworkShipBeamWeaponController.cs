using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkShipTargetingController))]
    [RequireComponent(typeof(NetworkShipReactorPowerController))]
    public sealed class NetworkShipBeamWeaponController : NetworkBehaviour
    {
        [SerializeField] BeamWeaponDefinition _beamDefinition;

        readonly NetworkVariable<bool> _isFiring = new(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        NetworkShipTargetingController _targeting;
        NetworkShipReactorPowerController _reactorPower;

        public bool IsFiring => _isFiring.Value;
        public BeamWeaponDefinition BeamDefinition => ResolveBeamDefinition();

        void Awake()
        {
            _targeting = GetComponent<NetworkShipTargetingController>();
            _reactorPower = GetComponent<NetworkShipReactorPowerController>();
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
            if (!IsServer || !_isFiring.Value)
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
                _isFiring.Value = false;
                return;
            }

            var fireRange = BeamWeaponMath.ResolveFireRangeMeters(definition, _targeting.LockRangeMeters);
            if (!BeamWeaponMath.CanFireAtTarget(
                    _targeting.LockedTargetNetworkObjectId,
                    transform.position,
                    lockedTarget.transform.position,
                    fireRange))
            {
                _isFiring.Value = false;
                return;
            }

            var weaponsFraction = _reactorPower != null ? _reactorPower.Current.Weapons : 1f;
            var effectiveDps = BeamWeaponMath.ComputeEffectiveDps(definition, weaponsFraction);
            var damage = BeamWeaponMath.ComputeTickDamage(effectiveDps, deltaTimeSeconds);
            if (damage <= 0f)
            {
                return;
            }

            var damageable = lockedTarget.GetComponent<NetworkDamageableHealth>();
            damageable?.ApplyDamage(damage);
        }
    }
}
