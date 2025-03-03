using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class SnapToGridButton : EditorWindow
{
    [InitializeOnLoadMethod]
    private static void AutoShowWindow()
    {
        GetWindow<SnapToGridButton>("SnapToGridButton");
    }

    [MenuItem("Snap all Objects to Grid")]
    public static void SnapAllToGrid()
    {
        scrGridMakerTilted[] panels = FindObjectsByType<scrGridMakerTilted>(FindObjectsSortMode.None);
        foreach (scrGridMakerTilted panel in panels)
        {
            panel.SnapToGrid();
        }
    }

         private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Snap all Objects to Grid", EditorStyles.boldLabel);

        if (GUILayout.Button("Click Me!", GUILayout.Height(40)))
        {
            Debug.Log("Snap all Objects to Grid button clicked!");
            SnapAllToGrid();
        }
    }
}

