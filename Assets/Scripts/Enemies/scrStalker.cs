using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class scrStalker : MonoBehaviour
{
    public GameObject target;
    Transform ParentPanel;
    List<GameObject> allObjectInSamePanel;


    void Start()
    {
        PathFinder pathfinder = new PathFinder();
        ParentPanel = transform.parent;
        allObjectInSamePanel = FindAllChildGameObjects(ParentPanel);
        //create empty grid 2d list
        List<List<int>> grid = new List<List<int>>();
        foreach (GameObject obj in allObjectInSamePanel)
        {
            if(obj is scrGridMakerTilted)//find the grid maker script
            {
                //set grid's width and height
                int width = obj.GetComponent<scrGridMakerTilted>().numBlocksX;
                int height = obj.GetComponent<scrGridMakerTilted>().numBlocksY;
                //go through grids
                for (int x = 0; x < width; x++)
                {
                    grid.Add(new List<int>());
                    for (int y = 0; y < height; y++)
                    {
                        grid[x].Add(0); 
                    }
                }
            }
        }

        //find all obstacles
        foreach (GameObject obj in allObjectInSamePanel)
        {
            Vector2Int objGridPos = Vector2Int.RoundToInt(obj.GetComponent<GridObject>().gridPosition);
            if(obj is scrPlayer && target == null)
            {
                target = obj;
            }
            else if(obj.tag == "wall" || obj.tag == "spike" || obj.tag == "goal")// obstacles
            {
                grid[objGridPos.x][objGridPos.y] = 1;// set to obstacle
            }
        }
        RefindPath(pathfinder, grid);

        
    }

    void RefindPath(PathFinder pathfinder, List<List<int>> grid)
    {
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int goal = new Vector2Int(4, 4);

        List<Vector2Int> path = pathfinder.FindPath(start, goal, grid);
    }

    //test the pathfinder script here
    void pathFindingTester()
    {
        int[,] grid = new int[,]
        {
            { 0, 0, 0, 0, 1 },
            { 0, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { 1, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int goal = new Vector2Int(4, 4);

        PathFinder pathfinder = new PathFinder();
        List<Vector2Int> path = pathfinder.FindPath(start, goal, grid);

        if (path != null)
        {
            Debug.Log("Path found!");
            foreach (Vector2Int pos in path)
            {
                Debug.Log($"Step: {pos}");
            }
        }
        else
        {
            Debug.Log("No path found!");
        }
    }

    List<GameObject> FindAllChildGameObjects(Transform parent)
    {
        List<GameObject> childObjects = new List<GameObject>();

        foreach (Transform child in parent)
        {
            childObjects.Add(child.gameObject);
            childObjects.AddRange(FindAllChildGameObjects(child)); // Recursive search
        }
        return childObjects;
    }
}
