using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class scrUndoManager : MonoBehaviour
{
    // for movement history of ONE object
    private struct RecordEntry
    {
        // 1. 
        public bool panelSwapMove; // if it is panel swap move, forget about what's below
        
        // 2.
        public Vector2 gridPosition;
        
        // 3.
        public Dictionary<string, bool> inventory; // if there's no inventory, == null.
        
        // 4.
        public bool dead;
        
        // 5.
        public int patrolMoveIndex; // be -1 if not enemy: patrol
        
        // 6. 
        public bool patrolIndexIncreasing; // be false if not enemy: patrol
        
        //7.
        public int remainingUses; // for portal. be -1 if it is not a portal.
    }

    // for panel swap history of ONE panel
    private struct PanelRecordEntry
    {
        public int Time_index;
        public Vector3 panelCoord;
    }
    
    // for inventory update.
    // 1. keep an pickupable object list updated.
    // 2. keep it in a stack.
    private struct PickuppableInfo
    {
        public Transform the_parent;
        public Vector2 grid_Coordinate;
        public String tag;
    }

    public float UndoAvailableTimer = 3;
    private bool undoFired = false;
    
    public GameObject keyPickUp1;
    public GameObject keyPickUp2;
    public GameObject keyPickUp3;

    private Stack<List<PickuppableInfo>> pickuppableInfoInThisStep; 
    private List<Stack<RecordEntry>> objectsMovementHistory;
    private List<Stack<PanelRecordEntry>> panelsSwapHistory;

    private List<GridObject> gridObjectsWithUpdate; // sorted by instance id.
    private List<scrPanel> panels; // sorted by instance id.
    
    // step one, search for all 
    private void Start()
    {
        pickuppableInfoInThisStep = InitializePickuppableInfo();
        gridObjectsWithUpdate = GetAllObjectsWithUpdated();
        panels = GetAllPanels();
        objectsMovementHistory = InitializeMovementHistory();
        panelsSwapHistory = InitializePanelHistory();
        
        // let's check if initialization works!
        checkObjectsWithUpdate();
        peekObjectMovementHistoryStack(objectsMovementHistory);
    }

    public void Retrace()
    {
        
        for (int foreach_counter = 0; foreach_counter < objectsMovementHistory.Count; foreach_counter++)
        {
            Stack<RecordEntry> record_entry = objectsMovementHistory[foreach_counter];
            // if it has something to pop... Pop it
            if (record_entry.TryPop(out RecordEntry entry))
            {
                // remember to implement a if panel swap move here
                if (entry.panelSwapMove)
                {
                    // pop panel history
                    RetracePanel();
                    
                    // pop this
                    for (int i = foreach_counter + 1; i < objectsMovementHistory.Count; i++)
                    {
                        objectsMovementHistory[i].Pop();
                    }
                    
                    return;
                    // move on
                }
                
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
                
                // if it is a portal
                if (entry.remainingUses != -1)
                {
                    gridObjectsWithUpdate[foreach_counter].GetComponent<scrPortal>().remainingUses =
                        entry.remainingUses;
                }
            }
            // if it doesn't have anything to pop..
            else
            {
                Debug.Log("Yo stacks have been emptied out. No further movement history.");
            }
        }
    }

    // called fro within retrace
    public void RetracePanel()
    {
        int foreach_counter = 0;
        foreach (Stack<PanelRecordEntry> panel_record_entry in panelsSwapHistory)
        {
            if (panel_record_entry.TryPop(out PanelRecordEntry entry))
            {
                panels[foreach_counter].Time_index = entry.Time_index;
                panels[foreach_counter].transform.position = entry.panelCoord;
            }
            else
            {
                Debug.Log("no futher panel swap history");
            }
        }
    }

    // works independently
    public void RetracePickuppable()
    {
        if (pickuppableInfoInThisStep.TryPop(out List<PickuppableInfo> popped_list))
        {
            for (int i = 0; i < popped_list.Count; i++)
            {
                if (popped_list[i].tag == "key1")
                {
                    GameObject pickuppable = Instantiate(keyPickUp1, popped_list[i].the_parent);
                    pickuppable.GetComponent<GridObject>().gridPosition = popped_list[i].grid_Coordinate;
                } else if (popped_list[i].tag == "key2")
                {
                    GameObject pickuppable = Instantiate(keyPickUp2, popped_list[i].the_parent);
                    pickuppable.GetComponent<GridObject>().gridPosition = popped_list[i].grid_Coordinate;
                }
                else if (popped_list[i].tag == "key3")
                {
                    GameObject pickuppable = Instantiate(keyPickUp3, popped_list[i].the_parent);
                    pickuppable.GetComponent<GridObject>().gridPosition = popped_list[i].grid_Coordinate;
                }
            }
        }
        else
        {
            Debug.Log("There aint no more items in pickuppable");
        }
        
    }

    #region Updating

    // call this from the outside
    public void UpdateMovementDriver()
    {
        if (!undoFired)
        {
            UpdateMovementHistory();
        }
    }
    
    private IEnumerator UpdateMovementHistory()
    {
        undoFired = true;
        yield return new WaitForSeconds(UndoAvailableTimer);
        
        // code follows below. To be updated.
    }
    
    #endregion
    
    #region Initialization
    
     List<Stack<RecordEntry>> InitializeMovementHistory() // returns the movement record (stack) list
    {
        List<GridObject> objects_with_updating = GetAllObjectsWithUpdated(); // sorted by instance id. use it to find correct index in stack list.
        List<Stack<RecordEntry>> object_movement_history = new List<Stack<RecordEntry>>(); // a list of movement history of objects. Share index reference with objects_with_updating.

        for (int i = 0; i < objects_with_updating.Count; i++)
        {
            // setting up the init move record for the object
            RecordEntry object_iths_entry = new RecordEntry(); // this is the first movement_history profile
            
            // 1.
            object_iths_entry.panelSwapMove = false; // first move can't be a swap move.
            
            // 2. 
            object_iths_entry.gridPosition = objects_with_updating[i].gridPosition;
            
            if (objects_with_updating[i].GetComponent<scrInventory>() == null)
            {
                //3. 
                object_iths_entry.inventory = null;
            }
            else
            {
                // 3. 
                object_iths_entry.inventory = objects_with_updating[i].GetComponent<scrInventory>().inventory;
            }

            // 4. 
            object_iths_entry.dead = false; // nothing is dead when setting up/

            if (objects_with_updating[i].GetComponent<scrPatrol>() == null)
            {
                // 5.
                object_iths_entry.patrolMoveIndex = -1;
                // 6.
                object_iths_entry.patrolIndexIncreasing = false;
            }
            else
            {
                // 5.
                object_iths_entry.patrolMoveIndex = objects_with_updating[i].GetComponent<scrPatrol>().moveIndex;
                // 6.
                object_iths_entry.patrolIndexIncreasing =
                    objects_with_updating[i].GetComponent<scrPatrol>().isIndexIncreasing;
            }

            // 7.
            if (objects_with_updating[i].GetComponent<scrPortal>() == null) // to be done later.
            {
                object_iths_entry.remainingUses = -1;
            }
            else
            {
                object_iths_entry.remainingUses = objects_with_updating[i].GetComponent<scrPortal>().remainingUses;
            }
            // setting up the init move record for the object

            object_movement_history.Add(new Stack<RecordEntry>());
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
            
            panel_movement_history.Add(new Stack<PanelRecordEntry>());
            panel_movement_history[i].Push(panel_iths_entry);
        }
        
        return panel_movement_history;
    }

    // keep pickuppables updated
    Stack<List<PickuppableInfo>> InitializePickuppableInfo()
    {
        List<scrPickuppable> pickuppables = new List<scrPickuppable>(FindObjectsByType<scrPickuppable>(FindObjectsSortMode.None));
        Stack<List<PickuppableInfo>> initedPickuppableInfo = new Stack<List<PickuppableInfo>>();
        List<PickuppableInfo> pickuppables_info = new List<PickuppableInfo>();

        // translating pickuppables to pickuppables info.
        foreach (scrPickuppable pickuppable in pickuppables)
        {
            PickuppableInfo this_pickuppable_info = new PickuppableInfo();
            this_pickuppable_info.the_parent = pickuppable.transform.parent;
            this_pickuppable_info.grid_Coordinate = pickuppable.GetComponent<GridObject>().gridPosition;
            this_pickuppable_info.tag = pickuppable.tag;
            pickuppables_info.Add(this_pickuppable_info);
        }
        
        initedPickuppableInfo.Push(pickuppables_info);
        
        return initedPickuppableInfo;
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

    #region Validations

    void checkObjectsWithUpdate()
    {
        Debug.Log("Hi. Checking gridObjectsWithUpdate.");
        int counter = 0;
        if (gridObjectsWithUpdate.Count <= 0)
        {
            Debug.Log("there is no gridObject with update.");
        }
        
        foreach (GridObject gridObject in gridObjectsWithUpdate)
        {
            Debug.Log("GridObject "+ counter + " (with Update): " + gridObject.gridPosition);
        }
    }

    void peekObjectMovementHistoryStack(List<Stack<RecordEntry>> movableObjectList)
    {
        Debug.Log("Hi. Checking all movement history of all objects.");
        for (int i = 0; i < movableObjectList.Count; i++)
        {
            Debug.Log("------Most recent movement history for " + movableObjectList[i]);
            RecordEntry most_recent_entry = movableObjectList[i].Peek();
            Debug.Log("1. Is it a panel_swap move? " +  most_recent_entry.panelSwapMove);
            Debug.Log("2. On this move, location was at " + most_recent_entry.gridPosition);

            if (most_recent_entry.inventory == null)
            {
                Debug.Log("3. No inventory bro");
            }
            else
            {
                Debug.Log("3. Inventory Status, keys: " + most_recent_entry.inventory.Keys);
                Debug.Log("3. Inventory Status, values: " + most_recent_entry.inventory.Values);
            }

            Debug.Log("4. Is it dead? " + most_recent_entry.dead);
            Debug.Log("5. Patrol Move Index "+ most_recent_entry.patrolMoveIndex);
            Debug.Log("6. Patrol Index Increasing "+most_recent_entry.patrolIndexIncreasing);
            Debug.Log("7. How many uses remaining for it as a portal? "+most_recent_entry.remainingUses);
            Debug.Log("------");
        }
        
    }
    

    #endregion
}
