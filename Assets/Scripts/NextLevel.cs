using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextLevel : MonoBehaviour
{
    // Start is called before the first frame update
    private Scene _scene;
    private void Awake()
    {
        _scene = SceneManager.GetActiveScene();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("player"))
        {
            SceneManager.LoadScene(_scene.buildIndex + 1);
        }
    }

    public void StartLevel()
    {

        SceneManager.LoadScene(_scene.buildIndex + 1);
    }



}
