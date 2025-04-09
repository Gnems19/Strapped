using UnityEngine;

namespace EnemyScripts
{
    public class BossAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private IBossController _boss;
        private static readonly int Sleeping = Animator.StringToHash("Sleeping");
        private static readonly int PlugedIn = Animator.StringToHash("PluggedIn");
        private static readonly int LaunchingMissiles = Animator.StringToHash("LaunchingMissiles");

        private void Awake() => _boss = GetComponentInParent<IBossController>();
    
        public void Update()
        {
            if (_boss.IsDead)
            {
                animator.SetBool(Sleeping, true);
                animator.SetBool(PlugedIn, false);
            } else if (_boss.IsSpecialAttacking)
            {
                animator.SetBool(LaunchingMissiles, true);
                // TODO: stop speaking
            } else if (_boss.IsSpeaking)
            {
                // TODO: add speaking animation
            }

        }
    }
}
