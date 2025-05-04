using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class scrPlayerFlip : MonoBehaviour
{
    private Animator animator;
    private string currentState;

    // Legacy Code.
    //private bool facing_left = true;
    //private float original_x;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        /*if (spriteUp == null || spriteDown == null || spriteLeft == null || spriteRight == null)
        {
            throw new NullReferenceException("bruh scrPlayerFlip needs all 4 sprites assigned to work. You lazy ass.");
        }*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeState("Child_Up");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeState("Child_Down");
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeState("Child_Left");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeState("Child_Right");
        }
    }
    private void ChangeState(string newState)
    {
        if (currentState == newState) return; 

        if (animator.HasState(0, Animator.StringToHash(newState)))
        {
            animator.Play(newState);
            currentState = newState;
        }
        else
        {
            Debug.LogWarning($"Animator state '{newState}' not found on {gameObject.name}");
        }
    }
}
