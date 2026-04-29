using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public static bool isPaused;

    void Start()
    {
        // pauseMenu.SetActive(false);
        // isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
    }



}
