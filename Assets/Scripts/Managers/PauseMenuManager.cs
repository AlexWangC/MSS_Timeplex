using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public bool isPaused;
    public GameObject pauseMenu;
    public GameObject pauseMenuPrefab;
    public Button resumeButton;
    public Button menuButton;

    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        Debug.Log(pauseMenu);
        /*
        if (pauseMenu == null)
        {
            InstantiatePauseMenu();
        }
        */
        InitializePauseMenu();
    }


    void Update()
    {
        // if press esc, toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (isPaused)
        {
            DeactivatePauseMenu();
        }
        else
        {
            ActivatePauseMenu();
        }
    }

    void InstantiatePauseMenu()
    {
        pauseMenu = Instantiate(pauseMenuPrefab);
        pauseMenu.transform.SetParent(GameObject.Find("Canvas").transform);
        pauseMenu.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void InitializePauseMenu()
    {
        resumeButton = pauseMenu.transform.Find("ResumeButton").GetComponent<Button>();
        menuButton = pauseMenu.transform.Find("MenuButton").GetComponent<Button>();
        resumeButton.onClick.AddListener(DeactivatePauseMenu);
        menuButton.onClick.AddListener(LoadMenu);
        DeactivatePauseMenu();
    }

    public void ActivatePauseMenu()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        print("Activated pause menu");
    }

    public void DeactivatePauseMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        print("Deactivated pause menu");
    }

    public void LoadMenu()
    {
        DeactivatePauseMenu();
        SceneManager.LoadScene("StartScene");
    }
}
