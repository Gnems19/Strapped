using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    // list of objects that will be detected
    public List<GameObject> DetectedObjects;
    [SerializeField] Logger logger;
    // Start is called before the first frame update
    private void Awake()
    {
        logger.Log("Awake called");
        DetectedObjects = new List<GameObject>();
    }
    // when object enters the collider it will be added to the list
    private void OnTriggerEnter2D(Collider2D other)
    {
        // debug
        logger.Log("OnTriggerEnter2D called");
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedObjects.Add(other.gameObject);
        }
    }
    // when object leaves the collider it will be removed from the list
    private void OnTriggerExit2D(Collider2D other)
    {
        //debug
        logger.Log("OnTriggerExit2D called");
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedObjects.Remove(other.gameObject);
        }
    }
}
