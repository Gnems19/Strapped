using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    public int gameStartSceneID;
    public void StartGame()
    {
        SceneManager.LoadScene(gameStartSceneID);  
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
