using System;
using UnityEngine;

public class scrSwapHighlightManager : MonoBehaviour
{
    public scrPanel First_Clicked_Panel;
    public scrSwapManager swap_manager;
    
    
    public void PanelHovered(scrPanel panel)
    {
        if (panel.Dead)
        {
            return;
        }
       Debug.Log("Panel hovered Invoked at panel " + panel.Time_index);
       panel.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    public void PanelExited(scrPanel panel)
    {
        if (panel.Dead)
        {
            return;
        }
        if (panel.GetComponent<SpriteRenderer>().enabled)
        {
            panel.GetComponent<SpriteRenderer>().color = panel.originalColorPanel;
        }
    }
}
