using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class WireBossScript
{
    public static string Execute()
    {
        // Find BossFightManager (should be active)
        var bfm = GameObject.Find("BossFightManager");
        if (bfm == null) return "ERROR: BossFightManager not found";

        // Find TheBoss under it (may be inactive)
        var bossTransform = bfm.transform.Find("TheBoss");
        if (bossTransform == null) return "ERROR: TheBoss not found under BossFightManager";
        var boss = bossTransform.gameObject;

        // Check which BossScript is attached
        var rootBs = boss.GetComponent<BossScript>();
        var enemyBs = boss.GetComponent<EnemyScripts.BossScript>();

        string status = "Components: root=" + (rootBs != null) + " enemy=" + (enemyBs != null);

        // Remove old EnemyScripts.BossScript if present
        if (enemyBs != null)
        {
            Undo.DestroyObjectImmediate(enemyBs);
            status += " | Removed EnemyScripts.BossScript";
        }

        // Add root BossScript if missing
        if (rootBs == null)
        {
            rootBs = Undo.AddComponent<BossScript>(boss);
            status += " | Added root BossScript";
        }

        // Find references
        var missileLaunchTransform = bfm.transform.Find("MissileLaunch");
        if (missileLaunchTransform == null) return status + " | ERROR: MissileLaunch not found";

        var player = GameObject.Find("Player");
        if (player == null) return status + " | ERROR: Player not found";

        var animator = boss.GetComponent<Animator>();
        if (animator == null) return status + " | ERROR: Animator not found";

        // Wire fields via SerializedObject
        var so = new SerializedObject(rootBs);
        so.FindProperty("missileLauncherRockets").objectReferenceValue = missileLaunchTransform.gameObject;
        so.FindProperty("_animator").objectReferenceValue = animator;
        so.FindProperty("player").objectReferenceValue = player;
        so.ApplyModifiedProperties();

        // Mark scene dirty and save
        EditorUtility.SetDirty(rootBs);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        return status + " | WIRED & SAVED: missiles=" + missileLaunchTransform.name +
               " animator=" + animator.name + " player=" + player.name;
    }
}
