using System;
using UnityEngine;

public class scrSoundManager : MonoBehaviour
{
    public static scrSoundManager Instance; // the singleton of sound manager.

    [SerializeField] private AudioSource audio_source;
    [SerializeField] private AudioClip theme;
    [SerializeField] public AudioClip hit_wall; //yes
    [SerializeField] public AudioClip walk; //yes
    [SerializeField] public AudioClip time_swap;
    [SerializeField] public AudioClip hurt; //yes
    [SerializeField] public AudioClip level_clear;
    [SerializeField] public AudioClip goal; //yes 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
        }
    }

    private void Start()
    {
        //plays the main theme
        PlaySound(theme, transform, 0.1f, true);
    }

    public void PlaySound(AudioClip audio_clip, Transform spawn_transform, float volume, bool on_loop)
    {
        // spawn game object
        AudioSource audio_source = Instantiate(this.audio_source, spawn_transform.position, Quaternion.identity);

        // assign the audioClip
        audio_source.clip = audio_clip;
        
        // assign volume
        audio_source.volume = volume;
        
        // assign on repeat
        audio_source.loop = on_loop;

        // play sound
        audio_source.Play();

        if (on_loop)
        {
            //set it to persistent, not destroyed when scene reloads/changes
            DontDestroyOnLoad(audio_source.gameObject);
        }
        
        // play sound
        audio_source.Play();

        if (!on_loop)
        {
            // get length of sound clip
            float clip_length = audio_source.clip.length;
            // destroy clip after playing
            Destroy(audio_source.gameObject, clip_length);
        }
    }

    public void PlaySound(AudioClip audio_clip, Transform spawn_transform, float volume)
    {
        // spawn game object
        AudioSource audio_source = Instantiate(this.audio_source, spawn_transform.position, Quaternion.identity);

        // assign the audioClip
        audio_source.clip = audio_clip;
        
        // assign volume
        audio_source.volume = volume;

        // play sound
        audio_source.Play();
        
        // get length of sound clip
        float clip_length = audio_source.clip.length;

        // destroy clip after playing
        Destroy(audio_source.gameObject, clip_length);
    }
}
