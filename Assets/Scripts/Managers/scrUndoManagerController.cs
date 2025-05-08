using System;
using UnityEngine;

public class scrUndoManagerController : MonoBehaviour
{
    private void Update()
    {
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