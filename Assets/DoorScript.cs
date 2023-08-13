using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    // get player object
    public GameObject player;
    // get sprites for opend and closed door
    [SerializeField] Sprite openedDoor;
    [SerializeField] Sprite closedDoor;
    [SerializeField] Logger logger;
    bool collide = false;
    float timer = 0;
    float duration = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        // set sprite to closed door
        GetComponent<SpriteRenderer>().sprite = closedDoor;
        //check if called by logging
        logger.Log("DoorScript Start called");
    }

    private void FixedUpdate()
    {
        //if the collision is grater than 1 second change scene to 2
        if(collide)
        {
            timer+=Time.deltaTime;
            if (timer >= duration)
            {
                SceneManager.LoadScene(2); // end game win !!!
            }
        }
    }

    // if player comes to the door changes sprite to opened door
    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if called by logging
        logger.Log("OnTriggerEnter2D called");
        if (other.gameObject == (player))
        {
            GetComponent<SpriteRenderer>().sprite = openedDoor;
            if(!collide)
                collide = true;
        }
        
    }
    

    // if the player leaves the door it closes
    private void OnTriggerExit2D(Collider2D other)
    {
        // check if called by logging
        logger.Log("OnTriggerExit2D called");
        
        if (other.gameObject == (player))
        {
            GetComponent<SpriteRenderer>().sprite = closedDoor;
        }
        collide = false;
        timer = 0;
    }
    
    
}
