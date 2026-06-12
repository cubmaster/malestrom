using IronExiles.Combat;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public class ChaseCameraPlacementTests
    {
        [Test]
        public void ResolveEyePosition_WithoutObstruction_ReturnsDesiredEye()
        {
            var pivot = Vector3.zero;
            var desired = new Vector3(0f, 4f, -12f);

            var resolved = ChaseCameraPlacement.ResolveEyePosition(
                pivot,
                desired,
                obstructionDistance: 0f,
                hasObstruction: false,
                collisionPadding: 0.25f,
                minDistanceFromPivot: 2f);

            Assert.AreEqual(desired, resolved);
        }

        [Test]
        public void ResolveEyePosition_WithObstruction_PullsCameraTowardPivot()
        {
            var pivot = Vector3.zero;
            var desired = new Vector3(0f, 0f, -10f);

            var resolved = ChaseCameraPlacement.ResolveEyePosition(
                pivot,
                desired,
                obstructionDistance: 5f,
                hasObstruction: true,
                collisionPadding: 0.25f,
                minDistanceFromPivot: 2f);

            Assert.AreEqual(new Vector3(0f, 0f, -4.75f), resolved);
        }

        [Test]
        public void ResolveEyePosition_WithObstruction_RespectsMinimumArmDistance()
        {
            var pivot = Vector3.zero;
            var desired = new Vector3(0f, 0f, -10f);

            var resolved = ChaseCameraPlacement.ResolveEyePosition(
                pivot,
                desired,
                obstructionDistance: 2.1f,
                hasObstruction: true,
                collisionPadding: 0.25f,
                minDistanceFromPivot: 2f);

            Assert.AreEqual(new Vector3(0f, 0f, -2f), resolved);
        }
    }
}
