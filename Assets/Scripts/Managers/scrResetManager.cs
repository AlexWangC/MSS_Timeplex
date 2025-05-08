using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrResetManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextScene();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            PreviousScene();
        }
    }

    public void UpdateResetStatus() // call this from the outside
    {
        /*
        if (checkIfAllDead())
        {
            // Get the current scene's index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Reload the current scene
            SceneManager.LoadScene(currentSceneIndex);
        }
        */
    }

    public void Reset()
    {
        // Get the current scene's index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex); 
    }

    public void NextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void PreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex - 1);
    }
    
    private bool checkIfAllDead()
    {
        foreach (scrPanel panel in FindObjectsByType<scrPanel>(FindObjectsSortMode.None))
        {
            if (panel.Dead == false)
            {
                return false;
            }
        }

        return true;
    }
}
