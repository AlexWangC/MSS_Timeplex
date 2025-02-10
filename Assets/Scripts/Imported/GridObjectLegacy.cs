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

[ExecuteInEditMode]
public class GridObjectLegacy : MonoBehaviour
{
    public Vector2 gridPosition;
    private Vector2 prevGridPosition;

    [HideInInspector]public GridMaker parentGrid; // Jingxing's mod. Using inheritance to get the corresponding grid.

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
        float x = parentGrid.TopLeft.x + parentGrid.cellWidth * (gridPosition.x - 0.5f);
        float y = parentGrid.TopLeft.y - parentGrid.cellWidth * (gridPosition.y - 0.5f);
        this.transform.position = new Vector3(x, y, 0);
    }

    private GridMaker getParentGrid()
    {
        scrPanel parent_panel = GetComponentInParent<scrPanel>();
        parentGrid = parent_panel.GetComponentInChildren<GridMaker>();
        
        return parentGrid;
    }
}
