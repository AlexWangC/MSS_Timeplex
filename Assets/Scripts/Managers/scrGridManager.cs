using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//shall be placed at the object with gridmaker
public class scrGridManager : MonoBehaviour
{
    private GridObject[] allGridObjects;

    private void Start()
    {
        initializeGridObjects();
        //printAllGridObjects();
    }

    /*
     * Two options for the following method:
     * 1. Check if that position specifically has any object
     * 2. Check if that position has any object of a certain type
     */
    public bool CheckForObjectAtGridPosition(Vector2Int position)
    {
        foreach (GridObject obj in allGridObjects)
        {
            // Check if the object's grid position matches the target position and has the specified tag
            if (obj.gridPosition == position)
            {
                print("hay something" + " at " + position);
                return true;
            }
            
        }
        return false; 
    }
    
    public bool CheckForObjectAtGridPosition(Vector2Int position, string tag)
    {
        foreach (GridObject obj in allGridObjects)
        {
            // Check if the object's grid position matches the target position and has the specified tag
            if (obj.gridPosition == position && obj.CompareTag(tag))
            {
                print("hay " + tag + " at " + position);
                return true;
            }
            
        }
        return false; 
    }
    
    //checks if a position on the grid with this script attached to is out of bound
    public bool OutOfBound(Vector2Int position)
    {
        if (position.x > this.GetComponent<scrGridMakerTilted>().numBlocksX ||
            position.x < 0 ||
            position.y > this.GetComponent<scrGridMakerTilted>().numBlocksY ||
            position.y < 0)
        {
            return true;
        }

        return false;
    }

    public GridObject GetGridObjectAtPosition(Vector2Int position)
    {
        foreach (GridObject obj in allGridObjects)
        {
            // Check if the object's grid position matches the target position and has the specified tag
            if (obj.gridPosition == position)
            {
                return obj;
            }
            
        }

        return null;
    }

    public GridObject[] GetGridObjectsAtPosition(Vector2Int position) // not sure if it works..
    {
        initializeGridObjects(); // current idea is for performance, only refresh when needed to
        
        List<GridObject> obj_list = new List<GridObject>();
        foreach (GridObject obj in allGridObjects)
        {
            if (obj.gridPosition == position)
            {
                obj_list.Add(obj);
            }
        }

        //debugging
        /*
        Debug.Log("GridObjects at position " + position.x + ", " + position.y);
        foreach (GridObject obj in obj_list.ToArray())
        {
            Debug.Log("a " + obj.tag);
        }

        if (obj_list.Count == 0)
        {
            Debug.Log("No grid object at position");
        }
        */
        //debugging
        
        return obj_list.ToArray();
    }
    
    private void initializeGridObjects()
    {
        allGridObjects = GetComponentInParent<scrPanel>().GetComponentsInChildren<GridObject>();
    }

    private void printAllGridObjects()
    {
        Debug.Log("Showing all grid objects...");
        foreach (var grid_object in allGridObjects)
        {
            Debug.Log("hay " + grid_object.name + " at " + grid_object.gridPosition.x + ", " + grid_object.gridPosition.y);
        }
    }
}
