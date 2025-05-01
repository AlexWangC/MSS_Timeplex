using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODOneShotSpeaker : MonoBehaviour
{

    public string parameterName; // Name of FMOD parameter
    public string volumeParameterName; // Name of FMOD volume parameter

    public bool playOnWake; // If audio plays on instantiation

    public string triggerTag; // Name of Trigger Gameobject Tag (if any)

    public float smoothingSpeed = 2f; // Speed of parameter transition

    public EventReference fmodEvent; // Assign FMOD Event in Inspector
    private EventInstance eventInstance;
    FMOD.Studio.PARAMETER_ID eventParameter;
    FMOD.Studio.PARAMETER_ID volumeParameter;

    [Range(0f, 1f)]
    public float currentVolume;
    [Range(0f, 1f)]
    public float targetVolume; // (Modify this to set starting volume)
    public float currentValue = 0f; // Current value of parameter
    public float targetValue = 0f; // Target value of parameter 

    void Start() // Instantiate FMOD Instance
    {
        DontDestroyOnLoad(this.gameObject); // Music / sound will not stop when switching scenes

        eventInstance = RuntimeManager.CreateInstance(fmodEvent);

        FMOD.Studio.EventDescription eventDescription;
        eventInstance.getDescription(out eventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION eventParameterDescription;
        eventDescription.getParameterDescriptionByName(parameterName, out eventParameterDescription);
        eventParameter = eventParameterDescription.id;

        FMOD.Studio.EventDescription volumeDescription;
        eventInstance.getDescription(out volumeDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION volumeParameterDescription;
        volumeDescription.getParameterDescriptionByName(volumeParameterName, out volumeParameterDescription);
        volumeParameter = volumeParameterDescription.id;

        if (playOnWake)
        {
            PlaySound();
        }
    }

    void Update()
    {
        // Smoothly transition towards the target value
        currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothingSpeed);
        eventInstance.setParameterByID(eventParameter, currentValue);
        // Smoothly transition towards the target volume
        currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothingSpeed);
        eventInstance.setParameterByID(volumeParameter, currentVolume);
    }

    public void PlaySound() // Play sound on call
    {
        if (eventInstance.isValid())
        {
            eventInstance.start();
        }
    }

    public void StopSound() // Stop sound on call
    {
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstance.release();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // Play sound on trigger enter with object of associated tag
    {
        if (triggerTag != null)
        {
            if (other.CompareTag(triggerTag)) // Only trigger if the object has the associated tag
            {
                PlaySound();
            }
        }
    }

    public void SetTargetParameter(float newValue) // Set target value for parameter manipulation (if any)
    {
        targetValue = newValue;
    }

    public void SetVolumeParameter(float newValue) // Set target value for parameter manipulation (if any)
    {
        targetVolume = newValue;
    }

    void OnDestroy() // Stop sound on destroy
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}


