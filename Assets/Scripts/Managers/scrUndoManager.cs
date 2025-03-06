using System;
using System.Collections.Generic;
using UnityEngine;

public class scrUndoManager : MonoBehaviour
{
    // for movement history of ONE object
    private struct RecordEntry
    {
        public bool panelSwapMove; // if it is panel swap move, forget about what's below
        
        public Vector2 gridPosition;
        public Dictionary<string, bool> inventory;
        public bool dead;
        public int patrolMoveIndex; // be -1 if not enemy: patrol
        public bool patrolIndexIncreasing; // be false if not enemy: patrol
    }

    // for panel swap history of ONE panel
    private struct PanelRecordEntry
    {
        public int Time_index;
        public Vector3 panelCoord;
    }

    private List<Stack<RecordEntry>> objectsMovementHistory;
    private List<Stack<PanelRecordEntry>> panelsSwapHistory;

    private List<GridObject> gridObjectsWithUpdate;
    private List<scrPanel> panels;
    
    // step one, search for all 
    private void Start()
    {
        gridObjectsWithUpdate = GetAllObjectsWithUpdated();
        panels = GetAllPanels();
        objectsMovementHistory = InitializeMovementHistory();
        panelsSwapHistory = InitializePanelHistory();
    }

    public void Retrace()
    {
        int foreach_counter = 0;
        foreach (Stack<RecordEntry> record_entry in objectsMovementHistory)
        {
            // if it has something to pop... Pop it
            if (record_entry.TryPop(out RecordEntry entry))
            {
                // remember to implement a if panel swap move here
                
                gridObjectsWithUpdate[foreach_counter].gridPosition = entry.gridPosition;
                
                // if the object has an inventory
                if (entry.inventory != null)
                {
                    gridObjectsWithUpdate[foreach_counter].GetComponent<scrInventory>().inventory = entry.inventory;
                    gridObjectsWithUpdate[foreach_counter].GetComponent<scrInventory>().syncActualInventoryWithDictionary();
                }
                
                // Panel Revive check: if the gridObject is player, the panel that contains the player is currently dead, but the previous entry is not dead
                if (gridObjectsWithUpdate[foreach_counter].GetComponentInParent<scrPanel>().Dead && !entry.dead &&
                    gridObjectsWithUpdate[foreach_counter].CompareTag("player"))
                {
                    // revive the panel, should probably call something here too for visuals
                    gridObjectsWithUpdate[foreach_counter].GetComponentInParent<scrPanel>().Dead = false;
                    gridObjectsWithUpdate[foreach_counter].GetComponentInParent<scrPanel>().PanelRevived();
                }
                
                // if it is a patrol
                if (entry.patrolMoveIndex != -1)
                {
                    gridObjectsWithUpdate[foreach_counter].GetComponent<scrPatrol>().moveIndex = entry.patrolMoveIndex;
                    gridObjectsWithUpdate[foreach_counter].GetComponent<scrPatrol>().isIndexIncreasing =
                        entry.patrolIndexIncreasing;
                }
            }
            // if it doesn't have anything to pop..
            else
            {
                
            }
        }
    }

    #region Initialization
    
     List<Stack<RecordEntry>> InitializeMovementHistory() // returns the movement record (stack) list
    {
        List<GridObject> objects_with_updating = GetAllObjectsWithUpdated(); // sorted by instance id. use it to find correct index in stack list.
        List<Stack<RecordEntry>> object_movement_history = new List<Stack<RecordEntry>>(); // a list of movement history of objects.

        for (int i = 0; i < objects_with_updating.Count; i++)
        {
            // setting up the init move record for the object
            RecordEntry object_iths_entry = new RecordEntry();
            object_iths_entry.panelSwapMove = false;
            object_iths_entry.gridPosition = objects_with_updating[i].gridPosition;
            
            if (objects_with_updating[i].GetComponent<scrInventory>() == null)
            {
                object_iths_entry.inventory = null;
            }

            object_iths_entry.dead = false;

            if (objects_with_updating[i].GetComponent<scrPatrol>() == null)
            {
                object_iths_entry.patrolMoveIndex = -1;
                object_iths_entry.patrolIndexIncreasing = false;
            }
            else
            {
                object_iths_entry.patrolMoveIndex = objects_with_updating[i].GetComponent<scrPatrol>().moveIndex;
                object_iths_entry.patrolIndexIncreasing =
                    objects_with_updating[i].GetComponent<scrPatrol>().isIndexIncreasing;
            }
            // setting up the init move record for the object

            object_movement_history[i] = new Stack<RecordEntry>();
            object_movement_history[i].Push(object_iths_entry);
        }

        return object_movement_history;
    }

    List<Stack<PanelRecordEntry>> InitializePanelHistory()
    {
        List<scrPanel> panels = GetAllPanels(); // this is sorted. use it to find correct index later.
        List<Stack<PanelRecordEntry>> panel_movement_history = new List<Stack<PanelRecordEntry>>();

        for (int i = 0; i < panels.Count; i++)
        {
            PanelRecordEntry panel_iths_entry = new PanelRecordEntry();
            panel_iths_entry.Time_index = panels[i].Time_index;
            panel_iths_entry.panelCoord = panels[i].transform.position;
            
            panel_movement_history[i] = new Stack<PanelRecordEntry>();
        }
        
        return panel_movement_history;
    }

    #endregion
    
    #region Helpers

    // sorted through instance id.
    List<GridObject> GetAllObjectsWithUpdated()
    {
        List<GridObject> returned_grid_object_list = new List<GridObject>();
        
        foreach (GridObject grid_object in FindObjectsByType<GridObject>(FindObjectsSortMode.InstanceID))
        {
            if (grid_object.updated)
            {
                returned_grid_object_list.Add(grid_object);
            }
        }

        return returned_grid_object_list;
    }
    
    // sorted through instance id. All panels stored here.
    List<scrPanel> GetAllPanels()
    {
        scrPanel[] panels_array = FindObjectsByType<scrPanel>(FindObjectsSortMode.InstanceID);
        List<scrPanel> panels = new List<scrPanel>(panels_array);
        
        return panels;
    }

    #endregion
}
