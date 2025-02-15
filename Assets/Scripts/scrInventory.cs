using System;
using DialogueSystem;
using UnityEngine;

public class scrInventory : MonoBehaviour
{
    public bool checked_already;

    public DialogueDisplayer dd;

    private void Start()
    {
        checked_already = false;
        dd.onLineChanged += onLineUpdate;
    }

    // subscribe to onLineChanged 
    private void onLineUpdate(string oldLineId, string newLineId)
    {
        if (newLineId == "0") checked_already = true;
    }
}
