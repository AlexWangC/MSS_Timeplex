using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class scrDumbDumb : MonoBehaviour
{
    public void MoveDumbDumb(Vector2 dir)
    {
        GetComponent<scrEnemy>().Move(dir);
    }
}
