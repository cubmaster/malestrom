using IronExiles.Networking;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public sealed class SpawnPointManagerTests
    {
        [Test]
        public void GetNextSpawnPosition_allocates_distinct_round_robin_positions()
        {
            var root = new GameObject("SpawnRoot");
            var manager = root.AddComponent<SpawnPointManager>();

            var pointA = new GameObject("A").transform;
            pointA.position = new Vector3(0f, 0f, 0f);
            var pointB = new GameObject("B").transform;
            pointB.position = new Vector3(20f, 0f, 0f);

            manager.ConfigureSpawnPoints(new[] { pointA, pointB });

            Assert.That(manager.GetNextSpawnPosition(), Is.EqualTo(pointA.position));
            Assert.That(manager.GetNextSpawnPosition(), Is.EqualTo(pointB.position));
            Assert.That(manager.GetNextSpawnPosition(), Is.EqualTo(pointA.position));

            Object.DestroyImmediate(root);
            Object.DestroyImmediate(pointA.gameObject);
            Object.DestroyImmediate(pointB.gameObject);
        }
    }
}
