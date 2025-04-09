using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    // toggle this to enable/disable logging
    [SerializeField] bool loggingEnabled = true;
    // log a message
    public void Log(string message)
    {
        if (loggingEnabled)
        {
            Debug.Log(message);
        }
    }
}
