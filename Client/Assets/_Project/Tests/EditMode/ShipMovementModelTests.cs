using IronExiles.Combat;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public class ShipMovementModelTests
    {
        [Test]
        public void SpeedClamp_MaxThrustNeverExceedsMaxSpeed()
        {
            var model = new ShipMovementModel();
            model.SetStats(new ShipStatsSnapshot(
                maxSpeed: 10f,
                forwardThrust: 500f,
                strafeThrust: 500f,
                rotationRate: 90f,
                brakeDeceleration: 0f));
            model.SetSectorBoundsExtent(new Vector3(10000f, 10000f, 10000f));
            model.Reset(Vector3.zero, Quaternion.identity);
            model.SetMovementInput(new Vector3(1f, 0f, 0f), Vector3.zero, brake: false);

            for (var tick = 0; tick < 600; tick++)
            {
                model.Simulate(0.016f);
            }

            Assert.LessOrEqual(model.Velocity.magnitude, 10f + 0.01f);
        }

        [Test]
        public void SectorBounds_PositionStaysInsideAabb()
        {
            var model = new ShipMovementModel();
            var extent = new Vector3(5f, 5f, 5f);
            model.SetSectorBoundsExtent(extent);
            model.SetStats(ShipStatsDefinition.HumanStarterFighterDefaults());
            model.Reset(Vector3.zero, Quaternion.identity);
            model.SetMovementInput(new Vector3(0f, 1f, 0f), Vector3.zero, brake: false);

            for (var tick = 0; tick < 2000; tick++)
            {
                model.Simulate(0.016f);
            }

            Assert.LessOrEqual(Mathf.Abs(model.Position.x), extent.x + 0.01f);
            Assert.LessOrEqual(Mathf.Abs(model.Position.y), extent.y + 0.01f);
            Assert.LessOrEqual(Mathf.Abs(model.Position.z), extent.z + 0.01f);
        }

        [Test]
        public void Momentum_ReleasingThrustRetainsVelocity()
        {
            var model = new ShipMovementModel();
            model.SetStats(ShipStatsDefinition.HumanStarterFighterDefaults());
            model.SetSectorBoundsExtent(new Vector3(10000f, 10000f, 10000f));
            model.Reset(Vector3.zero, Quaternion.identity);
            model.SetMovementInput(new Vector3(1f, 0f, 0f), Vector3.zero, brake: false);

            for (var tick = 0; tick < 30; tick++)
            {
                model.Simulate(0.016f);
            }

            var speedAfterThrust = model.Velocity.magnitude;
            Assert.Greater(speedAfterThrust, 0.1f);

            model.SetMovementInput(Vector3.zero, Vector3.zero, brake: false);
            model.Simulate(0.016f);

            Assert.Greater(model.Velocity.magnitude, speedAfterThrust * 0.9f);
        }
    }
}
