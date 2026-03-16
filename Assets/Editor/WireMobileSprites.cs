using UnityEngine;
using UnityEditor;

public class WireMobileSprites
{
    public static void Execute()
    {
        // Load sprites
        var btnNormal = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI Elements/Button.png");
        var btnPressed = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI Elements/ButtonPressed.png");
        var interactNormal = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI Elements/InteractionButton.png");

        if (btnNormal == null) { Debug.LogError("Button.png not found"); return; }
        if (btnPressed == null) { Debug.LogError("ButtonPressed.png not found"); return; }
        if (interactNormal == null) { Debug.LogError("InteractionButton.png not found"); return; }

        Debug.Log($"Loaded sprites: {btnNormal.name}, {btnPressed.name}, {interactNormal.name}");

        // Find all MobileControls in the scene
        var allMC = Object.FindObjectsByType<MobileControls>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (allMC.Length == 0)
        {
            Debug.LogError("No MobileControls component found in scene!");
            return;
        }

        foreach (var mc in allMC)
        {
            var so = new SerializedObject(mc);
            so.FindProperty("buttonNormal").objectReferenceValue = btnNormal;
            so.FindProperty("buttonPressed").objectReferenceValue = btnPressed;
            so.FindProperty("interactNormal").objectReferenceValue = interactNormal;
            // interactPressed left null — will tint darker on press
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(mc);
            Debug.Log($"Wired sprites on {mc.gameObject.name}");
        }
    }
}
