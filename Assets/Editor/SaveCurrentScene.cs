using UnityEditor.SceneManagement;

public class SaveCurrentScene
{
    public static void Execute()
    {
        EditorSceneManager.SaveOpenScenes();
        UnityEngine.Debug.Log("Saved all open scenes.");
    }
}
