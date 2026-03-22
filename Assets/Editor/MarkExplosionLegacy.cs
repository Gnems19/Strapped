using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class MarkExplosionLegacy
    {
        public static string Execute()
        {
            var path = "Assets/Sprites/Player/explosionAnimation.aseprite";
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            var count = 0;

            foreach (var asset in assets)
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__"))
                {
                    if (!clip.legacy)
                    {
                        clip.legacy = true;
                        EditorUtility.SetDirty(clip);
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                AssetDatabase.SaveAssets();
                return $"Marked {count} clip(s) as Legacy in {path}";
            }

            return "No non-legacy clips found — check if the clip is at a different path";
        }
    }
}
