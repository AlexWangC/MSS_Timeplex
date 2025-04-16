/*
 * Depending on how many panels are there, this script is going to zoom in at where the panels are.
 *
 * Attach to the camera in effect!
 */

using System.Collections;
using UnityEngine;

public class scrCameraZoom : MonoBehaviour
{
    public float targetOrthoSize = 5f;
    public float ZoomDuration = 1f;

    private float initOrthoSize;
    private Vector3 zoomTargetCoord;
    private Vector3 cameraOriginalCoord;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        initOrthoSize = mainCamera.orthographicSize;

        CalculateTargetCoord();
    }
    
    // The placeholding approach to test ZoomToTarget by zooming it to a clicked point
    /*
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            clickWorldPos.z = mainCamera.transform.position.z;
            zoomTargetCoord = clickWorldPos;
            
            StopAllCoroutines();
            StartCoroutine(ZoomToTarget());
        }
    }
    */

    Vector3 CalculateTargetCoord()
    {
        scrPanel[] panels = FindObjectsByType<scrPanel>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        int panel_count = panels.Length;
        
        // if only one panel..
        if (panel_count == 1)
        {
            zoomTargetCoord = panels[0].transform.position;
            // adjust the num here to adjust how much each block should shift
            zoomTargetCoord.x = zoomTargetCoord.x + FindAnyObjectByType<scrGridMakerTilted>(FindObjectsInactive.Exclude).numBlocksX * 0.5f; // adjust the float here for how much it shifts.
            zoomTargetCoord.y = zoomTargetCoord.y - 2f;
            StartCoroutine(ZoomToTarget());
            return zoomTargetCoord;
        }

        if (panel_count == 2)
        {
            targetOrthoSize = initOrthoSize - 0.5f;
            zoomTargetCoord.x = ((panels[0].transform.position.x + panels[1].transform.position.x) / 2) + (FindAnyObjectByType<scrGridMakerTilted>(FindObjectsInactive.Exclude).numBlocksX * 0.65f);
            //zoomTargetCoord.x = (panels[0].transform.position.x + panels[1].transform.position.x) / 2;
            zoomTargetCoord.y = panels[0].transform.position.y - 2f;
            zoomTargetCoord.z = panels[0].transform.position.z;
            StartCoroutine(ZoomToTarget());
            return zoomTargetCoord;
        }

        else
        {
            return new Vector3(-1, -1, -1);
        }
    }

    IEnumerator ZoomToTarget()
    {
        yield return new WaitForSeconds(3f);

        if (GetComponent<scrCameraBreath>() != null)
        {
            GetComponent<scrCameraBreath>().StopBreath();
        }

        // Record the starting position and size.
        Vector3 startPosition = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;
        
        Vector3 targetPosition = new Vector3(zoomTargetCoord.x, zoomTargetCoord.y, startPosition.z);

        float elapsedTime = 0f;
        
        while (elapsedTime < ZoomDuration)
        {
            // Calculate interpolation factor (0 to 1).
            float t = elapsedTime / ZoomDuration;
            // Optional: smooth the interpolation with smoothstep
            t = t * t * (3f - 2f * t);
            
            // Lerp the camera's position and orthographicSize
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetOrthoSize, t);

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame.
        }

        // Ensure the values are set exactly at the target after the loop.
        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = targetOrthoSize;

        if (GetComponent<scrCameraBreath>() != null)
        {
            GetComponent<scrCameraBreath>().initialPosition = mainCamera.transform.position;
            GetComponent<scrCameraBreath>().ContinueBreath();
        }
    }
}
