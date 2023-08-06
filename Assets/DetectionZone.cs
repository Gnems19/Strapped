using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    Collider2D DetectionCollider;
    // list of objects that will be detected
    public List<GameObject> DetectedObjects;
    // Start is called before the first frame update
    private void Awake()
    {
        DetectionCollider = GetComponent<Collider2D>();
        
    }
    // when object enters the collider it will be added to the list
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedObjects.Add(other.gameObject);
        }
    }
    // when object leaves the collider it will be removed from the list
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DetectedObjects.Remove(other.gameObject);
        }
    }
}
