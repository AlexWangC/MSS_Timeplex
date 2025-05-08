using UnityEngine;

public class TitleMusic : MonoBehaviour
{
    public FMODUnity.EventReference titleMusic;

    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot(titleMusic);
    }
}
