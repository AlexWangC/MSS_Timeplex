using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class scrPanel : MonoBehaviour
{
    // stores info about relative time here;
    public int Time_index;  // manual initialization at spawn but managed through scripts laters.
    public bool Dead; //a bool to check if this panel is dead. If dead, disable dragging
    public GridMaker LocalGrid; // it is refreshed in the update method
    [HideInInspector] public Color originalColorPanel;
    public UnityEvent<scrPanel> OnMouseOverEvent;
    public UnityEvent<scrPanel> OnMouseExitEvent;
    
    [SerializeField] private ParticleSystem deathBleed; //reference in the prefab
    private ParticleSystem deathBleedInstance;
    
    private void Start()
    {
        Dead = false;
        originalColorPanel = this.GetComponent<SpriteRenderer>().color;
    }

    private void Update()
    {
        LocalGrid.CreateGrid();
    }

    public void PanelKilled() // what to do when this panel is dead.
    {
        // og code that sets the panel to black.
        // GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
        //lerp it to the color opaque to full.
        StartCoroutine(fadeToMaxOpaque(0.8f));
        spawnDeathParticles(getPanelPlayerLocation());
    }

    private void OnMouseExit()
    {
        Debug.Log("Mouse exit panel " + Time_index);
        OnMouseExitEvent?.Invoke(this);
    }

    private void OnMouseOver() //event
    {
        Debug.Log("Mouse over panel " + Time_index);
        OnMouseOverEvent?.Invoke(this);
        return;
    }

    private void OnMouseDown()
    {
        if (!Dead) // if this screen is not dead
        {
            Debug.Log("You've clicked panel " + gameObject.name);
            scrSwapManager swap_manager = FindAnyObjectByType<scrSwapManager>();
            scrSwapHighlightManager swap_highlight_manager = FindAnyObjectByType<scrSwapHighlightManager>();
            if (swap_manager != null)
            {
                switch (swap_manager.state)
                {
                    case scrSwapManager.SwappingState.NotSwapping:
                        break;
                    case scrSwapManager.SwappingState.FirstClick:
                        //disable panel's panel sprite, not to be affected by swap highlight
                        GetComponent<SpriteRenderer>().enabled = false;
                        
                        swap_manager.First_clicked_panel = this;
                        swap_manager.SetState(scrSwapManager.SwappingState.SecondClick);
                        break;
                    case scrSwapManager.SwappingState.SecondClick:
                        swap_manager.Second_clicked_panel = this;
                        swap_manager.SwapPanels(swap_manager.First_clicked_panel, swap_manager.Second_clicked_panel, 1);
                        swap_manager.First_clicked_panel = null;
                        swap_manager.Second_clicked_panel = null;
                        swap_manager.SetState(scrSwapManager.SwappingState.FirstClick);
                        break;
                }
            }
        }
    }

    // this is the coroutine that fades the panel's sprite renderer's sprite to max opaque.
    private IEnumerator fadeToMaxOpaque(float fadeDuration)
    {
        SpriteRenderer sprite_renderer = GetComponent<SpriteRenderer>();
        if (sprite_renderer == null)
        {
            Debug.LogError("SpriteRenderer component not found!");
            yield break;
        }
        
        Color current_color = sprite_renderer.color;
        float elapsed_time = 0f;

        while (elapsed_time < fadeDuration)
        {
            elapsed_time += Time.deltaTime;
            float new_alpha = Mathf.Lerp(current_color.a, 1.0f, elapsed_time / fadeDuration);
            sprite_renderer.color = new Color(current_color.r, current_color.g, current_color.b, new_alpha);
            yield return null;
        }

        sprite_renderer.color = new Color(current_color.r, current_color.g, current_color.b, 1.0f);
    }

    private Vector3 getPanelPlayerLocation()
    {
        return GetComponentInChildren<scrPlayer>().transform.position;
    }
    
    private void spawnDeathParticles(Vector3 player_location) // the particle for player death
    {
        deathBleedInstance = Instantiate(deathBleed, player_location, Quaternion.identity);
    }
}
