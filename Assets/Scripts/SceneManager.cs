using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void StartGame() 
    {         
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }

    public void QuitGame() 
    {
        Application.Quit();
    }   
}
