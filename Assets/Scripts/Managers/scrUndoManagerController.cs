using System;
using UnityEngine;

public class scrUndoManagerController : MonoBehaviour
{
    public PauseMenuManager pauseMenuManager;

    void Start()
    {
        pauseMenuManager = FindAnyObjectByType<PauseMenuManager>();
    }
    private void Update()
    {
        if (pauseMenuManager.isPaused) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // if you can retrace..
            if (FindAnyObjectByType<scrUndoManager>().UndoAvailable)
            {
                GetComponent<scrUndoManager>().Retrace();
                // will be activated when pickuppable update functionality is done.
                //GetComponent<scrUndoManager>().RetracePickuppable();
            }
        }
    }
}