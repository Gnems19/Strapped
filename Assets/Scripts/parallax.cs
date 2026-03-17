using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Parallax : MonoBehaviour
{
    public Camera cam; // 0 
    public Transform subject;  // 11
    private Vector2 _startPosition; // 0
    private float _startZ;
    public PixelPerfectCamera pixelPerfect;
    // pixel grid is


    private Vector2 Travel => (Vector2)cam.transform.position - _startPosition; // 2d vector is for the jump parallax

    private float ZDifferenceFromSubject => transform.position.z - subject.position.z; // 39

    private float Normalizer => ((ZDifferenceFromSubject > 0 ? cam.farClipPlane - subject.position.z : subject.position.z)); // -10 + 50 = 40

    private float parallaxFactor => (ZDifferenceFromSubject) / (Normalizer);
    // Start is called before the first frame update
    public void Start()
    {
        _startPosition = transform.position;
        _startZ = transform.position.z;
    }
    // Update is called once per frame
    public void Update()
    {
        // Debug.Log($"Subject: {subject.position.z}");
        // Debug.Log($"distanceFromSubject: {distanceFromSubject}");
        // Debug.Log($"clippingPlane: {clippingPlane}");
        // Debug.Log($"parallaxFactor: {parallaxFactor}");
        // Debug.Log($"start z: {startZ}");

        var newPos = _startPosition + Travel * parallaxFactor;
        if (pixelPerfect)
        {
            float ppu = pixelPerfect.assetsPPU;
            newPos.x = Mathf.Round(newPos.x * ppu) / ppu;
            newPos.y = Mathf.Round(newPos.y * ppu) / ppu;
        }
        transform.position = new Vector3(newPos.x, newPos.y, _startZ);


    }

}
