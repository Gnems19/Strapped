using UnityEngine;
using UnityEngine.SceneManagement;

namespace InteractableItemsScripts
{
    public class DoorScript : MonoBehaviour
    {
        // get player object
        public GameObject player;
        // get sprites for opend and closed door
        [SerializeField] Sprite openedDoor;
        [SerializeField] Sprite closedDoor;
        [SerializeField] Logger logger;
        [SerializeField] int sceneToLoad;
        private bool _collide;
        private float _timer;
        private float _duration;
    
        // Start is called before the first frame update
        void Start()
        {
            _collide = false;
            _timer = 0f; 
            _duration = 2f;
            // set sprite to closed door
            GetComponent<SpriteRenderer>().sprite = closedDoor;
            //check if called by logging
            logger.Log("DoorScript Start called");
        }

        private void FixedUpdate()
        {
            //if the collision is grater than 1 second change scene to 2
            if(_collide)
            {
                _timer+=Time.deltaTime;
                if (_timer >= _duration)
                {
                    SceneManager.LoadScene(sceneToLoad); 
                }
            }
        }

        // if player comes to the door changes sprite to opened door
        private void OnTriggerEnter2D(Collider2D other)
        {
            // check if called by logging
            if (logger != null) logger.Log("OnTriggerEnter2D called");
            if (other.gameObject == (player))
            {
                GetComponent<SpriteRenderer>().sprite = openedDoor;
                if(!_collide)
                    _collide = true;
            }
        }
    

        // if the player leaves the door it closes
        private void OnTriggerExit2D(Collider2D other)
        {
            // check if called by logging
            if (logger != null) logger.Log("OnTriggerExit2D called");

            if (other.gameObject == (player))
            {
                GetComponent<SpriteRenderer>().sprite = closedDoor;
            }
        
            _collide = false;
            _timer = 0;
        }
    
    
    }
}
