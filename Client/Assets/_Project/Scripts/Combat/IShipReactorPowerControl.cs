using System;

namespace IronExiles.Combat
{
    public interface IShipReactorPowerControl
    {
        PowerAllocation Current { get; }
        event Action<PowerAllocation> AllocationChanged;
        void RequestAllocation(PowerAllocation allocation);
    }
}
