using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // get the detection object
    public DetectionZone detectionZone;
    // get the animator
    public Animator animator;

    private float _startX;
    // Start is called before the first frame update
    void Start()
    {
        // get the detection object
        detectionZone = GetComponentInChildren<DetectionZone>();
        // get the animator
        animator = GetComponent<Animator>();
        
        _startX = transform.position.x;
    }
    
    // Update is called once per frame
    
    void Update()
    {
        // create enemie AI 
        // if the player is in the detection zone move towards the player
        if (detectionZone.DetectedObjects.Count > 0f)
        {
            animator.SetBool("isAttacking", true);
            // get the player
            GameObject player = detectionZone.DetectedObjects[0];
            // get the player position
            Vector3 playerPosition = player.transform.position;
            // get the enemy position
            Vector3 enemyPosition = transform.position;
            // get the direction to the player
            Vector3 directionToPlayer = playerPosition - enemyPosition;
            // normalize the direction
            directionToPlayer.Normalize();
            // make direction to player 2d only on x
            directionToPlayer = new Vector3(directionToPlayer.x, 0, 0);
            //transform.position += directionToPlayer * (Time.deltaTime * 3);

        }
        else
        {
            animator.SetBool("isAttacking", false);
            if (transform.position.x > _startX + 3)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (transform.position.x < _startX - 3)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            transform.position += (new Vector3(-transform.localScale.x,0,0)) * (Time.deltaTime * 2);
        }
        
        
    }
    
    // if you hit the wall dont go though it
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
        }
    }
    
}
