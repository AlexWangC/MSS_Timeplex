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
        //search based on face direction (based on last move)

        //go through X
        //right
        if (GetComponent<scrEnemyFlip>().facingDirection == Vector2.right)
        {
            for (int i = X; i <= GridNumX - 1; i++)
            {
                if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)) == null)// search next grid in this direction
                    continue;

                if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).CompareTag("wall"))// stop searching, end move
                {
                    break;
                }
                else if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).GetComponent<scrPlayer>() != null)// move twice, stop searching, end move
                {
                    //find best route to player
                    MoveStalker();
                    break;
                }
            }
        }
        //left
        else if (GetComponent<scrEnemyFlip>().facingDirection == Vector2.left)
        {

            for (int i = X; i >= 0; i--)
            {
                if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)) == null)// search next grid in this direction
                    continue;

                if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).CompareTag("wall"))// stop searching, end move
                {
                    break;
                }
                else if (gridManager.GetGridObjectAtPosition(new Vector2Int(i, Y)).GetComponent<scrPlayer>() != null)// move twice, stop searching, end move
                {
                    //find best route to player
                    MoveStalker();
                    break;
                }
            }
        }
        //go through Y
        //down
        else if (GetComponent<scrEnemyFlip>().facingDirection == Vector2.down)
        {
            for (int i = Y; i <= GridNumY - 1; i++)
            {
                if (gridManager.GetGridObjectAtPosition(new Vector2Int(X , i)) == null)// search next grid in this direction
                    continue;

                if (gridManager.GetGridObjectAtPosition(new Vector2Int(X, i)).CompareTag("wall"))// stop searching, end move
                {
                    break;
                }
                else if (gridManager.GetGridObjectAtPosition(new Vector2Int(X, i)).GetComponent<scrPlayer>() != null)// move twice, stop searching, end move
                {
                    //find best route to player
                    MoveStalker();
                    break;
                }
            }
        }
        else if(GetComponent<scrEnemyFlip>().facingDirection == Vector2.up)
        {

            //up
            for (int i = Y; i >= 0; i--)
            {
                if (gridManager.GetGridObjectAtPosition(new Vector2Int(X, i)) == null)// search next grid in this direction
                    continue;

                if (gridManager.GetGridObjectAtPosition(new Vector2Int(X, i)).CompareTag("wall"))// stop searching, end move
                {
                    break;
                }
                else if (gridManager.GetGridObjectAtPosition(new Vector2Int(X, i)).GetComponent<scrPlayer>() != null)// move twice, stop searching, end move
                {
                    //find best route to player
                    MoveStalker();
                    break;
                }
            }
        }
        //search end, not find player, move once
        MoveStalker();
    }

}
