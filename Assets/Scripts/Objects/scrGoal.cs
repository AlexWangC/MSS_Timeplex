using System;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrGoal : MonoBehaviour
{
    //public bool MovementLock; // set whether after reaching this goal locks player movement in this scene. NOT YET IMPLEMENTED
    public bool Reached;
    public bool Locked = false;
    public int DoorType = -1; // whether it's corresponding to key 1, 2, or 3. Default -1 means the door's not locked.

    [HideInInspector]
    public string nextSceneName;
#if UNITY_EDITOR
    [Tooltip("Set connected scene")]
    [SerializeField] SceneAsset nextScene; // Drag a scene here in the Inspector
    private void OnValidate()
    {
        if (nextScene != null)
        {
            nextSceneName = nextScene.name;
        }
    }
#endif
    

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
        if (objects_at_this_position.Length >= 1)
        {
            foreach (GridObject obj in objects_at_this_position)
            {
                if (obj.CompareTag("player"))
                {
                    print(obj.name + " Setted reached to true");
                    Reached = true;
                    return true;
                }
            }
        }

        print(gameObject.name + " Setted reached to false");
        Reached = false;
        return false;
    }

    private Vector2Int toVector2Int(Vector2 vector2) // helper from scr player. Local access for efficiency
    {
        return new Vector2Int((int)vector2.x, (int)vector2.y);
    }
}
