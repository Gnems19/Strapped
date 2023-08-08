using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSliderScript : MonoBehaviour
{
    // add slider serializable field
    [SerializeField] private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(val=>SoundManager.Instance.SetSFXVolume(val));
    }
}
