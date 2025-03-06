using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.ComponentModel;
using System.Diagnostics;

public class movementHistory : MonoBehaviour
{
    /*
    public Dictionary<int, GridObject.Data> history = new Dictionary<int, GridObject.Data>();
    GridObject gridobject;
    Dictionary<string, bool> inventory;

    void Awake()
    {
        gridobject = GetComponent<GridObject>();
        inventory = GetComponent<scrInventory>().inventory;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    // Mushroom - record into Dictionary of Gridobject.Data Class; Use provided turnValue for index
    /*
    public void recordMovement(int turnValue)
    {
        GridObject.Data data = gridobject.GetData();
        history.Add(turnValue, data);
        history[turnValue].inventory = inventory;

        UnityEngine.Debug.Log(data.position);
    }
    public void undoMovement()
    {
        //DO NO TAKE -1 FROM turnValue! - recording happens **BEFORE** a move
        //Set grid location of parent to history.position
        //set inventory value to history.inventory
        //Refesh parent gridposition
        //refresh inventory scripts
    }
    public void redoMovement()
    {
        //Add +1 to turnValue
        //
    }
    */

}