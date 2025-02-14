using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class scrPlayerFlip : MonoBehaviour
{
    private bool facing_left = true;
    private float original_x;

    private void Start()
    {
        original_x = transform.localScale.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            facing_left = true;
            transform.localEulerAngles = new Vector3(0, 0f, 0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            facing_left = false;
            transform.localEulerAngles = new Vector3(0, 180f, 0);
        }
    }
}
