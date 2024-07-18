using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class menucontroler : MonoBehaviour
{

    public static bool Gameispaused = false;
    public GameObject Buttons;
    public GameObject start;
    public GameObject Panel;
    public GameObject Options;
    public AudioSource theme;

  

    void Update()

    {
       

        if (Input.GetKeyDown(KeyCode.Escape))
            if (Gameispaused)
            {
                Resume();
               
            }
            else{
            Pause();
        }

    }



    public void Resume()
    {
        Buttons.SetActive(false);
        Panel.SetActive(false);
        Time.timeScale = 1f;
        Gameispaused = false;
    }

    public void Pause()
    {
        Buttons.SetActive(true);
        Panel.SetActive(true);
        Time.timeScale = 0f;
        Gameispaused = true;
    } 
    public void LoadScene()
    {
        SceneManager.LoadScene("start");
    }
    public void ShowOptions()
    {
        Buttons.SetActive(false);
        Options.SetActive(true);
     
        Gameispaused = true;

    }
    public void SetQuality(int qual)
    {
        QualitySettings.SetQualityLevel(qual);
    }
    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
   
    public void SetMusic(bool isMusic)
    {
      theme.mute = !isMusic;
    }
   public void Quit()
        
    {
        Application.Quit();
    }

    public void ShowSettings()
    {
        start.SetActive(false);
        Buttons.SetActive(true);

        Gameispaused = true;


    }

}
