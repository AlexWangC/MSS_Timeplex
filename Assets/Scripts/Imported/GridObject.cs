// Code by Danny Hawk. 
// Edited by Jingxing
/*
 * Comments:
 *      Always place the object with this script AS A CHILDREN of a GridMaker!!
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Specialized;

[ExecuteInEditMode]
public class GridObject : MonoBehaviour
{
    // mush's code.
    //Class to store information in movementHistory for position inventory and if it is alive or not
    [System.Serializable]
    public class Data
    {
        public Vector2 position;
        public Dictionary<string, bool> inventory;
        public bool alive;
    }

    public Vector2 gridPosition;
    private Vector2 prevGridPosition;

    [HideInInspector] public scrGridMakerTilted parentGrid; // Jingxing's mod. Using inheritance to get the corresponding grid.

    [HideInInspector] public bool inventoryHasItem;
    
    private void Start()
    {
        getParentGrid();
    }

    private void Update()
    {
        //If our position hasn't been updated, don't 
        if (gridPosition == prevGridPosition)
            return;

        //Move to the new position
        UpdatePosition();

        //Keep track of our previous position
        prevGridPosition = gridPosition;
    }

    [Button("Update Position")]
    public void UpdatePosition()
    {
        //print(parentGrid);
        //print(gridPosition);
        this.transform.position = parentGrid.GetWorldPositionFromGrid(gridPosition);
        
        //Debug.Log("Object at " + parentGrid.GetWorldPositionFromGrid(gridPosition) + "projected successfully.");
    }

    

    private scrGridMakerTilted getParentGrid()
    {
        scrPanel parent_panel = GetComponentInParent<scrPanel>();
        parentGrid = parent_panel.GetComponentInChildren<scrGridMakerTilted>();
        
        return parentGrid;
    }
    
    //mush's code
    // record information of gridposition, alive state and inventory state
    public Data GetData()
    {
        Data data = new Data();
        data.position = gridPosition;
        // data.alive 
        // data.inventory
        return data;
    }
}
