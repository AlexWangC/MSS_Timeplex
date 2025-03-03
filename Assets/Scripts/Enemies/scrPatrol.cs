using System.Collections.Generic;
using UnityEngine;

public class scrPatrol : MonoBehaviour
{

    [System.Serializable]
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    [SerializeField] private List<Direction> movementSequence = new List<Direction>();
    public bool backAndForth = true;
    private bool isIndexIncreasing = true;
    public int moveIndex = 0;

    public void MovePatrol()
    {
        //get direction by vector of current index.
        Vector2 dir = Vector2.zero;
        print("move index: " + moveIndex);
        print(movementSequence[moveIndex]);
        if(backAndForth && !isIndexIncreasing)// special case: go through list reversedly, move reversedly (from end to start)
        {
            switch (movementSequence[moveIndex])
            {
                case Direction.Up:
                    dir = Vector2.up;
                    break;

                case Direction.Down:
                    dir = Vector2.down;
                    break;

                case Direction.Left:
                    dir = Vector2.right;
                    break;

                case Direction.Right:
                    dir = Vector2.left;
                    break;
            }
        }
        else
        {
            switch (movementSequence[moveIndex])// move from start to end, by list sequence 
            {
                case Direction.Up:
                    dir = Vector2.down;
                    break;

                case Direction.Down:
                    dir = Vector2.up;
                    break;

                case Direction.Left:
                    dir = Vector2.left;
                    break;

                case Direction.Right:
                    dir = Vector2.right;
                    break;
            }
        }
        GetComponent<scrEnemy>().Move(dir);//move the enemy

        //update moveindex
        if (!backAndForth)//loop
        {
            moveIndex++;
            print(moveIndex);
            print(movementSequence.Count - 1);
            if(moveIndex == movementSequence.Count) 
            { moveIndex = 0; }
        }
        else//go through by sequence, then reverse sequence
        {
            if (isIndexIncreasing)
            {
                if (moveIndex == movementSequence.Count - 1)
                {
                    isIndexIncreasing = false;// touch end, reverse
                }
                else
                {
                    moveIndex++;//move index foward
                }
            }
            else
            {
                if (moveIndex == -1)
                {
                    isIndexIncreasing = true;//touch end, reverse
                }
                else
                {
                    moveIndex--;//move index forward
                }
            }
            print("end");
        }

        print("end");
    }
}
