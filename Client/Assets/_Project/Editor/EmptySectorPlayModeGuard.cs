using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IronExiles.Editor
{
    [InitializeOnLoad]
    static class EmptySectorPlayModeGuard
    {
        static EmptySectorPlayModeGuard()
        {
            EditorApplication.delayCall += EnsureEmptySectorOpenInEditor;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void EnsureEmptySectorOpenInEditor()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (IsEmptySectorOpenInEditor())
            {
                return;
            }

            var activeScene = EditorSceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(activeScene.path))
            {
                return;
            }

            EmptySectorSceneMenu.OpenEmptySectorIfNeeded();
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            if (WillRunEmptySector())
            {
                return;
            }

            EditorApplication.isPlaying = false;

            var activeScene = EditorSceneManager.GetActiveScene();
            var sceneLabel = string.IsNullOrEmpty(activeScene.path)
                ? activeScene.name
                : activeScene.path;

            Debug.LogWarning(
                "[Iron Exiles] Opening EmptySector before Play. Active edit scene: " +
                $"{sceneLabel}. Expected {EmptySectorSceneMenu.ScenePath}.");

            EmptySectorSceneMenu.OpenEmptySectorIfNeeded();
            EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
        }

        static bool WillRunEmptySector()
        {
            if (IsEmptySectorOpenInEditor())
            {
                return true;
            }

            var startScene = EditorSceneManager.playModeStartScene;
            if (startScene == null)
            {
                return false;
            }

            return AssetDatabase.GetAssetPath(startScene) == EmptySectorSceneMenu.ScenePath;
        }

        static bool IsEmptySectorOpenInEditor()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            return activeScene.path == EmptySectorSceneMenu.ScenePath && FindFlightSetup() != null;
        }

        static GameObject FindFlightSetup()
        {
            foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (root.name == "FlightSetup")
                {
                    return root;
                }
            }

            return null;
        }
    }
}
