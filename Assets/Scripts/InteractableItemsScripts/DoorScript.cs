using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace InteractableItemsScripts
{
    public class DoorScript : MonoBehaviour
    {
        public GameObject player;
        [SerializeField] Sprite openedDoor;
        [SerializeField] Sprite closedDoor;
        [SerializeField] Logger logger;
        [SerializeField] int sceneToLoad;

        private bool _playerInRange;
        private GameObject _prompt;

        void Start()
        {
            GetComponent<SpriteRenderer>().sprite = closedDoor;
            if (logger != null) logger.Log("DoorScript Start called");
            BuildPrompt();
        }

        private void BuildPrompt()
        {
            var isMobile = false;
#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
#endif
            var prefabName = isMobile ? "interactionQueMobile" : "interactionQueKeyboard";
            var prefab = Resources.Load<GameObject>(prefabName);

            if (prefab != null)
            {
                _prompt = Instantiate(prefab, transform);
                _prompt.transform.localPosition = new Vector3(0f, 1.2f, 0f);

                var sg = _prompt.GetComponentInChildren<SortingGroup>();
                if (sg != null)
                {
                    sg.sortingLayerName = "Player";
                    sg.sortingOrder = 100;
                }

                var sr = _prompt.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    if (sg == null)
                    {
                        sr.sortingLayerName = "Player";
                        sr.sortingOrder = 100;
                    }
                    sr.color = new Color(1f, 1f, 1f, 0.6f);
                }
            }
            else
            {
                if (logger != null) logger.Log($"InteractionQue prefab '{prefabName}' not found in Resources");
                _prompt = new GameObject("InteractionQue");
                _prompt.transform.SetParent(transform);
                _prompt.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            }

            _prompt.SetActive(false);
        }

        private void Update()
        {
            if (!_playerInRange) return;

            var interact = Input.GetKeyDown(KeyCode.E);
            if (MobileControls.Instance != null)
                interact = interact || MobileControls.Instance.InteractDown;

            if (interact)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (logger != null) logger.Log("OnTriggerEnter2D called");
            if (other.gameObject == player)
            {
                _playerInRange = true;
                _prompt.SetActive(true);
                GetComponent<SpriteRenderer>().sprite = openedDoor;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (logger != null) logger.Log("OnTriggerExit2D called");
            if (other.gameObject == player)
            {
                _playerInRange = false;
                _prompt.SetActive(false);
                GetComponent<SpriteRenderer>().sprite = closedDoor;
            }
        }
    }
}
