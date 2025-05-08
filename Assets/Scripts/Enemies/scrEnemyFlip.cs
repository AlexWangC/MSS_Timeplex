using System;
using UnityEngine;

public class scrEnemyFlip : MonoBehaviour
{
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    public Vector2 facingDirection;

    // Legacy Code.
    //private bool facing_left = true;
    //private float original_x;

    private void Start()
    {
        if (spriteUp == null || spriteDown == null || spriteLeft == null || spriteRight == null)
        {
            throw new NullReferenceException("bruh scrPlayerFlip needs all 4 sprites assigned to work. You lazy ass.");
        }
    }

    private void Update()
    {
        if (facingDirection == Vector2.left)
        {
            GetComponent<SpriteRenderer>().sprite = spriteLeft;
        }

        if (facingDirection == Vector2.right)
        {
            GetComponent<SpriteRenderer>().sprite = spriteRight;
        }

        if (facingDirection == Vector2.up)
        {
            GetComponent<SpriteRenderer>().sprite = spriteDown;
        }

        if (facingDirection == Vector2.down)
        {
            GetComponent<SpriteRenderer>().sprite = spriteUp;
        }
    }
}
