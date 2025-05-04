using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJumper : MonoBehaviour {

    public string sceneName;

    public void jump() {
        SceneManager.LoadScene(sceneName);
    }
    
}
