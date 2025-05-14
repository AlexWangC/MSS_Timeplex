using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
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
        public Transform the_parent; // the panel transform
        public Vector2 grid_Coordinate;
        public String tag;
    }

    public float UndoAvailableTimer;
    public bool UndoAvailable = true; // for UndoManagerController's reference.
    private bool undoFired = false;
    
    public GameObject keyPickUp1;
    public GameObject keyPickUp2;
    public GameObject keyPickUp3;

    private Stack<List<PickuppableInfo>> pickuppableInfoInThisStep; 
    private List<Stack<RecordEntry>> objectsMovementHistory; // check its content.
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
        Debug.Log("Breakpoint Specifier");
        panelsSwapHistory = InitializePanelHistory();
        
        // let's check if initialization works!
        checkObjectsWithUpdate();
        peekObjectMovementHistoryStack(objectsMovementHistory);
        
        // move delay = delayVal * playerCount
        UndoAvailableTimer = FindAnyObjectByType<scrMoveInheritanceManager>().Move_delay *
            FindObjectsByType<scrPlayer>(FindObjectsSortMode.None).Length + 0.01f;
    }

    // after pop, peek next and set value to the peeked val
    public void Retrace() // this gets called
    {
        // insert retrace button sound here.
        
     
        // looping through all object's movement history stack
        for (int foreach_counter = 0; foreach_counter < objectsMovementHistory.Count; foreach_counter++)
        {
            Stack<RecordEntry> record_entry = objectsMovementHistory[foreach_counter];
            // if it has something to pop, check
            if (record_entry.TryPeek(out RecordEntry entryNow))
            {
                    // remember to implement a if panel swap move here
                    if (entryNow.panelSwapMove)
                    {
                        // do something about the panels
                        RetracePanel();

                        for (int i = foreach_counter + 1; i < objectsMovementHistory.Count; i++)
                        {
                            objectsMovementHistory[i].Pop();

                            // then pop this from all
                        }

                        return;
                        // move on
                    }

                    record_entry.Pop();
                    if (record_entry.TryPeek(out RecordEntry entry))
                    {

                        gridObjectsWithUpdate[foreach_counter].gridPosition = entry.gridPosition;
                        // we'll worry about death reset here later.
                        // currently death is just disabling the sprite renderer???
                        // Oh and changing tag to untagged!!

                        // if the object has an inventory
                        if (entry.inventory != null)
                        {
                            gridObjectsWithUpdate[foreach_counter].GetComponent<scrInventory>().inventory = entry.inventory;
                            gridObjectsWithUpdate[foreach_counter].GetComponent<scrInventory>()
                                .syncActualInventoryWithDictionary();
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
                            gridObjectsWithUpdate[foreach_counter].GetComponent<scrPatrol>().moveIndex =
                                entry.patrolMoveIndex;
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

                    else {
                        Debug.Log("Stack emptied out at second peek");
                    }
            }
            // if it doesn't have anything to pop..
            else
            {
                Debug.Log("Yo stacks have been emptied out after first peek. No further movement history.");
            }
            
            // notice how it's just setting the gridpos and not respawning? that means death is probably handled through moving things to a far away place.
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
                    // this is suspicious. How is it instantiation-based.
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

    // call this from the outside, updated when movement key detected
    public void UpdateMovementDriver()
    {
        if (!undoFired) // this is to make sure you only fire undoFired once
        {
            // let's check if this work!
            StartCoroutine(UpdateMovementHistory());
        }
        
        if (FindAnyObjectByType<scrMovementSavedText>(FindObjectsInactive.Include) != null)
        {
            StartCoroutine(ToggleMovementSavedText(UndoAvailableTimer, 1));
        }
    }
    
    private IEnumerator UpdateMovementHistory()
    {
        Debug.Log("Movement History updated.");
        
        undoFired = true;
        yield return new WaitForSeconds(UndoAvailableTimer); // after yielding, everything should be in place.
        
        // update pickuppables here.
        //pickuppableInfoInThisStep.Push();
        
        // code follows below. To be updated.
        // first of all we get all objects.
        List<GridObject> objects_with_updating = GetAllObjectsWithUpdated(); //this method sorts through instance ID.
        for (int i = 0; i < objects_with_updating.Count; i++)
        {
            RecordEntry currentRE = new RecordEntry();
            
            // 1. 
            currentRE.panelSwapMove = false; // panel swap move check would be done elsewhere
            
            // 2.
            currentRE.gridPosition = objects_with_updating[i].gridPosition;
            
            // check if objects_with_updating[i] has scrInventory
            if (objects_with_updating[i].GetComponent<scrInventory>() != null)
            {
                // 3. 
                currentRE.inventory = objects_with_updating[i].GetComponent<scrInventory>().inventory;
            }
            
            // if it is scrEnemy, you have a dead bool.
            // if it is scrPlayer, death is handled via killing panel. (a dead bool)
            // how is panel death handled? How is time "skipping" dead panels
            //      check if dead panel's objects inside are deleted.
            if (objects_with_updating[i].GetComponent<scrEnemy>() != null)
            {
                // 4.
                currentRE.dead = objects_with_updating[i].GetComponent<scrEnemy>().dead;
            } else if (objects_with_updating[i].GetComponent<scrPlayer>() != null)
            {
                // 4.
                currentRE.dead = objects_with_updating[i].GetComponentInParent<scrPanel>().Dead;
            }
            
            // if it is patrol...
            if (objects_with_updating[i].GetComponent<scrPatrol>() != null)
            {
                // 5.
                currentRE.patrolMoveIndex = objects_with_updating[i].GetComponent<scrPatrol>().moveIndex;
                
                // 6.
                currentRE.patrolIndexIncreasing = objects_with_updating[i].GetComponent<scrPatrol>().isIndexIncreasing;
            }
            else
            {
                // 5.
                currentRE.patrolMoveIndex = -1; 
                
                // 6.
                currentRE.patrolIndexIncreasing = false;
            }
            
            // if it is portal...
            if (objects_with_updating[i].GetComponent<scrPortal>() != null)
            {
                // 7.
                currentRE.remainingUses = objects_with_updating[i].GetComponent<scrPortal>().remainingUses;
            }
            else
            {
                currentRE.remainingUses = -1;
            }
            
            objectsMovementHistory[i].Push(currentRE);
            Debug.Log("breakpoint specifier");
            
            peekObjectMovementHistoryStack(objectsMovementHistory);
        }

        undoFired = false; // set it to false so that it can fire again.
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

    #region UI
    IEnumerator ToggleMovementSavedText(float seconds_saved, float seconds_flashed)
    {
        FindAnyObjectByType<scrUndoAvailableText>(FindObjectsInactive.Include).GetComponent<TextMeshProUGUI>().text =
            "Saving Movement..."; // while this, disable hit z.
        UndoAvailable = false;
        
        yield return new WaitForSeconds(seconds_saved);
        
        FindAnyObjectByType<scrMovementSavedText>(FindObjectsInactive.Include).gameObject.SetActive(true);
        FindAnyObjectByType<scrUndoAvailableText>(FindObjectsInactive.Include).GetComponent<TextMeshProUGUI>().text =
            "Hit 'Z' to Undo";
        UndoAvailable = true;
        
        yield return new WaitForSeconds(seconds_flashed);
        
        FindAnyObjectByType<scrMovementSavedText>(FindObjectsInactive.Include).gameObject.SetActive(false);
    }

    #endregion
}
