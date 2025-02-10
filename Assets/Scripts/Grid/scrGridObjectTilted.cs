using UnityEngine;
using NaughtyAttributes;

public class scrGridObjectTilted : MonoBehaviour
{
    public Vector2 gridPosition;
    private Vector2 prevGridPosition;

    [HideInInspector]public scrGridMakerTilted parentGrid; // Jingxing's mod. Using inheritance to get the corresponding grid.

    private void Start()
    {
        getParentGrid();
    }

    private void Update()
    {
        //If our position hasn't been updated, don't 
        if (gridPosition == prevGridPosition)
            return;

        //Move to the new position
        UpdatePosition();

        //Keep track of our previous position
        prevGridPosition = gridPosition;
    }

    [Button("Update Position")]
    public void UpdatePosition()
    {
        this.transform.position = parentGrid.GetWorldPositionFromGrid(gridPosition);
        Debug.Log("Object at " + parentGrid.GetWorldPositionFromGrid(gridPosition) + "projected successfully.");
    }

    private scrGridMakerTilted getParentGrid()
    {
        scrPanel parent_panel = GetComponentInParent<scrPanel>();
        parentGrid = parent_panel.GetComponentInChildren<scrGridMakerTilted>();
        
        return parentGrid;
    }
}
