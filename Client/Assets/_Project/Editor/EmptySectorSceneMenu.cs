using UnityEditor;
using UnityEditor.SceneManagement;

namespace IronExiles.Editor
{
    public static class EmptySectorSceneMenu
    {
        public const string ScenePath = "Assets/Scenes/Test/EmptySector.unity";

        [MenuItem("Iron Exiles/Open EmptySector Scene")]
        public static void OpenEmptySector()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }
        }
    }
}
