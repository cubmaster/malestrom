using IronExiles.Combat;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public sealed class TargetSelectionTests
    {
        [Test]
        public void CollectTabCandidates_sorts_by_forward_angle_then_distance()
        {
            var origin = Vector3.zero;
            var forward = Vector3.forward;
            var self = CreateTargetable("Self", TargetAffiliation.Friendly, new Vector3(0f, 0f, 0f), 1UL);
            var nearSide = CreateTargetable("Near", TargetAffiliation.Hostile, new Vector3(0f, 0f, 30f), 2UL);
            var farSide = CreateTargetable("Far", TargetAffiliation.Hostile, new Vector3(0f, 0f, 80f), 3UL);
            var offAxis = CreateTargetable("OffAxis", TargetAffiliation.Hostile, new Vector3(40f, 0f, 10f), 4UL);

            var candidates = TargetSelectionMath.CollectTabCandidates(
                origin,
                forward,
                self.GetNetworkObjectId(),
                new[] { self, nearSide, farSide, offAxis },
                500f);

            Assert.That(candidates.Count, Is.EqualTo(3));
            Assert.That(candidates[0].NetworkObjectId, Is.EqualTo(2UL));
            Assert.That(candidates[1].NetworkObjectId, Is.EqualTo(3UL));
            Assert.That(candidates[2].NetworkObjectId, Is.EqualTo(4UL));
        }

        [Test]
        public void SelectNextTabIndex_cycles_forward()
        {
            var candidates = new[]
            {
                new TargetCandidate(10UL, 10f, 5f),
                new TargetCandidate(20UL, 20f, 15f),
                new TargetCandidate(30UL, 30f, 25f)
            };

            var nextFromNone = TargetSelectionMath.SelectNextTabIndex(candidates, 0UL, 1);
            var nextFromFirst = TargetSelectionMath.SelectNextTabIndex(candidates, 10UL, 1);
            var wrapFromLast = TargetSelectionMath.SelectNextTabIndex(candidates, 30UL, 1);

            Assert.That(nextFromNone, Is.EqualTo(0));
            Assert.That(nextFromFirst, Is.EqualTo(1));
            Assert.That(wrapFromLast, Is.EqualTo(0));
        }

        [Test]
        public void IsWithinLockRange_rejects_out_of_range_targets()
        {
            Assert.True(TargetSelectionMath.IsWithinLockRange(Vector3.zero, new Vector3(100f, 0f, 0f), 250f));
            Assert.False(TargetSelectionMath.IsWithinLockRange(Vector3.zero, new Vector3(300f, 0f, 0f), 250f));
        }

        [Test]
        public void ToRadarPlane01_rotates_contacts_when_ship_turns()
        {
            var origin = Vector3.zero;
            var forward = Vector3.forward;
            var targetPosition = new Vector3(0f, 0f, 80f);
            const float lockRange = 500f;

            var ahead = TargetSelectionMath.ToRadarPlane01(origin, forward, targetPosition, lockRange);
            var turnedRight = TargetSelectionMath.ToRadarPlane01(origin, Vector3.right, targetPosition, lockRange);

            Assert.That(Mathf.Abs(ahead.y), Is.GreaterThan(Mathf.Abs(ahead.x)));
            Assert.That(Mathf.Abs(turnedRight.x), Is.GreaterThan(0.1f));
            Assert.That(Mathf.Abs(turnedRight.y), Is.LessThan(0.05f));
        }

        [Test]
        public void CollectRadarContacts_limits_to_max_contacts_by_distance()
        {
            var origin = Vector3.zero;
            var forward = Vector3.forward;
            var self = CreateTargetable("Self", TargetAffiliation.Friendly, Vector3.zero, 1UL);
            var targets = new TargetableEntity[5];
            for (var i = 0; i < targets.Length; i++)
            {
                targets[i] = CreateTargetable($"T{i}", TargetAffiliation.Hostile, new Vector3((i + 1) * 10f, 0f, 0f), (ulong)(i + 2));
            }

            var all = new TargetableEntity[targets.Length + 1];
            all[0] = self;
            for (var i = 0; i < targets.Length; i++)
            {
                all[i + 1] = targets[i];
            }

            var contacts = TargetSelectionMath.CollectRadarContacts(origin, forward, 1UL, all, 500f, 3);
            Assert.That(contacts.Count, Is.EqualTo(3));
            Assert.That(contacts[0].Distance, Is.LessThan(contacts[1].Distance));
            Assert.That(contacts[1].Distance, Is.LessThan(contacts[2].Distance));
        }

        static TargetableEntity CreateTargetable(string name, TargetAffiliation affiliation, Vector3 position, ulong networkObjectId)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var targetable = go.AddComponent<TargetableEntity>();
            targetable.Configure(name, affiliation, 100f);
            targetable.AssignNetworkObjectIdForTests(networkObjectId);
            return targetable;
        }
    }
}
