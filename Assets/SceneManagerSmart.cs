using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerSmart : MonoBehaviour
{
    public Animator transition;

    IEnumerator LoadLevel(int sceneIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        AudioManager.instance.PlayMusic(AudioManager.instance.battleMusic);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        transition.SetTrigger("End");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        StartCoroutine(LoadLevel(0));
    }

    public void LoadGameOver()
    {
        StartCoroutine(LoadLevel(2));
    }
}
