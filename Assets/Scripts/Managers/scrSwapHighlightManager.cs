using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class scrSwapHighlightManager : MonoBehaviour
{
    public scrPanel First_Clicked_Panel;
    public scrSwapManager swap_manager;

    public GameObject Top_left_corner;
    public GameObject Top_right_corner;
    public GameObject Buttom_left_corner;
    public GameObject Buttom_right_corner;

    private List<GameObject> spawnedCorners = new List<GameObject>();
    
    public void PanelHovered(scrPanel panel)
    {
        if (panel.Dead)
        {
            return;
        }

        if (Top_left_corner == null || Top_right_corner == null || Buttom_left_corner == null ||
            Buttom_right_corner == null)
        {
            Debug.Log("Please assign corner prefabs for swap highlight manager");
            return;
        }
        // generate four corners
        SpawnCorners(findGridCorners(panel.GetComponentInChildren<scrGridMakerTilted>()));
        
        // Legacy Highlight effect of changing the color.a of a panel.
        /*
      // Debug.Log("Panel hovered Invoked at panel " + panel.Time_index);
       panel.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
       */
    }

    public void PanelExited(scrPanel panel)
    {
        if (panel.Dead)
        {
            return;
        }
        
        if (Top_left_corner == null || Top_right_corner == null || Buttom_left_corner == null ||
            Buttom_right_corner == null)
        {
            Debug.Log("Please assign corner prefabs for swap highlight manager");
            return;
        }
        
        // generate four corners
        DestroyCorners();
        
        // Legacy Highlight effect of changing the color.a of a panel.
        /*
        if (panel.GetComponent<SpriteRenderer>().enabled)
        {
            panel.GetComponent<SpriteRenderer>().color = panel.originalColorPanel;
        }
        */
    }

    private Vector3[] findGridCorners(scrGridMakerTilted grid)
    {
        float offset_x = 1.5f; // the offset of each horizontal block width
        float offset_y = 1.7f; // the offset of each vertical block height

        float top_left_left_offset = 0.5f;
        float top_left_up_offset = 2.5f;
        
        Vector3 topLeft = grid.gameObject.transform.position - new Vector3(top_left_left_offset, 0, 0) + new Vector3(0, top_left_up_offset, 0);
        Vector3 topRight = topLeft + new Vector3(grid.numBlocksX * grid.blockWidth * offset_x, 0, 0);
        Vector3 bottomLeft = topLeft - new Vector3(0, grid.numBlocksY * grid.blockHeight * offset_y, 0);
        Vector3 bottomRight = topLeft - new Vector3(0, grid.numBlocksY * grid.blockHeight * offset_y, 0) +
                              new Vector3(grid.numBlocksX * grid.blockWidth * offset_x, 0, 0);

        return new Vector3[]{topLeft, topRight, bottomLeft, bottomRight};
    }

    // take in a coord[] with first index topLeft, sec topRight, third buttomLeft, etc.
    private void SpawnCorners(Vector3[] target_coords)
    {
        DestroyCorners();
        
        spawnedCorners.Add(Instantiate(Top_left_corner, target_coords[0], Quaternion.identity));
        spawnedCorners.Add(Instantiate(Top_right_corner, target_coords[1], Quaternion.identity));
        spawnedCorners.Add(Instantiate(Buttom_left_corner, target_coords[2], Quaternion.identity));
        spawnedCorners.Add(Instantiate(Buttom_right_corner, target_coords[3], Quaternion.identity));
    }

    private void DestroyCorners()
    {
        foreach (var corner in spawnedCorners)
        {
            Destroy(corner);
        }
        
        spawnedCorners.Clear();
    }
}
