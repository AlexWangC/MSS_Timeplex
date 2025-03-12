using System;
using Fries;
# if UNITY_EDITOR
using UnityEditor;
# endif
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrGoal : MonoBehaviour
{
    //public bool MovementLock; // set whether after reaching this goal locks player movement in this scene. NOT YET IMPLEMENTED
    public bool Reached;
    public bool Locked = false;
    public int DoorType = -1; // whether it's corresponding to key 1, 2, or 3. Default -1 means the door's not locked.

    // [HideInInspector]
    public string nextSceneName;
    [Tooltip("Set connected scene")]
    # if UNITY_EDITOR
    [SerializeField] public SceneAsset nextScene; // Drag a scene here in the Inspector
    # endif
    // private void OnValidate()
    // {
    //     # if UNITY_EDITOR
    //     if (nextScene != null)
    //     {
    //         nextSceneName = nextScene.name;
    //     }
    //     # endif
    // }

    [MenuItem("Tools/Fries/Bat")]
    public static void item() {
        scrGoal[] goals = FindObjectsByType<scrGoal>(FindObjectsSortMode.None);
        goals.ForEach(goal => {
            if (goal.nextScene == null) return;
            goal.nextSceneName = goal.nextScene.name;
        });
    }
    

    private void Start()
    {
        Reached = false; // uwu 
        // if (nextScene.name == scrSceneSequenceManager.lastScene.name)
        if (nextSceneName == scrSceneSequenceManager.lastScene.name)
        {
            //move player position
            Transform parent = transform.parent;
            if (parent != null)
            {
                // Search for a child with the "Player" tag
                foreach (Transform child in parent)
                {
                    if (child.CompareTag("Player")) // Check tag
                    {
                        GridObject gridObject = child.GetComponent<GridObject>();
                        if (gridObject != null)
                        {
                            gridObject.gridPosition = GetComponent<GridObject>().gridPosition;
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        checkIfReached();
    }

    // i think this is the problem. Refactor it first?
    public bool checkIfReached()
    {
        scrGridManager grid_manager = transform.parent.gameObject.GetComponentInChildren<scrGridManager>();
        
        GridObject[] objects_at_this_position = grid_manager.GetGridObjectsAtPosition(toVector2Int(GetComponent<GridObject>().gridPosition)); // getting all objects at this scrGoal's location
        if (objects_at_this_position.Length >= 1)
        {
            foreach (GridObject obj in objects_at_this_position)
            {
                if (obj.CompareTag("player"))
                {
                    //print(obj.name + " Set reached to true");
                    Reached = true;
                    return true;
                }
            }
        }

        //print(gameObject.name + " Set reached to false");
        Reached = false;
        return false;
    }

    private Vector2Int toVector2Int(Vector2 vector2) // helper from scr player. Local access for efficiency
    {
        return new Vector2Int((int)vector2.x, (int)vector2.y);
    }
}
