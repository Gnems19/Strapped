using UnityEngine;

namespace InteractableItemsScripts
{
    public class PowerOutlet : MonoBehaviour, IPowerOutlet
    {
    
        // the collider for the power outlet
        private Collider2D _collider2D;
        private bool _canPullOut;
        public bool PluggedIn { get; private set; }

        // get the player GameObject
        [SerializeField] GameObject player;
    
        void Start()
        {
            PluggedIn = true;
            _canPullOut = false;
            _collider2D = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (_canPullOut && Input.GetKeyDown(KeyCode.E))
            {
                _collider2D.enabled = false;
                PluggedIn = false;
            }
        }

        // if player collides with power outlet, set can turn off to true
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == player)
            {
                _canPullOut = true;
            }
        }
    
        // if player leaves power outlet, set can turn off to false
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject == player)
            {
                _canPullOut = false;
            }
        }

    
    }
}
