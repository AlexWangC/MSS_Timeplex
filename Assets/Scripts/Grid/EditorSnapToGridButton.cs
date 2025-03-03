using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorSnapToGridButton
{    static EditorSnapToGridButton()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 200, 50)); // Position in Scene View

        if (GUILayout.Button("Snap All to Grid", GUILayout.Height(30)))
        {
            SnapAllToGrid();
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void SnapAllToGrid()
    {
        scrGridMakerTilted[] panels = Object.FindObjectsByType<scrGridMakerTilted>(FindObjectsSortMode.None);
        foreach (scrGridMakerTilted panel in panels)
        {
            panel.SnapToGrid();
        }

        Debug.Log("All objects snapped to grid!");
    }
}

