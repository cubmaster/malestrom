using IronExiles.Combat;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public sealed class BeamWeaponMathTests
    {
        [Test]
        public void GetWeaponPerformanceMultiplier_clamps_to_zero_and_one()
        {
            Assert.That(ReactorPowerAllocationMath.GetWeaponPerformanceMultiplier(-1f), Is.EqualTo(0f));
            Assert.That(ReactorPowerAllocationMath.GetWeaponPerformanceMultiplier(0f), Is.EqualTo(0f));
            Assert.That(ReactorPowerAllocationMath.GetWeaponPerformanceMultiplier(1f), Is.EqualTo(1f));
            Assert.That(ReactorPowerAllocationMath.GetWeaponPerformanceMultiplier(2f), Is.EqualTo(1f));
        }

        [Test]
        public void EffectiveDps_at_full_weapons_is_at_least_double_half_weapons()
        {
            var definition = ScriptableObject.CreateInstance<BeamWeaponDefinition>();
            var full = BeamWeaponMath.ComputeEffectiveDps(definition, 1f);
            var half = BeamWeaponMath.ComputeEffectiveDps(definition, 0.5f);

            Assert.That(full, Is.GreaterThanOrEqualTo(half * 2f));
            Assert.That(full, Is.EqualTo(BeamWeaponSettings.DefaultTier1BaseDps).Within(0.01f));
        }

        [Test]
        public void ComputeTickDamage_multiplies_dps_by_delta_time()
        {
            var damage = BeamWeaponMath.ComputeTickDamage(50f, 0.1f);
            Assert.That(damage, Is.EqualTo(5f).Within(0.001f));
        }

        [Test]
        public void CanFireAtTarget_requires_lock_and_range()
        {
            var origin = Vector3.zero;
            var inRange = new Vector3(0f, 0f, 100f);

            Assert.That(BeamWeaponMath.CanFireAtTarget(0UL, origin, inRange, 2500f), Is.False);
            Assert.That(BeamWeaponMath.CanFireAtTarget(42UL, origin, inRange, 2500f), Is.True);
            Assert.That(BeamWeaponMath.CanFireAtTarget(42UL, origin, new Vector3(0f, 0f, 3000f), 2500f), Is.False);
        }
    }
}
