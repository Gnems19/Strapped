using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Self-bootstrapping mobile controls.
/// Place this component on any GameObject in your gameplay scenes.
/// On mobile it creates its own DontDestroyOnLoad Canvas with Left, Right, Jump, and Interact buttons
/// using custom pixel art sprites. On desktop/editor (non-mobile) it hides itself.
/// </summary>
public class MobileControls : MonoBehaviour
{
    public static MobileControls Instance { get; private set; }

    [Header("Movement Button Sprites")]
    [SerializeField] private Sprite buttonNormal;
    [SerializeField] private Sprite buttonPressed;

    [Header("Interact Button Sprites")]
    [SerializeField] private Sprite interactNormal;
    [SerializeField] private Sprite interactPressed;

    public float HorizontalInput { get; private set; }
    public bool JumpDown { get; private set; }
    public bool JumpUp { get; private set; }
    public bool InteractDown { get; private set; }

    private bool _leftHeld;
    private bool _rightHeld;
    private bool _jumpPressedThisFrame;
    private bool _jumpReleasedThisFrame;
    private bool _interactPressedThisFrame;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        if (!IsMobile())
        {
            gameObject.SetActive(false);
            return;
        }

        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        BuildUI();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Hide controls on MainMenu (0) and EndScene (3), show on gameplay scenes
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
            canvas.enabled = scene.buildIndex != 0 && scene.buildIndex != 3;
    }

    void Update()
    {
        HorizontalInput = 0f;
        if (_leftHeld) HorizontalInput -= 1f;
        if (_rightHeld) HorizontalInput += 1f;

        JumpDown = _jumpPressedThisFrame;
        JumpUp = _jumpReleasedThisFrame;
        InteractDown = _interactPressedThisFrame;
        _jumpPressedThisFrame = false;
        _jumpReleasedThisFrame = false;
        _interactPressedThisFrame = false;
    }

    private void BuildUI()
    {
        // Add Canvas as screen overlay so it's independent of camera viewport
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 540);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

        gameObject.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }

        // Button sizing: percentage-friendly values at 960x540 reference
        float btnSize = 70f;
        float gap = 10f;
        float marginX = 10f;
        float marginY = 10f + btnSize * 0.4f;

        // === LEFT SIDE: movement arrows (shifted up by btnSize and right by btnSize) ===

        // Left arrow — rotated 90° so up-arrow points left
        CreateButton("LeftBtn",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(marginX + btnSize, marginY - btnSize * 0.5f),
            new Vector2(btnSize, btnSize),
            buttonNormal, buttonPressed, 90f,
            () => _leftHeld = true, () => _leftHeld = false);

        // Right arrow — rotated -90° so up-arrow points right
        CreateButton("RightBtn",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(marginX + btnSize + gap, marginY + btnSize - btnSize * 0.5f),
            new Vector2(btnSize, btnSize),
            buttonNormal, buttonPressed, -90f,
            () => _rightHeld = true, () => _rightHeld = false);

        // === RIGHT SIDE: action buttons (shifted up by btnSize) ===

        // Jump — bottom-right, arrow points up (no rotation)
        CreateButton("JumpBtn",
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(-marginX, marginY + btnSize - btnSize * 0.5f),
            new Vector2(btnSize, btnSize),
            buttonNormal, buttonPressed, 0f,
            () => _jumpPressedThisFrame = true, () => _jumpReleasedThisFrame = true);

        // Interact — left of jump
        CreateButton("InteractBtn",
            new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(-marginX - btnSize - gap, marginY + btnSize - btnSize * 0.5f),
            new Vector2(btnSize, btnSize),
            interactNormal, interactPressed, 0f,
            () => _interactPressedThisFrame = true, null);
    }

    private void CreateButton(string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 size,
        Sprite normal, Sprite pressed, float zRotation,
        System.Action onDown, System.Action onUp)
    {
        var btnGo = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        btnGo.transform.SetParent(transform, false);

        var rt = btnGo.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        rt.localRotation = Quaternion.Euler(0, 0, zRotation);

        var img = btnGo.GetComponent<Image>();
        img.type = Image.Type.Simple;
        img.preserveAspect = true;
        img.raycastTarget = true;

        var defaultAlpha = 0.85f;
        if (normal != null)
        {
            img.sprite = normal;
            img.color = new Color(1f, 1f, 1f, defaultAlpha);
        }
        else
            img.color = new Color(1f, 1f, 1f, 0.35f);

        var trigger = btnGo.AddComponent<EventTrigger>();

        if (onDown != null)
        {
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener(_ =>
            {
                onDown();
                if (pressed != null) img.sprite = pressed;
                else if (normal != null) img.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            });
            trigger.triggers.Add(entry);
        }

        {
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entry.callback.AddListener(_ =>
            {
                onUp?.Invoke();
                if (normal != null) img.sprite = normal;
                img.color = new Color(1f, 1f, 1f, defaultAlpha);
            });
            trigger.triggers.Add(entry);
        }
    }

    private static bool IsMobile()
    {
#if UNITY_ANDROID || UNITY_IOS
        return true;
#else
        return UnityEngine.Device.Application.isMobilePlatform;
#endif
    }
}
