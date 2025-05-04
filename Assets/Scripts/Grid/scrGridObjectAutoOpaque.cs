/*
 * Attach this script to objects that you wish to opaque out when there's something behind it. Disable it if you don't want this effect.
 */

using System;
using System.Collections;
using UnityEngine;

public class scrGridObjectAutoOpaque : MonoBehaviour
{
    public float targetOpacity; // this is hoow opaque you want the object to be when there's something behind.
    public float fadeSpeed; // how fast it fades
    
    private float originalOpacity;
    private SpriteRenderer sr; // initialized at start.
    private Coroutine currentFade;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalOpacity = sr.color.a;
    }

    // call this after everything finished moving.
    public void CheckWhatsAtBack()
    {
        // if there is something at back
        if (checkObject(new Vector2Int((int)(this.GetComponent<GridObject>().gridPosition.x), (int)(this.GetComponent<GridObject>().gridPosition.y - 1))))
        {
            if (!Mathf.Approximately(sr.color.a, targetOpacity))
            {
                UpdateToTargetOpacity();
            }
        }
        else
        {
            if (!Mathf.Approximately(sr.color.a, originalOpacity))
            {
                RevertOpacity();
            }
        }
    }

    #region Methods to call for fading to target or to original

    void UpdateToTargetOpacity() // gets called by something else, changes the sprite renderer attached to the same object.
    {
        if (sr == null)
        {
            throw new NullReferenceException(
                "For the auto-opaque function to work, the object needs to have a SpriteRenderer.");
        }
        
        else // fade into the target opacity.
        {
            if (currentFade != null)
            {
                StopCoroutine(currentFade);
            }
            currentFade = StartCoroutine(FadeTo(targetOpacity));
        }
    }

    void RevertOpacity()
    {
        if (currentFade != null)
        {
            StopCoroutine(currentFade);
        }

        currentFade = StartCoroutine(FadeTo(originalOpacity));
    }

    #endregion

    #region Helpers
    IEnumerator FadeTo(float target)
    {
        while (!Mathf.Approximately(sr.color.a, target)) // good practice for avoiding marginal errors in float point comparison.
        {
            Color color = sr.color;
            color.a = Mathf.MoveTowards(sr.color.a, target, fadeSpeed * Time.deltaTime);
            sr.color = color;
            yield return null;
        }
    }

    private bool checkObject(Vector2Int position)
    {
        GridObject[] objects_at_position = gameObject.GetComponentInParent<scrPanel>().GetComponentInChildren<scrGridManager>()
            .GetGridObjectsAtPosition(position);
        foreach (GridObject obj in objects_at_position)
        {
            if (obj.tag != "tiles")
            {
                return true;
            }
        }

        return false;
    }
    
    #endregion
}
