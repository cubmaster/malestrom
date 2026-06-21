using System;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    public struct ReactorPowerNetworkState : INetworkSerializable, IEquatable<ReactorPowerNetworkState>
    {
        public float Weapons;
        public float Shields;
        public float Engines;
        public float Ecm;

        public static ReactorPowerNetworkState From(PowerAllocation allocation) =>
            new ReactorPowerNetworkState
            {
                Weapons = allocation.Weapons,
                Shields = allocation.Shields,
                Engines = allocation.Engines,
                Ecm = allocation.Ecm
            };

        public PowerAllocation ToPowerAllocation() =>
            new PowerAllocation
            {
                Weapons = Weapons,
                Shields = Shields,
                Engines = Engines,
                Ecm = Ecm
            };

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Weapons);
            serializer.SerializeValue(ref Shields);
            serializer.SerializeValue(ref Engines);
            serializer.SerializeValue(ref Ecm);
        }

        public bool Equals(ReactorPowerNetworkState other) =>
            Mathf.Approximately(Weapons, other.Weapons)
            && Mathf.Approximately(Shields, other.Shields)
            && Mathf.Approximately(Engines, other.Engines)
            && Mathf.Approximately(Ecm, other.Ecm);
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class NetworkShipReactorPowerController : NetworkBehaviour, IShipReactorPowerControl
    {
        readonly NetworkVariable<ReactorPowerNetworkState> _allocation = new(
            ReactorPowerNetworkState.From(ReactorPowerAllocationMath.CombatPreset),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        ShipMovementController _movement;
        NetworkShipMovementController _networkMovement;

        public PowerAllocation Current => _allocation.Value.ToPowerAllocation();
        public event Action<PowerAllocation> AllocationChanged;

        void Awake()
        {
            _movement = GetComponent<ShipMovementController>();
            _networkMovement = GetComponent<NetworkShipMovementController>();
            _allocation.OnValueChanged += OnAllocationChanged;
        }

        void OnDestroy()
        {
            _allocation.OnValueChanged -= OnAllocationChanged;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ApplyEngineMultiplier(Current);
        }

        public void RequestAllocation(PowerAllocation allocation)
        {
            if (!ReactorPowerAllocationMath.IsValid(allocation))
            {
                return;
            }

            if (!IsSpawned)
            {
                ApplyLocalAllocation(allocation);
                return;
            }

            if (!IsOwner)
            {
                return;
            }

            SetAllocationServerRpc(
                allocation.Weapons,
                allocation.Shields,
                allocation.Engines,
                allocation.Ecm);
        }

        [ServerRpc]
        void SetAllocationServerRpc(
            float weapons,
            float shields,
            float engines,
            float ecm,
            ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            if (!ReactorPowerAllocationMath.TryCreate(weapons, shields, engines, ecm, out var allocation))
            {
                return;
            }

            _allocation.Value = ReactorPowerNetworkState.From(allocation);
        }

        void OnAllocationChanged(ReactorPowerNetworkState previous, ReactorPowerNetworkState next)
        {
            ApplyEngineMultiplier(next.ToPowerAllocation());
            AllocationChanged?.Invoke(next.ToPowerAllocation());
        }

        void ApplyLocalAllocation(PowerAllocation allocation)
        {
            ApplyEngineMultiplier(allocation);
            AllocationChanged?.Invoke(allocation);
        }

        void ApplyEngineMultiplier(PowerAllocation allocation)
        {
            var multiplier = ReactorPowerAllocationMath.GetEnginePerformanceMultiplier(allocation);
            _movement?.Model.SetEnginePerformanceMultiplier(multiplier);
            _networkMovement?.SetEnginePerformanceMultiplier(multiplier);
        }
    }
}
