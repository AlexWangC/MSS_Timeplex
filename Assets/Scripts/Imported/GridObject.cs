// Code by Danny Hawk. 
// Edited by Jingxing
/*
 * Comments:
 *      Always place the object with this script AS A CHILDREN of a GridMaker!!
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Specialized;
using Fries;

[ExecuteInEditMode]
public class GridObject : MonoBehaviour
{
    /* mush's code.
    //Class to store information in movementHistory for position inventory and if it is alive or not
    [System.Serializable]
    public class Data
    {
        public Vector2 position;
        public Dictionary<string, bool> inventory;
        public bool alive;
    }
    */

    public bool updated = false; // mark this as true if you want it to be reset in movement History.
    public Vector2 gridPosition;
    
    private SpriteRenderer sr; // part of temporalProjectionFix
    private Quaternion originalRotation;
    private Vector3 originalScale;
    [HideInInspector] public scrGridMakerTilted parentGrid; // Jingxing's mod. Using inheritance to get the corresponding grid.

    [HideInInspector] public bool inventoryHasItem;

    private bool isPlayer = false;
    
    private void Start()
    {
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
        var pl = gameObject.getComponent<scrPlayer>();
        if (pl) isPlayer = true;
        
        getParentGrid();
        
        temporalProjectionFixStart();
        
        //mightbug for Text UI with grid position, need exception
    }

    private bool isFirstRun = true;
    private void Update()
    {
        //Move to the new position
        UpdatePosition();
        if (isFirstRun && isPlayer) {
            isFirstRun = false;
            scrMoveInheritanceManager.invokeOnPlayerMove(this.getComponent<scrPlayer>(), this);
        }
    }

    [Button("Update Position")]
    public void UpdatePosition()
    {
        //print(parentGrid);
        //print(gridPosition);
        if (parentGrid == null)
        {
            Debug.Log("got a null parent grid", gameObject);
        }

        this.transform.position = parentGrid.GetWorldPositionFromGrid(gridPosition);
        
        temporalProjectionFixUpdate();
        
        //Debug.Log("Object at " + parentGrid.GetWorldPositionFromGrid(gridPosition) + "projected successfully.");
    }

    private IEnumerator LerpMoveWithAnimation(Vector3 startWorldPos, Vector3 targetWorldPos, float duration)
    {
        // Calculate direction from positions (normalized, 2D)
        Vector2 direction = (targetWorldPos - startWorldPos);
        direction = new Vector2(
            Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? Mathf.Sign(direction.x) : 0,
            Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? Mathf.Sign(direction.y) : 0
        );

        // Animation parameters (subtle)
        Vector3 stretchScale = new Vector3( 0.01f, -0.01f, 0f);
        float leanAngle = 5f * (direction.x != 0 ? Mathf.Sign(direction.x) : Mathf.Sign(direction.y)); // subtle lean

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Lerp position
            transform.position = Vector3.Lerp(startWorldPos, targetWorldPos, t);

            // Animate squash and stretch (ease in/out)
            float squashT = Mathf.Sin(t * Mathf.PI); // 0->1->0
            transform.localScale = Vector3.LerpUnclamped(originalScale, originalScale + stretchScale, squashT);

            // Animate lean (ease in/out)
            float leanT = Mathf.Sin(t * Mathf.PI); // 0->1->0
            transform.localRotation = Quaternion.LerpUnclamped(originalRotation, Quaternion.Euler(0, 0, leanAngle), leanT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final position and reset animation
        transform.position = targetWorldPos;
        transform.localScale = originalScale;
        transform.localRotation = originalRotation;
    }

    public scrGridMakerTilted getParentGrid()
    {
        scrPanel parent_panel = GetComponentInParent<scrPanel>();
        parentGrid = parent_panel.GetComponentInChildren<scrGridMakerTilted>();
        
        return parentGrid;
    }

    private void temporalProjectionFixStart()
    {
        // temporal projection fix
        sr = GetComponent<SpriteRenderer>();
        if(sr.sortingLayerName != "tiles")
            sr.sortingLayerName = "objects";
        // temporal projection fix
    }

    private void temporalProjectionFixUpdate()
    {
        if(sr!= null)
        {
            sr.sortingOrder = Mathf.RoundToInt(gridPosition.y * 100 - gridPosition.x);
        }
    }
    
    /*mush's code
    // record information of gridposition, alive state and inventory state
    public Data GetData()
    {
        Data data = new Data();
        data.position = gridPosition;
        // data.alive 
        // data.inventory
        return data;
    }
    */
}
