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
using Fries;

[ExecuteInEditMode]
public class GridObject : MonoBehaviour
{
    /* mush's code.
    //Class to store information in movementHistory for position inventory and if it is alive or not
    [System.Serializable]
    public class Data
    {
        public Vector2 position;
        public Dictionary<string, bool> inventory;
        public bool alive;
    }
    */

    public bool updated = false; // mark this as true if you want it to be reset in movement History.
    public Vector2 gridPosition;
    
    private SpriteRenderer sr; // part of temporalProjectionFix

    [HideInInspector] public scrGridMakerTilted parentGrid; // Jingxing's mod. Using inheritance to get the corresponding grid.

    [HideInInspector] public bool inventoryHasItem;

    private bool isPlayer = false;
    
    private void Start()
    {
        var pl = gameObject.getComponent<scrPlayer>();
        if (pl) isPlayer = true;
        
        getParentGrid();
        
        temporalProjectionFixStart();
        
        //mightbug for Text UI with grid position, need exception
    }

    private bool isFirstRun = true;
    private void Update()
    {
        //Move to the new position
        UpdatePosition();
        if (isFirstRun) {
            isFirstRun = false;
            scrMoveInheritanceManager.invokeOnPlayerMove(this.getComponent<scrPlayer>(), this);
        }
    }

    [Button("Update Position")]
    public void UpdatePosition()
    {
        //print(parentGrid);
        //print(gridPosition);
        if (parentGrid == null)
        {
            Debug.Log("got a null parent grid", gameObject);
        }
        this.transform.position = parentGrid.GetWorldPositionFromGrid(gridPosition);
        
        temporalProjectionFixUpdate();
        
        //Debug.Log("Object at " + parentGrid.GetWorldPositionFromGrid(gridPosition) + "projected successfully.");
    }

    

    public scrGridMakerTilted getParentGrid()
    {
        scrPanel parent_panel = GetComponentInParent<scrPanel>();
        parentGrid = parent_panel.GetComponentInChildren<scrGridMakerTilted>();
        
        return parentGrid;
    }

    private void temporalProjectionFixStart()
    {
        // temporal projection fix
        sr = GetComponent<SpriteRenderer>();
        if(sr.sortingLayerName != "tiles")
            sr.sortingLayerName = "objects";
        // temporal projection fix
    }

    private void temporalProjectionFixUpdate()
    {
        if(sr!= null)
        {
            sr.sortingOrder = Mathf.RoundToInt(gridPosition.y * 100 - gridPosition.x);
        }
    }
    
    /*mush's code
    // record information of gridposition, alive state and inventory state
    public Data GetData()
    {
        Data data = new Data();
        data.position = gridPosition;
        // data.alive 
        // data.inventory
        return data;
    }
    */
}
