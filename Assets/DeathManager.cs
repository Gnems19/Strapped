using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    GameObject player;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        // this is a script for player but first we must get the player object
        player = GameObject.FindGameObjectWithTag("Player");
        //debug
        Debug.Log("DeathManager Start called");
        animator = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        //check if the player fell in a pit
        if (player.transform.position.y < -10)
        {
            // if so restart the game
            RestartGame();
        }

    }

    // if the player collides with the enemy eye sight restart the game
    private void OnTriggerEnter2D(Collider2D other)
    {
        //debug
        Debug.Log("OnTriggerEnter2D called");
        //if the other is enemy EnemySight play death animation and wait for it to finish and restart the game
        if (other.gameObject.CompareTag("EnemySight"))
        {
            animator.SetBool("isDead", true);
            Invoke("RestartGame", 1);
        }
    }
    void RestartGame()
    {
        // restart the game
        SceneManager.LoadScene(1);
    }
}
