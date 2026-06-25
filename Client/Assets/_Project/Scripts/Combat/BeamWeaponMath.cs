using UnityEngine;

namespace IronExiles.Combat
{
    public static class BeamWeaponMath
    {
        public static float ComputeEffectiveDps(float baseDps, float weaponsFraction)
        {
            if (baseDps <= 0f)
            {
                return 0f;
            }

            var multiplier = ReactorPowerAllocationMath.GetWeaponPerformanceMultiplier(weaponsFraction);
            return baseDps * multiplier;
        }

        public static float ComputeEffectiveDps(BeamWeaponDefinition definition, float weaponsFraction)
        {
            return definition == null ? 0f : ComputeEffectiveDps(definition.BaseDps, weaponsFraction);
        }

        public static float ComputeTickDamage(float effectiveDps, float deltaTimeSeconds)
        {
            if (effectiveDps <= 0f || deltaTimeSeconds <= 0f)
            {
                return 0f;
            }

            return effectiveDps * deltaTimeSeconds;
        }

        public static bool CanFireAtTarget(
            ulong lockedTargetNetworkObjectId,
            Vector3 origin,
            Vector3 targetPosition,
            float maxRangeMeters)
        {
            if (lockedTargetNetworkObjectId == 0UL)
            {
                return false;
            }

            if (maxRangeMeters <= 0f)
            {
                return false;
            }

            return TargetSelectionMath.IsWithinLockRange(origin, targetPosition, maxRangeMeters);
        }

        public static float ResolveFireRangeMeters(BeamWeaponDefinition definition, float lockRangeMeters)
        {
            if (definition == null)
            {
                return lockRangeMeters;
            }

            return Mathf.Min(definition.RangeMeters, lockRangeMeters);
        }
    }
}
