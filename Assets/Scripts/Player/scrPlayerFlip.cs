using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class scrPlayerFlip : MonoBehaviour
{
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;
    
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetComponent<SpriteRenderer>().sprite = spriteLeft;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GetComponent<SpriteRenderer>().sprite =　spriteRight;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<SpriteRenderer>().sprite = spriteUp;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GetComponent<SpriteRenderer>().sprite = spriteDown;
        }
    }
}
