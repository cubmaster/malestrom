using UnityEngine;

namespace IronExiles.Combat
{
    public static class BeamWeaponControllerLogic
    {
        public static bool TryApplyTickDamage(
            bool isFiring,
            ulong lockedTargetNetworkObjectId,
            Vector3 origin,
            Vector3 targetPosition,
            float fireRangeMeters,
            float baseDps,
            float weaponsFraction,
            float deltaTimeSeconds,
            ref float targetCurrentHull,
            out float damageApplied)
        {
            damageApplied = 0f;

            if (!isFiring)
            {
                return false;
            }

            if (!BeamWeaponMath.CanFireAtTarget(lockedTargetNetworkObjectId, origin, targetPosition, fireRangeMeters))
            {
                return false;
            }

            var effectiveDps = BeamWeaponMath.ComputeEffectiveDps(baseDps, weaponsFraction);
            damageApplied = BeamWeaponMath.ComputeTickDamage(effectiveDps, deltaTimeSeconds);
            if (damageApplied <= 0f)
            {
                return false;
            }

            targetCurrentHull = DamageableHealthMath.ApplyDamage(targetCurrentHull, damageApplied, out _);
            return true;
        }

        public static bool TryApplyTickDamageWithShields(
            bool isFiring,
            ulong lockedTargetNetworkObjectId,
            Vector3 origin,
            Vector3 targetPosition,
            float fireRangeMeters,
            float baseDps,
            float weaponsFraction,
            float deltaTimeSeconds,
            ref float shieldFacingHp,
            ref float targetCurrentHull,
            out float damageApplied,
            out float shieldAbsorbed)
        {
            damageApplied = 0f;
            shieldAbsorbed = 0f;

            if (!isFiring)
            {
                return false;
            }

            if (!BeamWeaponMath.CanFireAtTarget(lockedTargetNetworkObjectId, origin, targetPosition, fireRangeMeters))
            {
                return false;
            }

            var effectiveDps = BeamWeaponMath.ComputeEffectiveDps(baseDps, weaponsFraction);
            damageApplied = BeamWeaponMath.ComputeTickDamage(effectiveDps, deltaTimeSeconds);
            if (damageApplied <= 0f)
            {
                return false;
            }

            var (absorbed, overflow) = ShieldMath.ComputeAbsorption(shieldFacingHp, damageApplied);
            shieldAbsorbed = absorbed;
            shieldFacingHp -= absorbed;

            if (overflow > 0f)
            {
                targetCurrentHull = DamageableHealthMath.ApplyDamage(targetCurrentHull, overflow, out _);
            }

            return true;
        }

        public static float SimulateSustainedFire(
            float baseDps,
            float weaponsFraction,
            float durationSeconds,
            float tickSeconds,
            float initialHull)
        {
            var hull = initialHull;
            var elapsed = 0f;
            while (elapsed < durationSeconds && hull > 0f)
            {
                var step = Mathf.Min(tickSeconds, durationSeconds - elapsed);
                TryApplyTickDamage(
                    true,
                    1UL,
                    Vector3.zero,
                    Vector3.forward * 100f,
                    2500f,
                    baseDps,
                    weaponsFraction,
                    step,
                    ref hull,
                    out _);
                elapsed += step;
            }

            return hull;
        }
    }
}
