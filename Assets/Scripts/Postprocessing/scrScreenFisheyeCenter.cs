using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class scrScreenFisheyeCenter : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 normalizedPosition = new Vector2(
            mousePosition.x / Screen.width,
            mousePosition.y / Screen.height
        );

        // Remap normalized [0, 1] to [-0.2, 0.2]
        float remappedX = Mathf.Lerp(-0.2f, 0.2f, normalizedPosition.x);
        float remappedY = Mathf.Lerp(-0.2f, 0.2f, normalizedPosition.y);

        this.GetComponent<PostProcessVolume>().profile.GetSetting<LensDistortion>().centerX.value = remappedX;
        this.GetComponent<PostProcessVolume>().profile.GetSetting<LensDistortion>().centerY.value = remappedY;
    }

}
