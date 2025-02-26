using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

public class scrSwapManager : MonoBehaviour
{
    public enum SwappingState
    {
        NotSwapping,
        FirstClick,
        SecondClick
    };
    
    public SwappingState state { get; set; }
    [HideInInspector] public scrPanel First_clicked_panel;
    [HideInInspector] public scrPanel Second_clicked_panel;

    private scrGridLocations grid_locations;
    private scrPanel[] all_panels;
    private GridObject[] all_grid_objects;
    
    private void Start() // temporarily setting default swapping state to waiting for first click.
    {
        state = SwappingState.FirstClick;
        First_clicked_panel = null;
        Second_clicked_panel = null;

        getPanels();
        getGridObjects();
    }

    private void Update()
    {
        //Debug.Log("current state: " + state);
    }

    // Invoked by other functions.
    public void SetState(SwappingState newState)
    {
        state = newState;
    }

    //this is called by panels.
    public void SwapPanels(scrPanel panel0, scrPanel panel1, float duration)
    {
        getGridObjects();
        Vector3 panel0_position_og = getCoordinates(panel0);
        Vector3 panel1_position_og = getCoordinates(panel1);
        
        //play sound
        scrSoundManager.Instance.PlaySound(scrSoundManager.Instance.time_swap, this.transform, 30f);
        
        movePanel(panel1_position_og, duration, panel0);
        movePanel(panel0_position_og, duration, panel1);
        
        // updates time index here as well
        int temp = panel0.Time_index;
        panel0.Time_index = panel1.Time_index;
        panel1.Time_index = temp;
        
        //reenable all panel sprites, no highlight
        foreach (scrPanel panel in all_panels)
        {
            panel.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    //this is called by panels.

    /*
     * Below are the helper functions
     */
    
    private Vector3 getCoordinates(scrPanel panel)
    {
        return panel.transform.position;
    }

    private scrPanel[] getPanels()
    {
        all_panels = GetComponent<scrGridLocations>().panels;
        if (all_panels == null)
        {
            Debug.Log("no panels found from scrGridlocations");
        }

        return all_panels;
    }

    private GridObject[] getGridObjects()
    {
        all_grid_objects = FindObjectsByType<GridObject>(FindObjectsSortMode.None);
        return all_grid_objects;
    }
    
    //these two go hand-in-hand, helpers
    private bool movePanel(Vector3 destination, float duration, scrPanel panel)
    {
        StartCoroutine(smoothMove(destination, duration, panel));
        return true;
    }

    private IEnumerator smoothMove(Vector3 destination, float duration, scrPanel panel)
    {
        Vector3 startPosition = panel.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            updateAllPanelsPosition();
            updateAllGridObjectPosition();
            panel.transform.position = Vector3.Lerp(startPosition, destination, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        panel.transform.position = destination; // Ensure the position is set to the exact destination
    }

    private void updateAllPanelsPosition()
    {
        foreach (scrPanel panel_in_array in all_panels)
        {
            panel_in_array.GetComponentInChildren<scrGridMakerTilted>().CreateGrid();
        }
    }
    
    private void updateAllGridObjectPosition()
    {
        foreach (GridObject grid_object in all_grid_objects)
        {
            grid_object.UpdatePosition();
        }
    }
    //these two go hand-in-hand
}
