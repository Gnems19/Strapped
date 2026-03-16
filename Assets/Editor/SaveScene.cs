using UnityEditor;
using UnityEditor.SceneManagement;

public class SaveScene
{
    public static string Execute()
    {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        return "Scene saved: " + EditorSceneManager.GetActiveScene().path;
    }
}
