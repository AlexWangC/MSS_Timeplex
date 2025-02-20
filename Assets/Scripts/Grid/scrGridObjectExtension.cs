using System;
using UnityEngine;

public class scrGridObjectExtension : MonoBehaviour
{
    // Used on objects that take up 2 blocks. Calculate where's the other block going to be (shift right)

    private void Update()
    {
        this.GetComponent<GridObject>().gridPosition =
            transform.parent.GetComponent<GridObject>().gridPosition + Vector2.right;
    }
}
