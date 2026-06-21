using IronExiles.Combat;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public class ReactorPowerAllocationTests
    {
        [Test]
        public void IsValid_acceptsNormalizedPreset()
        {
            Assert.IsTrue(ReactorPowerAllocationMath.IsValid(ReactorPowerAllocationMath.CombatPreset));
            Assert.IsTrue(ReactorPowerAllocationMath.IsValid(ReactorPowerAllocationMath.TravelPreset));
            Assert.IsTrue(ReactorPowerAllocationMath.IsValid(ReactorPowerAllocationMath.BalancedPreset));
        }

        [Test]
        public void IsValid_rejectsAllocationNotSummingToOne()
        {
            var invalid = new PowerAllocation
            {
                Weapons = 0.5f,
                Shields = 0.5f,
                Engines = 0.5f,
                Ecm = 0f
            };

            Assert.IsFalse(ReactorPowerAllocationMath.IsValid(invalid));
            Assert.IsFalse(ReactorPowerAllocationMath.TryCreate(0.5f, 0.5f, 0.5f, 0f, out _));
        }

        [Test]
        public void AdjustChannel_maintainsTotalOfOne()
        {
            var adjusted = ReactorPowerAllocationMath.AdjustChannel(
                ReactorPowerAllocationMath.CombatPreset,
                ReactorPowerAllocationMath.PowerChannel.Engines,
                0.8f);

            Assert.IsTrue(ReactorPowerAllocationMath.IsValid(adjusted));
            Assert.That(adjusted.Engines, Is.EqualTo(0.8f).Within(0.001f));
        }

        [Test]
        public void EngineMultiplier_atZeroEngines_isHalfPerformance()
        {
            var multiplier = ReactorPowerAllocationMath.GetEnginePerformanceMultiplier(0f);
            Assert.That(multiplier, Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void EngineMultiplier_atFullEngines_isFullPerformance()
        {
            var multiplier = ReactorPowerAllocationMath.GetEnginePerformanceMultiplier(1f);
            Assert.That(multiplier, Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void EngineAllocation_fullEngines_reachesHigherMaxSpeedThanFullWeapons()
        {
            var stats = new ShipStatsSnapshot(
                maxSpeed: 100f,
                forwardThrust: 500f,
                strafeThrust: 200f,
                rotationRate: 90f,
                brakeDeceleration: 0f);

            var maxEnginesSpeed = SimulateUntilSpeedPlateau(
                ReactorPowerAllocationMath.GetEnginePerformanceMultiplier(ReactorPowerAllocationMath.MaxEnginesPreset));

            var maxWeaponsSpeed = SimulateUntilSpeedPlateau(
                ReactorPowerAllocationMath.GetEnginePerformanceMultiplier(ReactorPowerAllocationMath.MaxWeaponsPreset));

            Assert.Greater(maxEnginesSpeed, maxWeaponsSpeed);
            Assert.That(maxEnginesSpeed, Is.EqualTo(100f).Within(1f));
            Assert.That(maxWeaponsSpeed, Is.EqualTo(50f).Within(1f));

            float SimulateUntilSpeedPlateau(float engineMultiplier)
            {
                var model = new ShipMovementModel();
                model.SetStats(stats);
                model.SetSectorBoundsExtent(new Vector3(10000f, 10000f, 10000f));
                model.SetEnginePerformanceMultiplier(engineMultiplier);
                model.Reset(Vector3.zero, Quaternion.identity);
                model.SetMovementInput(new Vector3(1f, 0f, 0f), Vector3.zero, brake: false);

                var lastSpeed = 0f;
                for (var tick = 0; tick < 800; tick++)
                {
                    model.Simulate(0.016f);
                    lastSpeed = model.Velocity.magnitude;
                }

                return lastSpeed;
            }
        }
    }
}
