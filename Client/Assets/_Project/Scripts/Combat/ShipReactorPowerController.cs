using System;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class ShipReactorPowerController : MonoBehaviour, IShipReactorPowerControl
    {
        [SerializeField] PowerAllocation _allocation = ReactorPowerAllocationMath.CombatPreset;

        ShipMovementController _movement;

        public PowerAllocation Current => _allocation;
        public event Action<PowerAllocation> AllocationChanged;

        void Awake()
        {
            _movement = GetComponent<ShipMovementController>();
            ApplyAllocation(_allocation, notify: false);
        }

        public void RequestAllocation(PowerAllocation allocation)
        {
            if (!ReactorPowerAllocationMath.IsValid(allocation))
            {
                return;
            }

            ApplyAllocation(allocation, notify: true);
        }

        void ApplyAllocation(PowerAllocation allocation, bool notify)
        {
            _allocation = allocation;
            ApplyEngineMultiplier(allocation);
            if (notify)
            {
                AllocationChanged?.Invoke(allocation);
            }
        }

        void ApplyEngineMultiplier(PowerAllocation allocation)
        {
            if (_movement == null)
            {
                return;
            }

            var multiplier = ReactorPowerAllocationMath.GetEnginePerformanceMultiplier(allocation);
            _movement.Model.SetEnginePerformanceMultiplier(multiplier);
        }
    }
}
