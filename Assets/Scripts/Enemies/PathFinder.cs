using System.Collections.Generic;
using UnityEngine;

/*
This is a tool for path finding 
to find best route from a start position to a goal position
To use this tool:
1. Define a 2D grid, 0 = Walkable tile, 1 = Obstacle
2. create an FindPath list
3. recreate the findpath list everytime the grid map/ start position/ goal position change.
*/
public class PathFinder
{
    // this is using A star algorithm for path finding
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, List<List<int>> grid)// pass in the grid position, not world position
    {
        //print the grid in the console
        foreach (var row in grid)
        {
            foreach (var col in row)
            {
                //Debug.Log(col);
            }
        }
        
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();//Grids to be explored
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();//Which grid does the current grid came from/ last step
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();//Cost from the start to the current node
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();//Estimated cost from the current node to the goal

        openSet.Enqueue(start, 0);//start from starting point
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal); //Total estimated cost

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            foreach (var direction in directions)//go through neighbours
            {
                Vector2Int neighbor = current + direction;

                if (!IsValid(neighbor, grid)) continue; // Skip if it's an obstacle

                int tentativeGScore = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }
        return null; // No path found
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan Distance, only horizontal & vertical moves
        //now that I learned that too, I'mma put it here as note:
        //return sqrt((x1 - x2) ^ 2 + (y1 - y2) ^ 2)// Euclidean Distance for diagonal movement allowed
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        //print the path in the console 
        foreach (var pos in path)
        {
            //Debug.Log(pos);
        }
        return path;
    }

    private bool IsValid(Vector2Int pos, List<List<int>> grid)
    {
        //out of bound
        if (!(pos.x >= 0 && pos.y >= 0 && pos.x < grid.Count && pos.y < grid[0].Count && grid[pos.x][pos.y] == 0))
            return false;

        //if (is obstacle)

        return true;
    }
}