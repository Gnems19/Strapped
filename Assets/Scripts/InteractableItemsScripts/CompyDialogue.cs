using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace InteractableItemsScripts
{
    public class CompyDialogue : MonoBehaviour
    {
        private class DialogueIterator
        {
            private readonly string[][] _dialogues;
            private int _nextIndex;
            private int _deathsAtLastYield;
            private readonly int _deathGap;

            public DialogueIterator(string[][] dialogues, int initialDeathThreshold = -1, int deathGap = 1)
            {
                _dialogues = dialogues;
                _deathsAtLastYield = initialDeathThreshold;
                _deathGap = deathGap;
            }

            public bool HasNext => _nextIndex < _dialogues.Length
                                   && PlayerDeathController.TotalDeathCount >= _deathsAtLastYield + _deathGap;

            public string[] Current => _dialogues[_nextIndex];

            public bool IsExhausted => _nextIndex >= _dialogues.Length;

            public void Complete()
            {
                _deathsAtLastYield = PlayerDeathController.TotalDeathCount;
                _nextIndex++;
            }
        }

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

        private static readonly string[][] QuantumDialogues =
        {
            new []{"...I hope"},
            new[] {
                "Hey... can I tell you something?",
                "I've been watching you.",
                "Not just here. Across realities.",
                /*"I'm a quantum computer. I see them all.",
                "Every death, every restart.",
                "But you keep going.",
                "Make your consciousness proud."*/
            },
            new[] {
                /*
                "Ok, at this point you must know what you're doing.",
                */
                "It seems like...",
                "Your consciousness keeps its memories\nthrough the multiverse.",
                "So next life, right after you wake up,\nstart running. Don't stop.",
                "Some things are easier than they seem.",
                "You just have to believe."
            }
        };

        // -1 = first dialogue available immediately (0 deaths)
        private static readonly DialogueIterator _normalIter = new DialogueIterator(Dialogues, -1);
        // 9 = quantum unlocks when deaths > 9, i.e. 10+
        private static readonly DialogueIterator _quantumIter = new DialogueIterator(QuantumDialogues, 9, 69);

        private DialogueIterator _activeIter;
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
            BuildBubble();
            _bubble.SetActive(false);
        }

        void Update()
        {
            // Player may start inactive — keep looking until found
            if (!_player)
            {
                _player = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (!_player) return;
            }

            var dist = Vector2.Distance(_interactionCenter, _player.position);
            var inRange = dist <= interactionRange;

            // Pick the active iterator: normal first, then quantum, then "Go!" fallback
            var activeIter = !_normalIter.IsExhausted ? _normalIter
                : !_quantumIter.IsExhausted ? _quantumIter
                : null;

            // Iterator has a dialogue ready — press E to interact
            if (activeIter != null && activeIter.HasNext)
            {
                var interact = UnityEngine.Input.GetKeyDown(KeyCode.E);
                if (MobileControls.Instance)
                    interact = interact || MobileControls.Instance.InteractDown;

                if (!_dialogueActive)
                {
                    if (inRange && interact)
                    {
                        _lines = activeIter.Current;
                        _activeIter = activeIter;
                        StartDialogue();
                    }
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
                    if (!inRange) CloseBubble();
                }
                return;
            }

            // All dialogues exhausted or waiting for deaths — auto-show "Go!"
            if (_normalIter.IsExhausted)
            {
                if (inRange && !_autoShown)
                {
                    _autoShown = true;
                    _lines = new[] { "Go!" };
                    StartDialogue();
                    StartCoroutine(AutoHide(1.5f));
                }
                if (!inRange)
                {
                    _autoShown = false;
                    if (_dialogueActive) CloseBubble();
                }
            }
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
                _activeIter?.Complete();
                _activeIter = null;
                CloseBubble();
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
            _text.font = pixelFont != null ? pixelFont : Resources.Load<TMP_FontAsset>("Fonts & Materials/Electronic Highway Sign SDF");

            _text.fontSize = fontSize;
            _text.color = (Color)TextColor;
            _text.alignment = TextAlignmentOptions.Center;
            _text.enableWordWrapping = false;
            _text.overflowMode = TextOverflowModes.Overflow;
            _text.raycastTarget = false;
            _text.text = "";
        }
    }
}
