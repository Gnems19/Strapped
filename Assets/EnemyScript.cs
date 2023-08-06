using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // get the detection object
    public DetectionZone detectionZone;
    // get the animator
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        // get the detection object
        detectionZone = GetComponentInChildren<DetectionZone>();
        // get the animator
        animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    
    void Update()
    {
        // check if someone is in the list and attack if so
        if (detectionZone.DetectedObjects.Count > 0)
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
        
    }
}
