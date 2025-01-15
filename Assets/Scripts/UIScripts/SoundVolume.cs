using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolume : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    // Start is called before the first frame update
    void Start()
    {
        // add a listener to change volume
        scrollbar.onValueChanged.AddListener(val => SoundManager.Instance.SetSFXVolume(val));   
    }
    
}
