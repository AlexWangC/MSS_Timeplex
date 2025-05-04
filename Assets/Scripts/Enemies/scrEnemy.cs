using System;
using DG.Tweening;
using UnityEngine;

public class scrEnemy : MonoBehaviour
{
    public int priority; // help enemy manager decide which enemy in the panel moves first. 1 = 1st to move, 2 = 2nd to move, etc.
    
    private GridObject _gridObject;

    public bool dead; // showing whether this enemy is dead. adaptation for undo manager.

    private void Start()
    {
        dead = false;
        _gridObject = GetComponent<GridObject>();
    }

    public bool Move(Vector2 dir)
    {
        if (GetComponentInParent<scrPanel>().Dead == false && !dead) // if panel not dead (no point in chasing if so)
        {
            if (dir != Vector2.zero)
            {
                Vector2 targetPosition = _gridObject.gridPosition + dir;
                
                // not affected by inheritance.

                if (checkObject(toVector2Int(targetPosition), "goal"))
                {
                    //wallShake();
                    return forbidMovement();
                }
                
                if (checkObject(toVector2Int(targetPosition), "wall"))
                {
                    //wallShake();
                    return forbidMovement();
                }
                
                if (checkObject(toVector2Int(targetPosition), "spike"))
                {
                    // must be separated because when running into an enemy you want the inventory to drop at separate locs.
                    GetComponent<scrInventory>().inventoryDropEverything();
                    killThisGuard();
                }
                
                if (checkObject(toVector2Int(targetPosition), "enemy"))
                {
                    GetComponent<scrInventory>().inventoryDropEverything(); // drop everything before going.
                    killTheOtherGuard(targetPosition);
                    killThisGuard();
                }

                // Below key 1, key 2, key 3 encounter codes are exactly the same. Refactor later.
                if (checkObject(toVector2Int(targetPosition), "key1"))
                {
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
                
                if (checkObject(toVector2Int(targetPosition), "key2"))
                {
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
                
                if (checkObject(toVector2Int(targetPosition), "key3"))
                {
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
                // Above key 1, key 2, key 3 encounter codes are exactly the same. Refactor later.

                if (checkObject(toVector2Int(targetPosition), "portal"))
                {
                    GameObject target_portal = transform.parent.gameObject.GetComponentInChildren<scrGridManager>()
                        .GetGridObjectAtPosition(toVector2Int(targetPosition)).gameObject;
                    
                    // 0.1 if portal has not be used up
                    if (target_portal.GetComponent<scrPortal>().remainingUses > 0)
                    {
                        // 0.2 use the portal by one
                        target_portal.GetComponent<scrPortal>().remainingUses--;
                        target_portal.GetComponent<scrPortal>().correspondingPortal.GetComponent<scrPortal>().remainingUses--;
                        
                        // play sound & check if sound manager is here.
                        if (scrSoundManager.Instance)
                        {
                            scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.walk, this.transform, 1);
                        }
                        else
                        {
                            throw new NullReferenceException("Hey you might wanna throw sound manager in. scrPlayer needs it for movement sound");
                        }

                        // 2. find corresponding portal
                        // 3. create a new player at the specified loc de corresponding portal
                       this.transform.SetParent(target_portal.GetComponent<scrPortal>().correspondingPortal.transform.parent, false);
                       this.GetComponent<GridObject>().gridPosition =
                            target_portal.GetComponent<GridObject>().gridPosition;
                       GetComponent<GridObject>().getParentGrid();

                       return true;
                    }
                }

                if (checkObject(toVector2Int(targetPosition), "player"))
                {
                    // kill the other player.
                    GameObject player = transform.parent.gameObject.GetComponentInChildren<scrGridManager>()
                        .GetGridObjectAtPosition(toVector2Int(targetPosition)).gameObject;
                    player.GetComponent<scrPlayer>().killThisPlayer();
                }

                // finished all checks. As long as not out of bound we're good.
                if (!checkOutofBound(toVector2Int(targetPosition)))
                {
                    // play sound & check if sound manager is here.
                    if (scrSoundManager.Instance)
                    {
                        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.walk, this.transform, 1);
                    }
                    else
                    {
                        throw new NullReferenceException("Hey you might wanna throw sound manager in. scrDumbDumb needs it for movement sound");
                    }

                    //stepShake();

                    //set flipping
                    GetComponent<scrEnemyFlip>().facingDirection = targetPosition - _gridObject.gridPosition;

                    _gridObject.gridPosition = targetPosition;
                    dir = Vector2.zero;

                    return true;
                }
            }
        }
        return false;
    }

    #region Helpers

    // old way of killing the guard, where the guard instance is completed removed.
    private void killThisGuardLegacy()
    {
        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hurt, this.transform, 3);
        Destroy(gameObject);
    }

    // old way of killing the guard, where the guard instance is completed removed.
    private void killTheOtherGuardLegacy(Vector2 _targetPosition)
    {
        GameObject _other_guard = transform.parent.gameObject.GetComponent<scrGridManager>()
            .GetGridObjectAtPosition(toVector2Int(_targetPosition)).gameObject;
        _other_guard.GetComponent<scrInventory>().inventoryDropEverything(); // have the other mf drop everything before killing them
        Destroy(_other_guard);
    }
    
    private void killThisGuard()
    {
        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hurt, this.transform, 3);
        gameObject.tag = "Untagged";
        dead = true;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void killTheOtherGuard(Vector2 _targetPosition)
    {
        GameObject _other_guard = transform.parent.gameObject.GetComponentInChildren<scrGridManager>()
            .GetGridObjectAtPosition(toVector2Int(_targetPosition)).gameObject;
        _other_guard.GetComponent<scrInventory>().inventoryDropEverything(); // have the other mf drop everything before killing them
        _other_guard.GetComponent<scrEnemy>().killThisGuard();
    }
    
    private bool forbidMovement()
    {
        //play collide wall music
        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.hit_wall, this.transform, 100);
        
        return false;
    }
    
    private bool checkOutofBound(Vector2Int position)
    {
        return gameObject.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridManager>().OutOfBound(position);
    }
    
    private Vector2Int toVector2Int(Vector2 vector2)
    {
        return new Vector2Int((int)vector2.x, (int)vector2.y);
    }
    
    private bool checkObject(Vector2Int position, string tag)
    {
        return gameObject.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridManager>()
            .CheckForObjectAtGridPosition(position, tag);
    }
    
    private void wallShake()
    {
        Vector3 original_position = GetComponentInParent<scrPanel>().transform.position;
        GetComponentInParent<scrPanel>().transform.DOShakePosition(1f, 1f, 10, 40, true).OnComplete(() =>
        {
            GetComponentInParent<scrPanel>().transform.position = original_position;
            updateEnemyPos();
        });
    }
    
    private void stepShake()
    {
        Vector3 original_position = GetComponentInParent<scrPanel>().transform.position;
        GetComponentInParent<scrPanel>().transform.DOShakePosition(0.1f, 0.1f, 1, 5, true).OnComplete(() =>
        {
            GetComponentInParent<scrPanel>().transform.position = original_position;
            updateEnemyPos();
        });
    }
    
    private void updateEnemyPos() // helper 
    {
        this.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridMakerTilted>().CreateGrid();
        this.GetComponent<GridObject>().UpdatePosition();
    }
    #endregion
}
