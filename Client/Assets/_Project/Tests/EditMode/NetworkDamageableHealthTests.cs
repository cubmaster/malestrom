using IronExiles.Combat;
using NUnit.Framework;

namespace IronExiles.Core.Tests
{
    public sealed class NetworkDamageableHealthTests
    {
        [Test]
        public void ApplyDamage_clamps_at_zero()
        {
            var next = DamageableHealthMath.ApplyDamage(100f, 40f, out var destroyed);
            Assert.That(next, Is.EqualTo(60f));
            Assert.That(destroyed, Is.False);

            next = DamageableHealthMath.ApplyDamage(30f, 50f, out destroyed);
            Assert.That(next, Is.EqualTo(0f));
            Assert.That(destroyed, Is.True);
        }

        [Test]
        public void ToHullPercent_converts_current_to_percentage()
        {
            Assert.That(DamageableHealthMath.ToHullPercent(500f, 1000f), Is.EqualTo(50f).Within(0.01f));
            Assert.That(DamageableHealthMath.ToHullPercent(0f, 1000f), Is.EqualTo(0f));
        }
    }
}
