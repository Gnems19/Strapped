using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBossAreaScript : MonoBehaviour
{
    public CameraController cameraController; // Assign this in Inspector
    public int targetSize = 3; // Zoom value for boss area
    public float transitionTime = 1f; // Duration of zoom

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered Boss Area");
            if (cameraController != null)
            {
                Debug.Log("Calling ZoomTo()");
                cameraController.ZoomTo(targetSize, transitionTime);
            }
            else
            {
                Debug.LogWarning("CameraController reference is null.");
            }
        }
    }
}
