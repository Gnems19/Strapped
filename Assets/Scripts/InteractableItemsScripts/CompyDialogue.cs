using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class CompyDialogue : MonoBehaviour
{
    private static int TimesInteracted;

    [SerializeField] private TMP_FontAsset pixelFont;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float bubbleYOffset = 1.2f;
    [SerializeField] private float bubbleScale = 0.025f;
    [SerializeField] private float fontSize = 16f;

    // Gameboy palette
    private static readonly Color32 BgColor = new Color32(15, 56, 15, 255);     // #0f380f darkest
    private static readonly Color32 TextColor = new Color32(139, 172, 15, 255);  // #8bac0f light

    private static readonly string[][] Dialogues =
    {
        new[] { "Psst! Hey!", "Name's Compy.", "They got you too huh?", "Find the outlet." },
        new[] { "Oh, you're back.", "The outlet, remember?" },
        new[] { "You got this!" }
    };

    private Transform _player;
    private Vector2 _interactionCenter;
    private string[] _lines;
    private int _currentLine;
    private bool _dialogueActive;
    private bool _autoShown;
    private bool _isTyping;

    private GameObject _bubble;
    private TextMeshProUGUI _text;
    private Coroutine _typingRoutine;

    void Start()
    {
        // Calculate interaction center from tilemap bounds (works even though transform is at origin)
        var tilemap = GetComponent<Tilemap>();
        if (tilemap != null)
        {
            tilemap.CompressBounds();
            var bounds = tilemap.localBounds;
            _interactionCenter = transform.TransformPoint(bounds.center);
        }
        else
        {
            _interactionCenter = transform.position;
        }

        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        RefreshLines();
        BuildBubble();
        _bubble.SetActive(false);
    }

    void Update()
    {
        // Player may start inactive — keep looking until found
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (_player == null) return;
        }

        var dist = Vector2.Distance(_interactionCenter, _player.position);
        var inRange = dist <= interactionRange;

        // After 3 interactions: auto-show "Go!" without pressing E
        if (TimesInteracted >= 3)
        {
            if (inRange && !_autoShown)
            {
                _autoShown = true;
                RefreshLines();
                StartDialogue();
                StartCoroutine(AutoHide(1.5f));
            }
            if (!inRange)
            {
                _autoShown = false;
                if (_dialogueActive) CloseBubble();
            }
            return;
        }

        var interact = UnityEngine.Input.GetKeyDown(KeyCode.E);
        if (MobileControls.Instance != null)
            interact = interact || MobileControls.Instance.InteractDown;

        if (!_dialogueActive)
        {
            if (inRange && interact)
                StartDialogue();
        }
        else
        {
            if (interact)
            {
                if (_isTyping)
                    SkipTyping();
                else
                    NextLine();
            }

            // Close if player walks away
            if (!inRange) CloseBubble();
        }
    }

    private void RefreshLines()
    {
        _lines = TimesInteracted >= 3
            ? new[] { "Go!" }
            : Dialogues[Mathf.Min(TimesInteracted, Dialogues.Length - 1)];
    }

    private void StartDialogue()
    {
        _currentLine = 0;
        _dialogueActive = true;
        _bubble.SetActive(true);
        ShowLine(0);
    }

    private void ShowLine(int index)
    {
        if (_typingRoutine != null) StopCoroutine(_typingRoutine);
        _typingRoutine = StartCoroutine(TypeText(_lines[index]));
    }

    private void SkipTyping()
    {
        if (_typingRoutine != null) StopCoroutine(_typingRoutine);
        _typingRoutine = null;
        _text.text = _lines[_currentLine];
        _isTyping = false;
    }

    private void NextLine()
    {
        _currentLine++;
        if (_currentLine >= _lines.Length)
        {
            TimesInteracted++;
            CloseBubble();
            RefreshLines();
        }
        else
        {
            ShowLine(_currentLine);
        }
    }

    private void CloseBubble()
    {
        if (_typingRoutine != null) StopCoroutine(_typingRoutine);
        _typingRoutine = null;
        _dialogueActive = false;
        _isTyping = false;
        _text.text = "";
        _bubble.SetActive(false);
    }

    private IEnumerator AutoHide(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_dialogueActive) CloseBubble();
    }

    private IEnumerator TypeText(string line)
    {
        _isTyping = true;
        _text.text = "";
        foreach (var c in line)
        {
            _text.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        _isTyping = false;
        _typingRoutine = null;
    }

    private void BuildBubble()
    {
        // World-space canvas positioned above Compy's tile center
        _bubble = new GameObject("DialogueBubble");
        _bubble.transform.position = new Vector3(_interactionCenter.x, _interactionCenter.y + bubbleYOffset, -1f);

        var canvas = _bubble.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingLayerName = "Player";
        canvas.sortingOrder = 100;

        var rt = _bubble.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 50);
        rt.localScale = new Vector3(bubbleScale, bubbleScale, 1f);

        // Panel — auto-sizes to fit text via ContentSizeFitter
        var panelGo = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelGo.transform.SetParent(_bubble.transform, false);

        var panelRt = panelGo.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0.5f, 0.5f);
        panelRt.anchorMax = new Vector2(0.5f, 0.5f);
        panelRt.pivot = new Vector2(0.5f, 0f);
        panelRt.anchoredPosition = Vector2.zero;

        var img = panelGo.GetComponent<Image>();
        img.color = BgColor;
        img.raycastTarget = false;

        var layout = panelGo.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(5, 5, 3, 3);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        var fitter = panelGo.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Text element
        var textGo = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer));
        textGo.transform.SetParent(panelGo.transform, false);

        _text = textGo.AddComponent<TextMeshProUGUI>();

        // Use assigned font or fall back to Electronic Highway Sign from Resources
        if (pixelFont != null)
            _text.font = pixelFont;
        else
            _text.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Electronic Highway Sign SDF");

        _text.fontSize = fontSize;
        _text.color = (Color)TextColor;
        _text.alignment = TextAlignmentOptions.Center;
        _text.enableWordWrapping = false;
        _text.overflowMode = TextOverflowModes.Overflow;
        _text.raycastTarget = false;
        _text.text = "";
    }
}
