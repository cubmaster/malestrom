using IronExiles.Combat;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public sealed class ShipMovementReplicationTests
    {
        [Test]
        public void EvaluateReconcileMode_returns_none_when_aligned()
        {
            var mode = ShipMovementReplicationMath.EvaluateReconcileMode(
                Vector3.zero,
                Quaternion.identity,
                Vector3.zero,
                Quaternion.identity,
                2f,
                15f);

            Assert.That(mode, Is.EqualTo(MovementReconcileMode.None));
        }

        [Test]
        public void EvaluateReconcileMode_returns_snap_when_position_error_exceeds_threshold()
        {
            var mode = ShipMovementReplicationMath.EvaluateReconcileMode(
                Vector3.zero,
                Quaternion.identity,
                new Vector3(3f, 0f, 0f),
                Quaternion.identity,
                2f,
                15f);

            Assert.That(mode, Is.EqualTo(MovementReconcileMode.Snap));
        }

        [Test]
        public void ShipMovementInput_clamps_axes()
        {
            var input = ShipMovementInput.FromAxes(
                new Vector3(2f, -2f, 0.5f),
                new Vector3(-3f, 0.25f, 1f),
                true);

            Assert.That(input.LocalThrust.x, Is.EqualTo(1f).Within(0.001f));
            Assert.That(input.LocalThrust.y, Is.EqualTo(-1f).Within(0.001f));
            Assert.That(input.LocalRotation.x, Is.EqualTo(-1f).Within(0.001f));
            Assert.True(input.Brake);
        }
    }
}
