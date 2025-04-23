using System;
using System.Collections;
using System.Collections.Generic;
using Fries.TaskPerformer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class scrSwapHighlightManager : MonoBehaviour
{
    public scrPanel First_Clicked_Panel;
    public scrSwapManager swap_manager;

    public GameObject Top_left_corner;
    public GameObject Top_right_corner;
    public GameObject Buttom_left_corner;
    public GameObject Buttom_right_corner;

    public Material Panel_one_mat;
    public Material Panel_two_mat;
    public Material Panel_three_mat;
    public Material Panel_four_mat;

    private List<GameObject> spawnedCorners = new List<GameObject>();
    
    private float standard_aber_alpha = (float)1.0f;
    private float highlight_aber_alpha = (float) 1.5f;
    private float highlight_duration = (float) 5.0f;

    private void Start()
    {
    }

    private void Update()
    {
        
    }

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
        
        HighlightPanel(panel);
        
        // highlight effect
        // StartCoroutine(HighlightPanel());
        //TaskPerformer.inst().executeIEnumerator(HighlightPanel(panel));

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
        
        DelightPanel(panel);

        //TaskPerformer.inst().executeIEnumerator(DelightPanel(panel));

        // Legacy Highlight effect of changing the color.a of a panel.
        /*
        if (panel.GetComponent<SpriteRenderer>().enabled)
        {
            panel.GetComponent<SpriteRenderer>().color = panel.originalColorPanel;
        }
        */
    }

    #region CornerSpawn
    
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
    #endregion

    #region Highlight/Delight
    
    private Material findPanelMat(scrPanel panel)
    {
        return panel.GetComponent<SpriteRenderer>().material;
    }

    private void HighlightPanel(scrPanel panel)
    {
        findPanelMat(panel).DOFloat(highlight_aber_alpha, Shader.PropertyToID("_AberrationAlpha"), highlight_duration);
    }

    private void DelightPanel(scrPanel panel)
    {
        findPanelMat(panel).DOFloat(standard_aber_alpha, Shader.PropertyToID("_AberrationAlpha"), highlight_duration);
    }

    /*
    private IEnumerator HighlightPanel(scrPanel panel)
    {
        float time_passed = 0f;

        while (time_passed < highlight_duration)
        {
            time_passed += Time.deltaTime;
            
            // curve edition here
            float t = Mathf.Clamp01(time_passed / highlight_duration);  
            
            SetIntensity(findPanelMat(panel), Mathf.Lerp(standard_light_intensity, highlight_intensity, t));
            yield return null;
        }
        SetIntensity(findPanelMat(panel), highlight_intensity);
    }

    private IEnumerator DelightPanel(scrPanel panel)
    {
        float timePassed        = 0f;

        while (timePassed < highlight_duration)
        {
            timePassed += Time.deltaTime;                           // advance the timer each frame :contentReference[oaicite:0]{index=0}
            
            float t = Mathf.Clamp01(timePassed / highlight_duration);         // normalised 0-1 progress :contentReference[oaicite:1]{index=1}
            
            SetIntensity(findPanelMat(panel),
                Mathf.Lerp(highlight_intensity, standard_light_intensity, t)); // smooth step back down :contentReference[oaicite:2]{index=2}
            yield return null;                                      // wait until the next frame :contentReference[oaicite:3]{index=3}
        }

        // Make sure we finish exactly on the baseline value
        SetIntensity(findPanelMat(panel), standard_light_intensity);
    }

    private Light2D findGlobalLight()
    {
        Light2D[] all_lights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);

        foreach (var light in all_lights)
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                return light;
            }
        }

        return null;
    }

    private Material findPanelMat(scrPanel panel)
    {
        return panel.GetComponent<SpriteRenderer>().material;
    }
    
    private void SetIntensity(Material target_mat, float target)
    {
        target_mat.SetFloat(Shader.PropertyToID("_AberrationAlpha"), target);
    }
    */

    #endregion
}
