using UnityEngine;

namespace IronExiles.Combat
{
    public enum ShieldFacing
    {
        Front = 0,
        Rear = 1,
        Port = 2,
        Starboard = 3
    }

    public static class ShieldMath
    {
        public static ShieldFacing DetermineFacing(Vector3 localDirection)
        {
            var forward = Vector3.Dot(localDirection.normalized, Vector3.forward);
            var right = Vector3.Dot(localDirection.normalized, Vector3.right);

            if (Mathf.Abs(forward) >= Mathf.Abs(right))
            {
                return forward >= 0f ? ShieldFacing.Front : ShieldFacing.Rear;
            }

            return right >= 0f ? ShieldFacing.Starboard : ShieldFacing.Port;
        }

        public static (float absorbed, float overflow) ComputeAbsorption(float shieldHp, float damage)
        {
            if (shieldHp <= 0f || damage <= 0f)
            {
                return (0f, Mathf.Max(0f, damage));
            }

            var absorbed = Mathf.Min(shieldHp, damage);
            var overflow = Mathf.Max(0f, damage - shieldHp);
            return (absorbed, overflow);
        }

        public static float ComputeRegenPerTick(float baseRate, float shieldPowerFraction, float deltaTime)
        {
            if (baseRate <= 0f || deltaTime <= 0f)
            {
                return 0f;
            }

            var multiplier = GetPowerMultiplier(shieldPowerFraction);
            return baseRate * multiplier * deltaTime;
        }

        public static float GetPowerMultiplier(float shieldPowerFraction)
        {
            var clamped = Mathf.Clamp01(shieldPowerFraction);
            return Mathf.Lerp(0.5f, ShieldSettings.DefaultPowerMultiplierMax, clamped);
        }
    }
}
