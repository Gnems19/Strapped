using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class SetupMobileControls
{
    public static void Execute()
    {
        // Find the MobileControlsPanel
        var panel = GameObject.Find("SystemCanvas/UIPanel/MobileControlsPanel");
        if (panel == null)
        {
            Debug.LogError("MobileControlsPanel not found!");
            return;
        }

        // Check if InteractButton already exists
        var existing = panel.transform.Find("InteractButton");
        if (existing != null)
        {
            Debug.Log("InteractButton already exists, skipping creation.");
        }
        else
        {
            // Create InteractButton as a child of MobileControlsPanel
            var btnGo = new GameObject("InteractButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(panel.transform, false);

            // Set up the Image
            var img = btnGo.GetComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.5f);

            // Set up RectTransform
            var rt = btnGo.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(120, 120);

            // Add a text child
            var textGo = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(btnGo.transform, false);
            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = "E";
            tmp.fontSize = 36;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;

            Debug.Log("InteractButton created.");
        }

        // Update text on existing buttons for clarity
        SetButtonText(panel, "LeftButton", "<");
        SetButtonText(panel, "RightButton", ">");
        SetButtonText(panel, "JumpButton", "^");

        EditorUtility.SetDirty(panel);
        Debug.Log("MobileControls setup complete.");
    }

    private static void SetButtonText(GameObject panel, string buttonName, string text)
    {
        var btn = panel.transform.Find(buttonName);
        if (btn == null) return;

        var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;
        }
    }
}
