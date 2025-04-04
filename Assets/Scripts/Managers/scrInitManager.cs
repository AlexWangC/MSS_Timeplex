using System;
using UnityEngine;

public class scrInitManager : MonoBehaviour
{
    private Vector2 _lastScreenSize = Vector2.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lastScreenSize = new Vector2(Screen.width, Screen.height);
        
        UpdateScreenSize();
    }

    private void Update()
    {
        // Check if the screen size has changed
        if (_lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
        {
            _lastScreenSize = new Vector2(Screen.width, Screen.height);
            UpdateScreenSize();
        }
    }

    private void UpdateScreenSize()
    {
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        int w, h;
        w = Screen.width;
        h = Screen.height;
        if (h > w * 9 / 16)
            h = w * 9 / 16;
        else
            w = h * 16 / 9;
        Screen.SetResolution(w,h, FullScreenMode.MaximizedWindow);
    }
}
