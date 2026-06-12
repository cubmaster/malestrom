using IronExiles.Combat;
using IronExiles.Core;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public class ProjectLoadsTests
    {
        private const string EmptySectorScenePath = "Assets/Scenes/Test/EmptySector.unity";

        [Test]
        public void ProjectLoads_EmptySectorSceneExists()
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(EmptySectorScenePath);
            Assert.IsNotNull(sceneAsset, $"Expected scene at {EmptySectorScenePath}");
        }

        [Test]
        public void ProjectLoads_BootstrapTypeLoads()
        {
            Assert.AreEqual("Iron Exiles", GameBootstrap.ProjectName);
        }

        [Test]
        public void ProjectLoads_EmptySectorOpensWithoutError()
        {
            var scene = EditorSceneManager.OpenScene(EmptySectorScenePath, OpenSceneMode.Single);
            Assert.IsTrue(scene.IsValid());
            Assert.AreEqual("EmptySector", scene.name);
            Assert.IsNotNull(Camera.main, "Main Camera should be tagged in EmptySector");
        }

        [Test]
        public void ProjectLoads_FlightSetupPresentInEmptySector()
        {
            EditorSceneManager.OpenScene(EmptySectorScenePath, OpenSceneMode.Single);
            var setup = Object.FindFirstObjectByType<EmptySectorFlightSetup>();
            Assert.IsNotNull(setup, "EmptySector should contain EmptySectorFlightSetup for REQ-033");
        }
    }
}
