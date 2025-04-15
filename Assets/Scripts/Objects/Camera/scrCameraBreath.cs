using UnityEngine;

public class scrCameraBreath : MonoBehaviour
{
    // could be changed to public later
    private float idleAmplitude = 1f;
    private float frequency = 0.2f;
    [SerializeField] private float fadeDuration = 1f;

    private bool breathing;
    private float currentBreathMultiplier = 0f;
    
    [HideInInspector] public Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
        breathing = true;
    }

    void Update()
    {
        // Gradually update the multiplier based on whether breathing is enabled.
        if (breathing)
        {
            currentBreathMultiplier = Mathf.Min(currentBreathMultiplier + Time.deltaTime / fadeDuration, 1f);
        }
        else
        {
            currentBreathMultiplier = Mathf.Max(currentBreathMultiplier - Time.deltaTime / fadeDuration, 0f);
        }

        // Apply the breathing effect only if there's some intensity.
        if (currentBreathMultiplier > 0f)
        {
            Breath();
        }
    }

    void Breath()
    {
        float offsetX = (Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f) * idleAmplitude * 2f * currentBreathMultiplier;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f) * idleAmplitude * 2f * currentBreathMultiplier;
        transform.position = initialPosition + new Vector3(offsetX, offsetY, 0f);
    }

    public void ContinueBreath()
    {
        breathing = true;
    }
    
    public void StopBreath()
    {
        breathing = false;
    }
}
