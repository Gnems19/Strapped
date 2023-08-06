using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // get player object
    public GameObject player;
    // get sprites for opend and closed door
    public Sprite openedDoor;
    public Sprite closedDoor;

    // Start is called before the first frame update
    void Start()
    {
        // set sprite to closed door
        GetComponent<SpriteRenderer>().sprite = closedDoor;
        //check if called by logging
        Debug.Log("Start called");
    }

    private void Update()
    {
        
        // transform.position = new Vector3(transform.position.x -1 , transform.position.y, transform.position.z);
        // Debug.Log("Update called");
    }

    // if player comes to the door changes sprite to opened door
    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if called by logging
        Debug.Log("OnTriggerEnter2D called");
        if (other.gameObject == (player))
        {
            GetComponent<SpriteRenderer>().sprite = openedDoor;
        }
    }
    

    // if the player leaves the door it closes
    private void OnTriggerExit2D(Collider2D other)
    {
        // check if called by logging
        Debug.Log("OnTriggerExit2D called");
        
        if (other.gameObject == (player))
        {
            GetComponent<SpriteRenderer>().sprite = closedDoor;
        }
    }
    
    
}
