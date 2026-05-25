using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject Controls;
    [SerializeField] GameObject Keyboard;
    [SerializeField] GameObject Controller;

    bool isPaused = false;

    void Start()
    {
        //pauseMenu.SetActive(false);
        CloseControl();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        Debug.Log("It was Pressed");

    //        if (isPaused)
    //        {
    //            ResumeGame();
    //        }
    //        else
    //        {
    //            PauseGame();
    //        }
    //    }
    //}

    //void PauseGame()
    //{
    //    pauseMenu.SetActive(true);

    //    Time.timeScale = 0f;
    //    isPaused = true;

    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.None;
    //}

    //public void ResumeGame()
    //{
    //    pauseMenu.SetActive(false);

    //    Time.timeScale = 1f;
    //    isPaused = false;

    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //}

    public void GoToSettings()
    {
        SceneHistory.previousScene = SceneManager.GetActiveScene().name;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Settings");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void CloseControl()
    {
        Controls.SetActive(false);
        Keyboard.SetActive(false);
        Controller.SetActive(false);
    }

    public void ControllerControls()
    {
        CloseControl();
        Controller.SetActive(true);
    }

    public void KeyboardControls()
    {
        CloseControl();
        Keyboard.SetActive(true);
    }
}