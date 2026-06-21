using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IronExiles.Editor
{
    public static class EmptySectorSceneMenu
    {
        public const string ScenePath = "Assets/Scenes/Test/EmptySector.unity";

        [MenuItem("Iron Exiles/Open EmptySector Scene")]
        public static void OpenEmptySector()
        {
            OpenEmptySector(promptSave: true);
        }

        public static void OpenEmptySector(bool promptSave)
        {
            if (promptSave && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Debug.Log("[Iron Exiles] Opened EmptySector. Hierarchy should show FlightSetup — then press Play.");
        }

        internal static void OpenEmptySectorIfNeeded()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.path == ScenePath)
            {
                return;
            }

            var canOpenWithoutPrompt = string.IsNullOrEmpty(activeScene.path) && !activeScene.isDirty;
            OpenEmptySector(promptSave: !canOpenWithoutPrompt);
        }
    }
}
