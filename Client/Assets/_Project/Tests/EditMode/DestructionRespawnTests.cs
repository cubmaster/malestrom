using IronExiles.Combat;
using NUnit.Framework;

namespace IronExiles.Core.Tests
{
    [TestFixture]
    public sealed class DestructionRespawnTests
    {
        [Test]
        public void ApplyDamage_exact_hull_to_zero_reports_destroyed()
        {
            var result = DamageableHealthMath.ApplyDamage(100f, 100f, out var destroyed);

            Assert.That(result, Is.EqualTo(0f));
            Assert.That(destroyed, Is.True);
        }

        [Test]
        public void ApplyDamage_overshoot_clamps_to_zero()
        {
            var result = DamageableHealthMath.ApplyDamage(50f, 200f, out var destroyed);

            Assert.That(result, Is.EqualTo(0f));
            Assert.That(destroyed, Is.True);
        }

        [Test]
        public void ApplyDamage_on_already_zero_hull_stays_at_zero()
        {
            var result = DamageableHealthMath.ApplyDamage(0f, 50f, out var destroyed);

            Assert.That(result, Is.EqualTo(0f));
            Assert.That(destroyed, Is.True);
        }

        [Test]
        public void ApplyDamage_partial_damage_does_not_destroy()
        {
            var result = DamageableHealthMath.ApplyDamage(1000f, 500f, out var destroyed);

            Assert.That(result, Is.EqualTo(500f));
            Assert.That(destroyed, Is.False);
        }

        [Test]
        public void ApplyDamage_negative_damage_treated_as_zero()
        {
            var result = DamageableHealthMath.ApplyDamage(100f, -50f, out var destroyed);

            Assert.That(result, Is.EqualTo(100f));
            Assert.That(destroyed, Is.False);
        }

        [Test]
        public void ApplyDamage_exactly_one_hp_remaining_not_destroyed()
        {
            var result = DamageableHealthMath.ApplyDamage(100f, 99f, out var destroyed);

            Assert.That(result, Is.EqualTo(1f).Within(0.001f));
            Assert.That(destroyed, Is.False);
        }

        [Test]
        public void HullPercent_zero_hull_returns_zero_percent()
        {
            var percent = DamageableHealthMath.ToHullPercent(0f, 1000f);

            Assert.That(percent, Is.EqualTo(0f));
        }

        [Test]
        public void HullPercent_full_hull_returns_hundred_percent()
        {
            var percent = DamageableHealthMath.ToHullPercent(1000f, 1000f);

            Assert.That(percent, Is.EqualTo(100f));
        }

        [Test]
        public void Sequential_damage_accumulates_correctly()
        {
            var hull = 1000f;
            hull = DamageableHealthMath.ApplyDamage(hull, 300f, out var destroyed1);
            Assert.That(hull, Is.EqualTo(700f));
            Assert.That(destroyed1, Is.False);

            hull = DamageableHealthMath.ApplyDamage(hull, 400f, out var destroyed2);
            Assert.That(hull, Is.EqualTo(300f));
            Assert.That(destroyed2, Is.False);

            hull = DamageableHealthMath.ApplyDamage(hull, 300f, out var destroyed3);
            Assert.That(hull, Is.EqualTo(0f));
            Assert.That(destroyed3, Is.True);
        }

        [Test]
        public void Repeated_damage_on_destroyed_hull_no_further_reduction()
        {
            var hull = DamageableHealthMath.ApplyDamage(50f, 100f, out var destroyed1);
            Assert.That(hull, Is.EqualTo(0f));
            Assert.That(destroyed1, Is.True);

            // Further damage on zero hull should stay at zero
            hull = DamageableHealthMath.ApplyDamage(hull, 100f, out var destroyed2);
            Assert.That(hull, Is.EqualTo(0f));
            Assert.That(destroyed2, Is.True);
        }
    }
}
