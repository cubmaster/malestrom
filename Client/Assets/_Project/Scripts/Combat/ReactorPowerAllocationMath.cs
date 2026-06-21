using UnityEngine;

namespace IronExiles.Combat
{
    public static class ReactorPowerAllocationSettings
    {
        public const float SumTolerance = 0.01f;
        public const float MinEnginePerformanceMultiplier = 0.5f;
        public const float MaxEnginePerformanceMultiplier = 1f;
    }

    public static class ReactorPowerAllocationMath
    {
        public enum PowerChannel
        {
            Weapons = 0,
            Shields = 1,
            Engines = 2,
            Ecm = 3
        }

        public static readonly PowerAllocation CombatPreset = new PowerAllocation
        {
            Weapons = 0.50f,
            Shields = 0.30f,
            Engines = 0.12f,
            Ecm = 0.08f
        };

        public static readonly PowerAllocation TravelPreset = new PowerAllocation
        {
            Weapons = 0.10f,
            Shields = 0.10f,
            Engines = 0.70f,
            Ecm = 0.10f
        };

        public static readonly PowerAllocation BalancedPreset = new PowerAllocation
        {
            Weapons = 0.25f,
            Shields = 0.25f,
            Engines = 0.25f,
            Ecm = 0.25f
        };

        public static readonly PowerAllocation MaxEnginesPreset = new PowerAllocation
        {
            Weapons = 0f,
            Shields = 0f,
            Engines = 1f,
            Ecm = 0f
        };

        public static readonly PowerAllocation MaxWeaponsPreset = new PowerAllocation
        {
            Weapons = 1f,
            Shields = 0f,
            Engines = 0f,
            Ecm = 0f
        };

        public static bool IsValid(PowerAllocation allocation)
        {
            if (allocation.Weapons < 0f || allocation.Shields < 0f || allocation.Engines < 0f || allocation.Ecm < 0f)
            {
                return false;
            }

            var sum = allocation.Weapons + allocation.Shields + allocation.Engines + allocation.Ecm;
            return Mathf.Abs(sum - 1f) <= ReactorPowerAllocationSettings.SumTolerance;
        }

        public static bool TryCreate(float weapons, float shields, float engines, float ecm, out PowerAllocation allocation)
        {
            allocation = new PowerAllocation
            {
                Weapons = weapons,
                Shields = shields,
                Engines = engines,
                Ecm = ecm
            };

            return IsValid(allocation);
        }

        public static float GetEnginePerformanceMultiplier(float enginesFraction)
        {
            var clamped = Mathf.Clamp01(enginesFraction);
            return Mathf.Lerp(
                ReactorPowerAllocationSettings.MinEnginePerformanceMultiplier,
                ReactorPowerAllocationSettings.MaxEnginePerformanceMultiplier,
                clamped);
        }

        public static float GetEnginePerformanceMultiplier(PowerAllocation allocation) =>
            GetEnginePerformanceMultiplier(allocation.Engines);

        public static PowerAllocation AdjustChannel(PowerAllocation current, PowerChannel channel, float newChannelValue)
        {
            newChannelValue = Mathf.Clamp01(newChannelValue);

            var values = new[]
            {
                current.Weapons,
                current.Shields,
                current.Engines,
                current.Ecm
            };

            var channelIndex = (int)channel;
            var remaining = 1f - newChannelValue;
            var otherSum = 0f;
            for (var i = 0; i < values.Length; i++)
            {
                if (i == channelIndex)
                {
                    continue;
                }

                otherSum += values[i];
            }

            values[channelIndex] = newChannelValue;

            if (remaining <= ReactorPowerAllocationSettings.SumTolerance)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (i != channelIndex)
                    {
                        values[i] = 0f;
                    }
                }
            }
            else if (otherSum <= ReactorPowerAllocationSettings.SumTolerance)
            {
                var evenShare = remaining / (values.Length - 1);
                for (var i = 0; i < values.Length; i++)
                {
                    if (i != channelIndex)
                    {
                        values[i] = evenShare;
                    }
                }
            }
            else
            {
                var scale = remaining / otherSum;
                for (var i = 0; i < values.Length; i++)
                {
                    if (i != channelIndex)
                    {
                        values[i] *= scale;
                    }
                }
            }

            return new PowerAllocation
            {
                Weapons = values[0],
                Shields = values[1],
                Engines = values[2],
                Ecm = values[3]
            };
        }
    }
}
