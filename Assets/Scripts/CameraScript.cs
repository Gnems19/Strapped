using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public float left_border = 1f;
    public float right_border = 1000f;

    public Vector3 offset;


    // Update is called once per frame
    public void Update()
    {
        Vector3 desiredPosition = target.position + offset;
        if (desiredPosition.x < left_border)
        {
            desiredPosition.x = left_border;
        }
        else if (desiredPosition.x > right_border)
        {
            desiredPosition.x = right_border;
        }
        transform.position = new Vector3(desiredPosition.x, transform.position.y, transform.position.z);
    }
}
