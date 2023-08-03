using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public int gameStartSceneID;
    
    public void StartGame()
    {
        SceneManager.LoadScene(gameStartSceneID);  
    }
    
}
