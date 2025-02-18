using System;
using UnityEngine;

public class scrGoal : MonoBehaviour
{
    //public bool MovementLock; // set whether after reaching this goal locks player movement in this scene. NOT YET IMPLEMENTED
    public bool Reached;
    public bool Locked = false;
    public int DoorType = -1; // whether it's corresponding to key 1, 2, or 3. Default -1 means the door's not locked.

    private void Start()
    {
        Reached = false;
    }

    private void Update()
    {
    }

    // i think this is the problem. Refactor it first?
    public bool checkIfReached()
    {
        scrGridManager grid_manager = transform.parent.gameObject.GetComponentInChildren<scrGridManager>();
        
        GridObject[] objects_at_this_position = grid_manager.GetGridObjectsAtPosition(toVector2Int(GetComponent<GridObject>().gridPosition)); // getting all objects at this scrGoal's location
        if (objects_at_this_position.Length >= 2)
        {
            foreach (GridObject obj in objects_at_this_position)
            {
                if (obj.CompareTag("player"))
                {
                    Reached = true;
                    return true;
                }
            }
        }
        
        Reached = false;
        return false;
    }
    
    private Vector2Int toVector2Int(Vector2 vector2) // helper from scr player. Local access for efficiency
    {
        return new Vector2Int((int)vector2.x, (int)vector2.y);
    }
}
