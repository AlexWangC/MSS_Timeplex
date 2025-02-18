using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrResetManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Get the current scene's index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Reload the current scene
            SceneManager.LoadScene(currentSceneIndex); 
        }
    }

    public void UpdateResetStatus() // call this from the outside
    {
        if (checkIfAllDead())
        {
            // Get the current scene's index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Reload the current scene
            SceneManager.LoadScene(currentSceneIndex);
        }
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
