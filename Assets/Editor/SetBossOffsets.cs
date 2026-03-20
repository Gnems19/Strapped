using UnityEditor;
using UnityEngine;

public class SetBossOffsets
{
    public static string Execute()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (!scene.IsValid()) return "ERROR: No valid active scene";

        var roots = scene.GetRootGameObjects();
        if (roots == null || roots.Length == 0) return $"ERROR: No root objects in scene '{scene.name}'";

        var info = $"Scene: {scene.name}, roots: {roots.Length}. ";
        foreach (var root in roots)
        {
            if (root == null) continue;
            var script = root.GetComponentInChildren<BossScript>(true);
            if (script == null) continue;

            // Use SerializedObject to set private [SerializeField] values
            var so = new SerializedObject(script);

            var propR = so.FindProperty("launchOffsetXRight");
            var propL = so.FindProperty("launchOffsetXLeft");
            var propY = so.FindProperty("launchOffsetY");

            // Set X offsets now; launchOffsetY will use code default (5.5) after recompile
            if (propR == null) return info + "ERROR: property 'launchOffsetXRight' not found";
            if (propL == null) return info + "ERROR: property 'launchOffsetXLeft' not found";

            propR.floatValue = 3.3f;
            propL.floatValue = -12.05f;
            if (propY != null) propY.floatValue = 5.5f;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(script);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);

            return info + $"OK: Set offsets Right=3.3, Left=-2.05, Y={(propY != null ? "5.5" : "pending recompile")} on '{script.gameObject.name}'";
        }

        return info + "ERROR: No BossScript found on any root or child";
    }
}
