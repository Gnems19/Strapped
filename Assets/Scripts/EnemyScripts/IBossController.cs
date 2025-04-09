namespace EnemyScripts
{
    public interface IBossController
    {
    
        public bool IsDead { get; }
        public bool IsSpecialAttacking { get; }
    
        public bool IsSpeaking { get; }
    }
}