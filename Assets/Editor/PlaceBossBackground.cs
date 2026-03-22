using UnityEngine;
using UnityEditor;

public class PlaceBossBackground
{
    public static string Execute()
    {
        // Force reimport with updated PPU
        AssetDatabase.ImportAsset("Assets/Sprites/Environment/BossBackground.aseprite", ImportAssetOptions.ForceUpdate);

        // Load all sub-assets from the Aseprite file — the importer generates a prefab sub-asset
        var allAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Environment/BossBackground.aseprite");

        GameObject prefab = null;
        foreach (var asset in allAssets)
        {
            if (asset is GameObject go)
            {
                prefab = go;
                break;
            }
        }

        if (prefab == null)
            return "ERROR: No prefab sub-asset found in BossBackground.aseprite";

        // Instantiate the prefab into the scene
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = "BossBackground";

        // Position at the center of the boss arena
        // BossFightManager world pos: (44.56, 3.0)
        // BossArea local offset: (12.0, -1.0) -> world (56.56, 2.0)
        // Collider offset: (13.97, 0.5), size: (41.06, 9.0)
        // Arena center: ~(70.53, 2.5)
        instance.transform.position = new Vector3(70.53f, 2.5f, 0f);

        // Set sorting layer to Background so it renders behind gameplay
        var renderers = instance.GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
        {
            r.sortingLayerName = "Background";
            r.sortingOrder = -1;
        }

        // Parent it under BossFightManager for organization
        var bossManager = GameObject.Find("BossFightManager");
        if (bossManager != null)
        {
            instance.transform.SetParent(bossManager.transform, true);
        }

        // Mark scene dirty so it can be saved
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        var info = $"Placed BossBackground at ({instance.transform.position.x}, {instance.transform.position.y}), " +
                   $"PPU=16, sorting layer=Background, renderers={renderers.Length}";

        // Log sprite size for verification
        if (renderers.Length > 0 && renderers[0].sprite != null)
        {
            var sprite = renderers[0].sprite;
            var bounds = renderers[0].bounds;
            info += $", sprite size={sprite.rect.width}x{sprite.rect.height}px, world bounds={bounds.size.x:F1}x{bounds.size.y:F1} units";
        }

        return info;
    }
}
