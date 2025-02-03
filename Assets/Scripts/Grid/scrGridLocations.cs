using System;
using System.Linq;
using UnityEngine;

//okay i don't really know why do i need this script after doing all swapping in swap manager.
//might come in handy later.
public class scrGridLocations : MonoBehaviour
{
    // storing the locations of the 4 grid systems
    private float[] Coordinates; // [x1, y1, x2, y2, x3, y3, x4, y4, ......]
    public float Coordinates_Z; // the z-axis value shared by the coordinates
    [HideInInspector] public scrPanel[] panels; 

    private void Awake() //in awake because this script initializes all info of the grid.
    {
        ObtainGridPositions();
        synchronizeCoordinates();
    }

    // These two functions go hand-in-hand
    public bool ObtainGridPositions() // feeds grid positions to panels array.
    {
        panels = FindObjectsByType<scrPanel>(FindObjectsSortMode.InstanceID);
        panels = panels.Reverse().ToArray(); // for some reason the above sortmode gets panel4 first.
        return verifyGridPositions();
    }

    private bool verifyGridPositions()
    {
        if (panels != null && panels.Length > 0)
        {
            Debug.Log("Finding Panels... \nNumber of Panels Found: " + panels.Length);
            for (int i = 0; i < panels.Length; i++)
            {
                Debug.Log($"Panel {i}: {panels[i].name}");
            }
            return true;
        }
        else
        {
            Debug.LogWarning("No panels found!");
            return false;
        }
    }
    // These two functions go hand-in-hand

    private void synchronizeCoordinates()
    {
        Coordinates = new float[panels.Length * 2]; //it's a float array twice the length of panels
        
        for (int i = 0; i < Coordinates.Length; i = i + 2) // only at index 0, 2, 4
        {
            Coordinates[i] = panels[i/2].transform.position.x;
            Coordinates[i + 1] = panels[i/2].transform.position.y;
        }
    }
    
}
