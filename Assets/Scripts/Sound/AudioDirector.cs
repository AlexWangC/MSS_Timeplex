using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Fries.Inspector.SceneBehaviours;
using Menu;

public class AudioDirector : MonoBehaviour
{
    private static AudioDirector Instance;
    public static AudioDirector Inst() 
    {
        if (Instance == null) create();
        return Instance;
    }

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

    public AudioClip enemyFootstepSound;
    public FMODUnity.EventReference enemyFootstepSoundFMOD;
    public float enemyFootstepVolume = 1.0f;

    public AudioClip portalSound;
    public FMODUnity.EventReference portalSoundFMOD;
    public float portalVolume = 1.0f;

    public AudioClip pickupSound;
    public FMODUnity.EventReference pickupSoundFMOD;
    public float pickupVolume = 1.0f;
    
    public AudioClip killGuardSound;
    public FMODUnity.EventReference killGuardFMOD;
    public float killGuardVolume = 1.0f;

    [Header("Background")]
    public AudioClip backgroundMusic;
    public FMODUnity.EventReference backgroundMusicFMOD;
    public float musicVolume = 1.0f;

    public AudioClip titleMusic;
    public FMODUnity.EventReference titleMusicFMOD;
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
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(FMODUnity.EventReference eventReference)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventReference);

    }

    public FMOD.Studio.EventInstance ei;

    [StartSceneAwakeEvent]
    public static void OnStartSceneAwake()
    {
        create();
        AudioDirector.Instance.ei = FMODUnity.RuntimeManager.CreateInstance("event:/MUSIC_Title");
        AudioDirector.Instance.ei.start();
    }

    public static void create()
    {
        GameObject gop = Resources.Load<GameObject>("AudioDirector");
        GameObject.Instantiate(gop);
    }
}
