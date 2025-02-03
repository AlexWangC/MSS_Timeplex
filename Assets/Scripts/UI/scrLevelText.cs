using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrLevelText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().text = "Level " + (SceneManager.GetActiveScene().buildIndex) + " / " + (SceneManager.sceneCountInBuildSettings - 1);
    }
}
