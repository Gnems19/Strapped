using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedScript : MonoBehaviour
{

    public GameObject player;
    
    public void ShowPlayer()
    {
        player.SetActive(true);
    }
   
}
