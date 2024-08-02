using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public GameManager gameManager;
    public Animator transition;
    private static SceneManager instance;
    public GameObject image;
    public static int gameStateForCursor = 0;

    private void Update()
    {
        if (gameStateForCursor == 1)
        {
            ToggleCursor();
        }
    }

    public void StartGame()
    {
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int sceneIndex)
    {
        image.GetComponent<Image>().color = Color.black;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        AudioManager.instance.PlayMusic(AudioManager.instance.battleMusic);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        transition.SetTrigger("End");
        gameStateForCursor = 0;
    }

    IEnumerator LoadMenuLevel(int sceneIndex)
    {
        image.GetComponent<Image>().color = Color.black;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        transition.SetTrigger("End");
        gameStateForCursor = 0;
    }

    IEnumerator LoadLevelWithDeath(int sceneIndex)
    {
        AudioManager.instance.PlayDeath();
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(2);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        transition.SetTrigger("End");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        image.GetComponent<Image>().color = Color.red;
        StartCoroutine(LoadLevelWithDeath(0));
        gameStateForCursor = 1;
    }

    public void RestartGameMenu()
    {
        image.GetComponent<Image>().color = Color.black;
        StartCoroutine(LoadMenuLevel(0));
        gameStateForCursor = 1;
    }

    public void ToggleSfxBack()
    {
        AudioManager.instance.ToggleSfx();
    }

    public void ToggleCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }
}
