using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathController : MonoBehaviour, IPlayerDeathController
{

    [SerializeField] private Logger logger;

    public bool IsDead { get; private set; }

    
    // Start is called before the first frame update
    void Start()
    {
        logger.Log("DeathManager Start called");
        IsDead = false;
        // ignore background collisions
        Physics2D.IgnoreLayerCollision(0,1);
    }
    
    void FixedUpdate()
    {
        //check if the player fell in a pit
        if (gameObject.transform.position.y < -10)
        {
            IsDead = true;
            RestartGame();
        }
        
        
    }

    // if the player collides with the enemy eye sight restart the game
    private void OnTriggerEnter2D(Collider2D other)
    {
        logger.Log("DeathManager OnTriggerEnter2D called");
        //if the other is enemy EnemySight play death animation and wait for it to finish and restart the game
        if (other.gameObject.CompareTag("EnemySight"))
        {
            IsDead = true;
            SoundManager.Instance.PlayPlayerDeathByExplosionSound();
            Invoke("RestartGame", 1.5f);
        }

        if (other.gameObject.CompareTag("HomingMissile"))
        {
            IsDead = true;
            SoundManager.Instance.PlayPlayerDeathByExplosionSound();
            Invoke("RestartGame", 1.5f);
        }
    }
    
    // while the player is in collision OnTriggerStay2D will be called if insstant death ever to be changed... (private float spottedTime = 0f;)

    void RestartGame()
    {
        // restart the game
        SceneManager.LoadScene(1);
    }
}
