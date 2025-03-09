using UnityEngine;

public class scrStalkerFlashlight : scrStalker
{
    int GridNumX;
    int GridNumY;
    int X;
    int Y;
    scrGridManager gridManager;
    void Start()
    {
        GridNumX = transform.parent.GetComponentInChildren<scrGridMakerTilted>().numBlocksX;
        GridNumY = transform.parent.GetComponentInChildren<scrGridMakerTilted>().numBlocksY;
        X = Mathf.FloorToInt(GetComponent<GridObject>().gridPosition.x);
        Y = Mathf.FloorToInt(GetComponent<GridObject>().gridPosition.y);
        gridManager = transform.parent.GetComponentInChildren<scrGridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveStalkerFlashlight()
    {
        //go through X
        //right
        for(int i = X; i <= GridNumX - 1; i++)
        {
            if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).CompareTag("Wall"))
            {
                break;
            }
            else if(gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).GetComponent<scrPlayer>() != null)
            {
                //find best route to player
                MoveStalker();

                //or

                //Directly move toward player
                GetComponent<scrEnemy>().Move(Vector2.right);
                break;
            }
        }
        //left
        for (int i = X; i >= 0; i--)
        {
            if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).CompareTag("Wall"))
            {
                break;
            }
            else if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).GetComponent<scrPlayer>() != null)
            {
                //find best route to player
                MoveStalker();

                //or

                //Directly move toward player
                GetComponent<scrEnemy>().Move(Vector2.left);
                break;
            }
        }
        //go through Y
        //down
        for (int i = Y; i <= GridNumY - 1; i++)
        {
            if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).CompareTag("Wall"))
            {
                break;
            }
            else if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).GetComponent<scrPlayer>() != null)
            {
                //find best route to player
                MoveStalker();

                //or

                //Directly move toward player
                GetComponent<scrEnemy>().Move(Vector2.up);
                break;
            }
        }
        //up
        for (int i = Y; i >= 0; i--)
        {
            if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).CompareTag("Wall"))
            {
                break;
            }
            else if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).GetComponent<scrPlayer>() != null)
            {
                //find best route to player
                MoveStalker();

                //or

                //Directly move toward player
                GetComponent<scrEnemy>().Move(Vector2.down);
                break;
            }
        }
    }
}
