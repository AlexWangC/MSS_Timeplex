using UnityEngine;
using NaughtyAttributes;

public class scrGridMakerTilted : MonoBehaviour
{
    [Header("Grid Settings")]
    public int numBlocksX = 10;
    public int numBlocksY = 10;
    public float blockWidth = 1f;
    public float blockHeight = 1f;

    [Header("Tilt Angles (Degrees)")]
    public float xAxisTilt = 30f;
    public float yAxisTilt = -30f;

    [Header("Line Settings")]
    public Material lineMaterial;
    public Color lineColor = Color.white;
    public float lineWidth = 0.05f;


    [Button("Refresh Grid")]
    public void CreateGrid()
    {
        ClearGrid();
        GenerateGrid();
    }

    public Vector3 GetWorldPositionFromGrid(Vector2 gridPos)
    {
        float xTiltRad = Mathf.Deg2Rad * xAxisTilt;
        float yTiltRad = Mathf.Deg2Rad * yAxisTilt;

        // Compute the basis vectors for the tilted grid
        Vector3 xMother = new Vector3(Mathf.Cos(xTiltRad) * blockWidth, Mathf.Sin(xTiltRad) * blockWidth, 0);
        Vector3 yMother = new Vector3(Mathf.Cos(yTiltRad) * blockHeight, Mathf.Sin(yTiltRad) * blockHeight, 0);

        // Calculate world position
        Vector3 worldPos = transform.position + (gridPos.x * xMother) + (gridPos.y * yMother);
        return worldPos;
    }
    
    private void GenerateGrid()
    {
        Vector3 origin = transform.position;
        float xTiltRad = Mathf.Deg2Rad * xAxisTilt;
        float yTiltRad = Mathf.Deg2Rad * yAxisTilt;

        Vector3 xMother = new Vector3(Mathf.Cos(xTiltRad) * blockWidth, Mathf.Sin(xTiltRad) * blockWidth, 0);
        Vector3 yMother = new Vector3(Mathf.Cos(yTiltRad) * blockHeight, Mathf.Sin(yTiltRad) * blockHeight, 0);

        for (int i = 0; i <= numBlocksY; i++)
        {
            Vector3 start = origin + i * yMother;
            Vector3 end = start + numBlocksX * xMother;
            DrawLine(start, end, "X_Line_" + i);
        }

        for (int j = 0; j <= numBlocksX; j++)
        {
            Vector3 start = origin + j * xMother;
            Vector3 end = start + numBlocksY * yMother;
            DrawLine(start, end, "Y_Line_" + j);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, string name)
    {
        GameObject lineGO = new GameObject(name);
        lineGO.transform.parent = this.transform;

        LineRenderer line = lineGO.AddComponent<LineRenderer>();
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
        line.SetPositions(new Vector3[] { start, end });

        if (lineMaterial != null)
            line.material = lineMaterial;
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}

