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
    }
}
