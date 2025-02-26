using System;
using System.Collections;
using System.Net.NetworkInformation;
using DG.Tweening;
using DialogueSystem;
using Fries;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GridObject))]
public class scrPlayer : MonoBehaviour
{
    GridObject gridObject;

    public int playerAgeIndex; // will be accessed when deciding within one frame how it's going to move.


    scrGoalManager goalManager;
    void Start()
    {
        this.gridObject = GetComponent<GridObject>();
        goalManager = FindAnyObjectByType<scrGoalManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // emergency use for unknown grid bugs only. Impacts performance greatly.
        // updatePlayerPos();
    }

    // remember to implement a multi-tag (a thing could be both dialoguable & wall) mechanic later

    private IEnumerator DelayedEnemiesMove(float delay, Vector2 dir)
    {
        yield return new WaitForSeconds(delay);
        this.transform.parent.GetComponentInChildren<scrEnemyManager>().MoveEnemies(dir);
    }
    
    public bool Move(int dir)
    {
        if (GetComponentInParent<scrPanel>().Dead == false) //if not dead
        {

            Vector2 direction = Vector2.zero; // Default to no movement

            // translating dir to vector2
            if (dir == 0) // up
            {
                direction = Vector2.down; // Move up
            }
            else if (dir == 1) // down
            {
                direction = Vector2.up; // Move down
            }
            else if (dir == 2) // left
            {
                direction = Vector2.left; // Move left
            }
            else if (dir == 3) // right
            {
                direction = Vector2.right; // Move right
            }

            // Check if a movement key was pressed and handle movement
            if (direction != Vector2.zero)
            {
                Vector2 targetPosition = gridObject.gridPosition + direction;
                
                // here plug in richard's dialogue system or anything that checks what's ahead regardless of movement
                /*
                if (checkObject(toVector2Int(targetPosition), "dialogue"))
                {
                    Debug.Log("Hey it's supposed to be dialogue here");

                    foreach (DialogueDisplayer displayer in FindObjectsByType<DialogueDisplayer>(FindObjectsSortMode.None))
                    {
                        if (this.GetComponentInParent<scrPanel>().Time_index == 1)
                        {
                            if (displayer.id == "panel0")
                            {
                                displayer.SetDialogueId("intro");
                                displayer.Open();
                            }
                        }

                        if (this.GetComponentInParent<scrPanel>().Time_index == 2)
                        {
                            if (displayer.id == "panel1")
                            {
                                displayer.SetDialogueId("intro_panel_2");
                                displayer.Open();
                            }
                        }
                    }
                    
                    FindAnyObjectByType<scrMoveInheritanceManager>().Can_move = false;
                    return false;
                }
                */

                // here call the coroutine of a guard that replicates player's movement
                
                
                // 1. notify the enemy manager to move.
                StartCoroutine(DelayedEnemiesMove(FindAnyObjectByType<scrMoveInheritanceManager>().First_enemy_move_delay, direction));
                
                // Note: Only MOVEMENT related code should be placed below!!
                // Note: Only MOVEMENT related code should be placed below!!
                if (!FindAnyObjectByType<scrMoveInheritanceManager>() // Note: Only MOVEMENT related code should be placed below!!
                        .Can_move) // if by this player's turn, already can't move
                {
                    return false; // then don't move
                }
                // Note: Only MOVEMENT related code should be placed below!!
                // Note: Only MOVEMENT related code should be placed below!!

                if (checkObject(toVector2Int(targetPosition), "goal")) // if moving into scrGoal...
                {
                    GridObject target_goal = gameObject.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridManager>().GetGridObjectAtPosition(toVector2Int(targetPosition));
                    switch (target_goal.GetComponent<scrGoal>().DoorType)
                    {
                        case -1:
                            break;
                        case 1:
                            if (GetComponent<scrKey1>())
                            {
                                target_goal.GetComponent<scrGoal>().Locked = false;
                            }
                            break;
                        case 2:
                            if (GetComponent<scrKey2>())
                            {
                                target_goal.GetComponent<scrGoal>().Locked = false;
                            }
                            break;
                        case 3:
                            if (GetComponent<scrKey3>())
                            {
                                target_goal.GetComponent<scrGoal>().Locked = false;
                            }
                            break;
                    }
                    // if the goal is not a locked goal, just move there.
                    if (!target_goal.GetComponent<scrGoal>().Locked)
                    {
                        moveToGoal(target_goal);
                    }
                    else
                    {
                        return forbidMovement();
                    }
                    /*
                    // have the goal check if i have the right key?
                    switch (target_goal.GetComponent<scrGoal>().DoorType)
                    {

                        case -1:
                            throw new ArgumentException("the door has no door type but is locked.");
                            break;
                        case 1:
                            if (!GetComponent<scrKey1>())
                            {
                                forbidMovement();
                            }
                            break;
                        case 2:
                            if (!GetComponent<scrKey2>())
                            {
                                forbidMovement();
                            }
                            break;
                        case 3:
                            if (!GetComponent<scrKey3>())
                            {
                                forbidMovement();
                            }
                            break;
                    }
                    */

                    String nextScene = target_goal.GetComponentInChildren<scrGoal>().nextSceneName;
                    // print("Load Scene Player Script");
                    
                    //goalManager.LoadScene(nextScene);

                    //the old logic, if door is locked player can't step on it

                    /*
                    // if the goal is not a locked goal, just move there.
                    if (!target_goal.GetComponent<scrGoal>().Locked)
                    {
                        moveToGoal();
                    }
                    
                    // if the goal is locked, cross-reference key.
                    else
                    {
                        // have the goal check if i have the right key?
                        switch (target_goal.GetComponent<scrGoal>().DoorType)
                        {
                            case -1:
                                throw new ArgumentException("the door has no door type but is locked.");
                                break;
                            case 1:
                                if (!GetComponent<scrKey1>())
                                {
                                    //return forbidMovement();
                                }
                                break;
                            case 2:
                                if (!GetComponent<scrKey2>())
                                {
                                    //return forbidMovement();
                                }
                                break;
                            case 3:
                                if (!GetComponent<scrKey3>())
                                {
                                    //return forbidMovement();
                                }
                                break;
                        }
                        moveToGoal();
                    
                    }
                    */

                    //the new logic, player can step on it even if it's locked.
                    //But won't trigger go to next level if player don't got all keys.
                    
                    //Go to next level
                    //if (checkIfAllPlayerAtDoor() && lockedDoors == 0)
                    //    endScene(nextSceneName);
                }

                if (checkObject(toVector2Int(targetPosition), "wall")) // if colliding into wall
                {
                    if (scrSoundManager.Instance)
                    {
                        FindAnyObjectByType<DialogueDisplayer>().Open();
                        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hit_wall, this.transform, 1);
                    }
                    else
                    {
                        throw new NullReferenceException("Hey you might wanna throw sound manager in. scrPlayer needs it for movement sound");
                    }
                    wallShake();
                    return forbidMovement();
                }

                if (checkObject(toVector2Int(targetPosition), "spike")) //if colliding spike... move to spike but disable panel.
                {
                    if (scrSoundManager.Instance)
                    {
                        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hurt, this.transform, 1);
                    }
                    else
                    {
                        throw new NullReferenceException("Hey you might wanna throw sound manager in. scrPlayer needs it for movement sound");
                    }
                    
                    killThisPlayer();
                    
                }
                
                // if going towards the enemy, kill it like how it is as spike. Because enemies move after player does.
                if (checkObject(toVector2Int(targetPosition), "enemy"))
                {
                    killThisPlayer();
                }

                if (checkObject(toVector2Int(targetPosition), "key1"))
                {
                    // 0.01 check if i have scrInventory (to see if i can pick up items)
                    if (GetComponent<scrInventory>() != null)
                    {
                        // 1. add key script to game object
                        GetComponent<scrInventory>().addToInventory("key1");
                    }
                    
                    // 2. delete this key from panel (remember to add it back when dropping all)
                    GameObject KeyPickUp1 = transform.parent.gameObject.GetComponentInChildren<scrGridManager>()
                        .GetGridObjectAtPosition(toVector2Int(targetPosition)).gameObject;
                    Destroy(KeyPickUp1);
                }

                // yet to be implemented.
                if (checkObject(toVector2Int(targetPosition), "key2"))
                {
                    // 0.01 check if i have scrInventory (to see if i can pick up items)
                    if (GetComponent<scrInventory>() != null)
                    {
                        // 1. add key script to game object
                        GetComponent<scrInventory>().addToInventory("key2");
                    }
                    
                    // 2. delete this key from panel (remember to add it back when dropping all)
                    GameObject KeyPickUp2 = transform.parent.gameObject.GetComponentInChildren<scrGridManager>()
                        .GetGridObjectAtPosition(toVector2Int(targetPosition)).gameObject;
                    Destroy(KeyPickUp2);
                }

                // yet to be implemented.
                if (checkObject(toVector2Int(targetPosition), "key3"))
                {
                    // 0.01 check if i have scrInventory (to see if i can pick up items)
                    if (GetComponent<scrInventory>() != null)
                    {
                        // 1. add key script to game object
                        GetComponent<scrInventory>().addToInventory("key3");
                    }
                    
                    // 2. delete this key from panel (remember to add it back when dropping all)
                    GameObject KeyPickUp3 = transform.parent.gameObject.GetComponentInChildren<scrGridManager>()
                        .GetGridObjectAtPosition(toVector2Int(targetPosition)).gameObject;
                    Destroy(KeyPickUp3);
                }
                
                // if running into portal
                if (checkObject(toVector2Int(targetPosition), "portal"))
                {
                    GameObject target_portal = transform.parent.gameObject.GetComponentInChildren<scrGridManager>()
                        .GetGridObjectAtPosition(toVector2Int(targetPosition)).gameObject;
                    
                    // 0.1 if portal has not be used up
                    if (target_portal.GetComponent<scrPortal>().remainingUses > 0)
                    {
                        // 0.2 use the portal by one
                        target_portal.GetComponent<scrPortal>().remainingUses--;

                        // 2. find corresponding portal
                        // 3. create a new player at the specified loc de corresponding portal
                        GameObject clone = Instantiate(gameObject);
                        clone.transform.SetParent(target_portal.GetComponent<scrPortal>().correspondingPortal.transform.parent, false);
                        clone.GetComponent<GridObject>().gridPosition =
                            target_portal.GetComponent<scrPortal>().correspondingPortal.GetComponent<GridObject>().gridPosition;

                        // 1. destroy itself
                        Destroy(gameObject);
                    }
                }
                
                // (part of portal functionality) if running into player
                if (checkObject(toVector2Int(targetPosition), "player"))
                {
                    // 1. destroy this panel
                    killThisPlayer();
                }

                    
                if (!checkOutofBound(toVector2Int(targetPosition))) // Check if the target position is out of bound
                {
                    // play sound & check if sound manager is here.
                    if (scrSoundManager.Instance)
                    {
                        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.walk, this.transform, 1);
                    }
                    else
                    {
                        throw new NullReferenceException("Hey you might wanna throw sound manager in. scrPlayer needs it for movement sound");
                    }

                    gridObject.gridPosition = targetPosition; // Update the grid position
                    direction = Vector2.zero;
                    
                    stepShake();
                    
                    return true;
                }
                else // if is out of bound, tell manager it didn't move
                {
                    FindAnyObjectByType<scrMoveInheritanceManager>().Can_move = false;
                    
                    if (scrSoundManager.Instance)
                    {
                        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hit_wall, this.transform, 1);
                    }
                    else
                    {
                        throw new NullReferenceException("Hey you might wanna throw sound manager in. scrPlayer needs it for movement sound");
                    }
                    
