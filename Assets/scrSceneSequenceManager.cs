using UnityEngine;
using UnityEngine.SceneManagement;

public class scrSceneSequenceManager : MonoBehaviour
{//Make it a persistant Singleton 
    public static scrSceneSequenceManager Instance;
    [HideInInspector] public scrGoal[] doors;
    public static Scene thisScene;
    public static Scene lastScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            thisScene = SceneManager.GetActiveScene();
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("New scene loaded: " + scene.name);

        //update last scene record
        if (thisScene != SceneManager.GetActiveScene())
        {
            lastScene = thisScene;
            thisScene = SceneManager.GetActiveScene();
        }
    }

}
