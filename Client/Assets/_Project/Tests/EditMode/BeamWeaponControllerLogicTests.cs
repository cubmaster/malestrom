using IronExiles.Combat;
using NUnit.Framework;

namespace IronExiles.Core.Tests
{
    public sealed class BeamWeaponControllerLogicTests
    {
        [Test]
        public void SimulateSustainedFire_reduces_hull_over_ten_seconds()
        {
            var remaining = BeamWeaponControllerLogic.SimulateSustainedFire(
                BeamWeaponSettings.DefaultTier1BaseDps,
                1f,
                10f,
                0.05f,
                BeamWeaponSettings.DefaultMaxHull);

            Assert.That(remaining, Is.LessThan(BeamWeaponSettings.DefaultMaxHull));
            Assert.That(remaining, Is.EqualTo(500f).Within(5f));
        }

        [Test]
        public void TryApplyTickDamage_returns_false_without_lock()
        {
            var hull = 100f;
            var applied = BeamWeaponControllerLogic.TryApplyTickDamage(
                true,
                0UL,
                UnityEngine.Vector3.zero,
                UnityEngine.Vector3.forward,
                2500f,
                50f,
                1f,
                0.1f,
                ref hull,
                out var damage);

            Assert.That(applied, Is.False);
            Assert.That(damage, Is.EqualTo(0f));
            Assert.That(hull, Is.EqualTo(100f));
        }

        [Test]
        public void TryApplyTickDamageWithShields_absorbs_damage_in_shield_before_hull()
        {
            var shieldHp = 100f;
            var hull = 500f;

            var applied = BeamWeaponControllerLogic.TryApplyTickDamageWithShields(
                true,
                1UL,
                UnityEngine.Vector3.zero,
                UnityEngine.Vector3.forward * 100f,
                2500f,
                50f,
                1f,
                1f,
                ref shieldHp,
                ref hull,
                out var damageApplied,
                out var shieldAbsorbed);

            Assert.That(applied, Is.True);
            Assert.That(damageApplied, Is.EqualTo(50f).Within(0.01f));
            Assert.That(shieldAbsorbed, Is.EqualTo(50f).Within(0.01f));
            Assert.That(shieldHp, Is.EqualTo(50f).Within(0.01f));
            Assert.That(hull, Is.EqualTo(500f), "Hull should not be damaged when shield absorbs all");
        }

        [Test]
        public void TryApplyTickDamageWithShields_overflow_damages_hull_when_shield_depleted()
        {
            var shieldHp = 20f;
            var hull = 500f;

            var applied = BeamWeaponControllerLogic.TryApplyTickDamageWithShields(
                true,
                1UL,
                UnityEngine.Vector3.zero,
                UnityEngine.Vector3.forward * 100f,
                2500f,
                50f,
                1f,
                1f,
                ref shieldHp,
                ref hull,
                out var damageApplied,
                out var shieldAbsorbed);

            Assert.That(applied, Is.True);
            Assert.That(damageApplied, Is.EqualTo(50f).Within(0.01f));
            Assert.That(shieldAbsorbed, Is.EqualTo(20f).Within(0.01f));
            Assert.That(shieldHp, Is.EqualTo(0f).Within(0.01f));
            Assert.That(hull, Is.EqualTo(470f).Within(0.01f), "Hull takes overflow damage (50 - 20 = 30)");
        }

        [Test]
        public void TryApplyTickDamageWithShields_zero_shield_passes_all_to_hull()
        {
            var shieldHp = 0f;
            var hull = 500f;

            var applied = BeamWeaponControllerLogic.TryApplyTickDamageWithShields(
                true,
                1UL,
                UnityEngine.Vector3.zero,
                UnityEngine.Vector3.forward * 100f,
                2500f,
                50f,
                1f,
                1f,
                ref shieldHp,
                ref hull,
                out var damageApplied,
                out var shieldAbsorbed);

            Assert.That(applied, Is.True);
            Assert.That(shieldAbsorbed, Is.EqualTo(0f));
            Assert.That(hull, Is.EqualTo(450f).Within(0.01f), "All damage passes through to hull");
        }

        [Test]
        public void TryApplyTickDamageWithShields_returns_false_when_not_firing()
        {
            var shieldHp = 100f;
            var hull = 500f;

            var applied = BeamWeaponControllerLogic.TryApplyTickDamageWithShields(
                false,
                1UL,
                UnityEngine.Vector3.zero,
                UnityEngine.Vector3.forward * 100f,
                2500f,
                50f,
                1f,
                1f,
                ref shieldHp,
                ref hull,
                out var damageApplied,
                out var shieldAbsorbed);

            Assert.That(applied, Is.False);
            Assert.That(shieldHp, Is.EqualTo(100f));
            Assert.That(hull, Is.EqualTo(500f));
        }
    }
}
