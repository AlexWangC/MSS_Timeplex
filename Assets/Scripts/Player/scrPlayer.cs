using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(GridObject))]
public class scrPlayer : MonoBehaviour
{
    GridObject gridObject;
    
    void Start()
    {
        this.gridObject = GetComponent<GridObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // emergency use for unknown grid bugs only. Impacts performance greatly.
        // updatePlayerPos();
    }

    public bool Move(int dir)
    {
        if (GetComponentInParent<scrPanel>().Dead == false) //if not dead
        {

            Vector2 direction = Vector2.zero; // Default to no movement

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

                if (!FindAnyObjectByType<scrMoveInheritanceManager>()
                        .Can_move) // if by this player's turn, already can't move
                {
                    return false; // then don't move
                }

                if (checkObject(toVector2Int(targetPosition), "goal")) // if moving into scrGoal...
                {
                    scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.goal, this.transform, 1);
                    
                    StartCoroutine(checkGoalsAfterMovement(() =>
                    {
                        FindAnyObjectByType<scrGoalManager>().GoalsReached(); //invoking goals reached here.
                    }));
                    // could implement something else here later...
                }

                if (checkObject(toVector2Int(targetPosition), "wall")) // if colliding into wall
                {
                    //play collide wall music
                    scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hit_wall, this.transform, 100);
                    
                    wallShake();
                    
                    FindAnyObjectByType<scrMoveInheritanceManager>().Can_move = false;
                    return false;
                }

                if (checkObject(toVector2Int(targetPosition), "spike")) //if colliding spike... move to spike but disable panel.
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

                if (!checkOutofBound(toVector2Int(targetPosition))) // Check if the target position is out of bound
                {
                    scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.walk, this.transform, 1);
                    
                    gridObject.gridPosition = targetPosition; // Update the grid position
                    direction = Vector2.zero;
                    
                    stepShake();
                    
                    return true;
                }
                else // if is out of bound, tell manager it didn't move
                {
                    FindAnyObjectByType<scrMoveInheritanceManager>().Can_move = false;
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

    // the coroutine that calls the content inside. Declared above
    private IEnumerator checkGoalsAfterMovement(System.Action action)
    {
        yield return new WaitForSeconds(0.5f);
        action?.Invoke();
    }

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
        GetComponentInParent<scrPanel>().transform.DOShakePosition(1f, 1f, 10, 40, true).OnComplete(() =>
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
