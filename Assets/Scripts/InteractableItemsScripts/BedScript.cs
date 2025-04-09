using UnityEngine;

namespace InteractableItemsScripts
{
    public class BedScript : MonoBehaviour
    {

        public GameObject player;
    
        public void ShowPlayer()
        {
            player.SetActive(true);
        }
   
    }
}