                    wallShake();
                    return false;
                }
            }

            return false;
        }
        else //if it's already dead
        {
            return false; 
        }
    }

    public void killThisPlayer()
    {
        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hurt, this.transform, 3);
                    
        //kill all of the rest of the panels after
        scrPanel[] panels = FindAnyObjectByType<scrGridLocations>().panels;
        foreach (var panel in panels)
        {
            if (panel.Time_index >= GetComponentInParent<scrPanel>().Time_index)
            {
                panel.Dead = true;
                panel.PanelKilled();
            }
        }
                    
        FindAnyObjectByType<scrResetManager>().UpdateResetStatus(); // reset if all dead.
        // no longer returns false so player actually moves here.
    }

    // to use this, put it in a return statement before out of bound check.
    private bool forbidMovement()
    {
        //play collide wall music
        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hit_wall, this.transform, 100);
                    
        wallShake();
                    
        FindAnyObjectByType<scrMoveInheritanceManager>().Can_move = false;
        return false;
    }

    #region GoalsRelated

    // the coroutine that calls the content inside. Declared above
    private IEnumerator checkGoalsAfterMovement(System.Action action, GridObject target_goal)
    {
        yield return new WaitForSeconds(1f);
        
        action?.Invoke();
        goalManager.LoadScene(target_goal.GetComponent<scrGoal>().nextSceneName);
    }


    private void moveToGoal(GridObject target_goal)
    {
        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.goal, this.transform, 1);

        StartCoroutine(checkGoalsAfterMovement(() =>
        {
            //FindAnyObjectByType<scrGoalManager>().GoalsReached(); //invoking goals reached here.
        }, target_goal));
        // could implement something else here later...
    }


    #endregion

    #region MovementHelpers
    private Vector2Int toVector2Int(Vector2 vector2)
    {
        return new Vector2Int((int)vector2.x, (int)vector2.y);
    }
    
    // calls a helper function from scrGridManager found on GridMaker under the same parent.
    private bool checkOutofBound(Vector2Int position)
    {
        return gameObject.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridManager>().OutOfBound(position);
    }

    // calls from scrGridManager
    private bool checkObject(Vector2Int position)
    {
        return gameObject.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridManager>()
            .CheckForObjectAtGridPosition(position);
    }

    // calls from scrGridManager
    private bool checkObject(Vector2Int position, string tag)
    {
        return gameObject.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridManager>()
            .CheckForObjectAtGridPosition(position, tag);
    }
    #endregion

    #region shakes
    private void wallShake()
    {
        Vector3 original_position = GetComponentInParent<scrPanel>().transform.position;
        GetComponentInParent<scrPanel>().transform.DOShakePosition(0.6f, 0.5f, 5, 20, true).OnComplete(() =>
        {
            GetComponentInParent<scrPanel>().transform.position = original_position;
            updatePlayerPos();
        });
    }

    private void stepShake()
    {
        Vector3 original_position = GetComponentInParent<scrPanel>().transform.position;
        GetComponentInParent<scrPanel>().transform.DOShakePosition(0.1f, 0.1f, 1, 5, true).OnComplete(() =>
        {
            GetComponentInParent<scrPanel>().transform.position = original_position;
            updatePlayerPos();
        });
    }

    private void deathShake()
    {
        Vector3 original_position = GetComponentInParent<scrPanel>().transform.position;
        GetComponentInParent<scrPanel>().transform.DOShakePosition(2f, 1f, 10, 40, true).OnComplete(() =>
        {
            GetComponentInParent<scrPanel>().transform.position = original_position;
            updatePlayerPos();
        });
    }

    private void updatePlayerPos() // helper 
    {
        this.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridMakerTilted>().CreateGrid();
        this.GetComponent<GridObject>().UpdatePosition();
    }
    #endregion
}
