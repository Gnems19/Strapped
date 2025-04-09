using InteractableItemsScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class BossController : MonoBehaviour, IBossController
    {
        // Start is called before the first frame update
        public bool IsDead { get; private set; }
        public bool IsSpecialAttacking { get; private set; }

        public bool IsSpeaking { get; private set; }
    
        // get the power outlet GameObject
        private IPowerOutlet _powerOutlet;
    
        [SerializeField] private GameObject player;

        private void Awake() => _powerOutlet = GetComponentInChildren<IPowerOutlet>();

        // Update is called once per frame
        void Update()
        {
            if (transform.position.x < player.transform.position.x)
            {
                IsSpeaking = false;
                IsSpecialAttacking = true;
            }
            if(!_powerOutlet.PluggedIn)
            {
                IsDead = true;
            }
        }
    }
}
