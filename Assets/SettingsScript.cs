using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    public int gameMenuSceneID;
    
    public void BackToMenu()
    {
        SceneManager.LoadScene(gameMenuSceneID);
    }
}
