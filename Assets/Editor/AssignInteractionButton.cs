using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AssignInteractionButton
    {
        [MenuItem("Tools/Assign Mobile Button Sprites")]
        public static void RunFromMenu()
        {
            var result = Execute();
            Debug.Log(result);
        }

        public static string Execute()
        {
            var mc = Object.FindFirstObjectByType<MobileControls>(FindObjectsInactive.Include);
            if (mc == null)
                return "ERROR: No MobileControls found in scene";

            var so = new SerializedObject(mc);
            var msg = "";

            // Movement buttons from Button.aseprite
            var btnSprites = LoadSpritePair("Assets/UI Elements/Button.aseprite");
            if (btnSprites.normal != null)
            {
                so.FindProperty("buttonNormal").objectReferenceValue = btnSprites.normal;
                if (btnSprites.pressed != null)
                    so.FindProperty("buttonPressed").objectReferenceValue = btnSprites.pressed;
                msg += $"Movement: normal={btnSprites.normal.name}, pressed={btnSprites.pressed?.name ?? "none"}. ";
            }
            else
            {
                msg += "ERROR: No sprites in Button.aseprite. ";
            }

            // Interact button from InteractionButton.aseprite
            var interactSprites = LoadSpritePair("Assets/UI Elements/InteractionButton.aseprite");
            if (interactSprites.normal != null)
            {
                so.FindProperty("interactNormal").objectReferenceValue = interactSprites.normal;
                if (interactSprites.pressed != null)
                    so.FindProperty("interactPressed").objectReferenceValue = interactSprites.pressed;
                msg += $"Interact: normal={interactSprites.normal.name}, pressed={interactSprites.pressed?.name ?? "none"}.";
            }
            else
            {
                msg += "ERROR: No sprites in InteractionButton.aseprite.";
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(mc);
            return msg;
        }

        private static (Sprite normal, Sprite pressed) LoadSpritePair(string asepritePath)
        {
            Sprite normal = null;
            Sprite pressed = null;
            var assets = AssetDatabase.LoadAllAssetsAtPath(asepritePath);
            foreach (var asset in assets)
            {
                if (asset is Sprite sprite)
                {
                    if (normal == null)
                        normal = sprite;
                    else if (pressed == null)
                        pressed = sprite;
                }
            }
            return (normal, pressed);
        }
    }
}
