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
            Vector3 og_scale = transform.localScale;
            this.transform.localScale = new Vector3(original_x, og_scale.y, og_scale.z);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            facing_left = false;
            Vector3 og_scale = transform.localScale;
            this.transform.localScale = new Vector3(original_x * -1, og_scale.y, og_scale.z);
        }
    }
}
