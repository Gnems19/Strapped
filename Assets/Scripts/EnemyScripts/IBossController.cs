using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBossController
{
    
    public bool IsDead { get; }
    
    public bool IsAttacking { get; }
    public bool IsSpecialAttacking { get; }

    public bool IsMoving { get; }
    public bool IsStunned { get; }
    public bool IsIdle { get; }
    
    public bool IsSpeaking { get; }
    
}
