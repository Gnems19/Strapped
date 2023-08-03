using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam; // 0 
    public Transform subject;  // 11
    private Vector2 startPosition; // 0
    private float startZ;

    private Vector2 travel => (Vector2)cam.transform.position - startPosition; // 2d vector is for the jump parallax

    private float zDifferenceFromSubject => transform.position.z - subject.position.z; // 39

    private float normalizer => ((zDifferenceFromSubject > 0 ? cam.farClipPlane - subject.position.z : subject.position.z)); // -10 + 50 = 40

    private float parallaxFactor => (zDifferenceFromSubject) / (normalizer);
    // Start is called before the first frame update
    public void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }
    // Update is called once per frame
    public void Update()
    {
        // Debug.Log($"Subject: {subject.position.z}");
        // Debug.Log($"distanceFromSubject: {distanceFromSubject}");
        // Debug.Log($"clippingPlane: {clippingPlane}");
        // Debug.Log($"parallaxFactor: {parallaxFactor}");
        // Debug.Log($"start z: {startZ}");
        
        Vector2 newPos = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }
}
