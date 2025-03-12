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

    }

    Vector2Int RefindPath(PathFinder pathfinder, List<List<int>> grid, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = pathfinder.FindPath(start, goal, grid);
        foreach(Vector2Int pos in path)
        {
            print(pos);
        }
        if (path.Count < 2) return goal - start;
        return (path[1] - path[0]);
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
    
    public void MoveStalker()
    {
        //set up path finder
        PathFinder pathfinder = new PathFinder();
        ParentPanel = transform.parent;
        allObjectInSamePanel = FindAllChildGameObjects(ParentPanel);
        //create empty grid 2d list
        List<List<int>> grid = new List<List<int>>();
        foreach (GameObject obj in allObjectInSamePanel)
        {
            if (obj.GetComponent<scrGridMakerTilted>() != null)//find the grid maker script
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
            if (obj.GetComponent<GridObject>() == null)// skip if not a grid object
                continue;

            Vector2Int objGridPos = Vector2Int.RoundToInt(obj.GetComponent<GridObject>().gridPosition);
            if (obj.GetComponent<scrPlayer>() != null && target == null)//find player if target is not setted
            {
                target = obj;
            }
            else if (obj.tag == "wall" || obj.tag == "goal")// obstacles
            {
                grid[objGridPos.x][objGridPos.y] = 1;// set to obstacle
            }
        }

        //debug
        for (int i = 0; i < grid.Count; i++)
        {
            string row = "";
            for (int j = 0; j < grid[i].Count; j++)
            {
                row += grid[i][j] + " ";
            }
            Debug.Log("Row " + i + ": " + row);
        }

        //find path
        Vector2 dir;
        dir = RefindPath(pathfinder, grid,
            Vector2Int.RoundToInt(GetComponent<GridObject>().gridPosition),
            Vector2Int.RoundToInt(target.GetComponent<GridObject>().gridPosition));//get the first movement in priorlist

        //Move
        GetComponent<scrEnemy>().Move(new Vector2(dir.x, dir.y));

    }
}
