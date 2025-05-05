using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioDirector : MonoBehaviour
{
    public static AudioDirector Instance;

    public AudioSource musicSource;
    public AudioSource titleSource;

    [Header("Game Sound Effects")]
    public AudioClip blockSound;
    public FMODUnity.EventReference blockSoundFMOD;
    public float blockVolume = 1.0f;

    public AudioClip footstepSound;
    public FMODUnity.EventReference footstepSoundFMOD;
    public float footstepVolume = 1.0f;

    public AudioClip hurtSound;
    public FMODUnity.EventReference hurtSoundFMOD;
    public float hurtVolume = 1.0f;

    public AudioClip timeswapSound;
    public FMODUnity.EventReference timeswapSoundFMOD;
    public float timeswapVolume = 1.0f;

    public AudioClip levelclearSound;
    public FMODUnity.EventReference levelclearSoundFMOD;
    public float levelclearVolume = 1.0f;

    [Header("Background")]
    public AudioClip backgroundMusic;
    public float musicVolume = 1.0f;

    public AudioClip titleMusic;
    public float titleVolume = 1.0f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();

        titleSource.clip = titleMusic;
        titleSource.volume = 1f;
        titleSource.Play();

    }

    public void PlaySound(FMODUnity.EventReference eventReference)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);

    }
}
