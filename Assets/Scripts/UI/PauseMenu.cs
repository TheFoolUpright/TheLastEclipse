using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using StarterAssets;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu; 
    [SerializeField] GameObject Controls;
    [SerializeField] GameObject keyboardUI;
    [SerializeField] GameObject Controller;


    MenuFogToggle fogToggle;

    private StarterAssetsInputs inputs;



    bool isPaused = false;

    private void Awake()
    {
        Controls.SetActive (false);

        inputs = FindFirstObjectByType<StarterAssetsInputs>();

        if (inputs == null)
            Debug.LogError("StarterAssetsInputs NOT FOUND!");
    }

    void Start()
    {
        fogToggle = GetComponent<MenuFogToggle>(); 

        if (fogToggle == null)
            
        pauseMenu.SetActive(false);
        CloseControl();
    }
    void Update()
    {
        if (inputs.PauseMenu)
        {
            inputs.PauseMenu = false;

            if (settingsMenu.activeSelf)
            {
                CloseSettings(); 
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        fogToggle.SetFogEnabled(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ThirdPersonController tpc = inputs.GetComponent<ThirdPersonController>();
        if (tpc != null) tpc.enabled = false;
        PlayerCamera pCam = inputs.GetComponent<PlayerCamera>();
        if (pCam != null) pCam.enabled = false;
        PlayerController pCon = inputs.GetComponent<PlayerController>();
        if (pCon != null) pCon.enabled = false;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        fogToggle.SetFogEnabled(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ThirdPersonController tpc = inputs.GetComponent<ThirdPersonController>();
        if (tpc != null) tpc.enabled = true;
        PlayerCamera pCam = inputs.GetComponent<PlayerCamera>();
        if (pCam != null) pCam.enabled = true;
        PlayerController pCon = inputs.GetComponent<PlayerController>();
        if (pCon != null) pCon.enabled = true;
    }
    public void GoToSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    //public void OpenControls()
    //{
    //    Controls.SetActive(true);

    //    CloseControl();
    //}
    void CloseControl()
    {
        keyboardUI.SetActive(false);
        Controller.SetActive(false);
    }

    //public void ControllerControls()
    //{
    //    CloseControl();
    //    Controller.SetActive(true);
    //}

    //public void KeyboardControls()
    //{
    //    CloseControl();
    //    keyboardUI.SetActive(true);
    //}

    public void OpenControls()
    {
        Controls.SetActive(true);

        keyboardUI.SetActive(false);
        Controller.SetActive(false);
    }

    public void ControllerControls()
    {
        keyboardUI.SetActive(false);
        Controller.SetActive(true);
    }

    public void KeyboardControls()
    {
        Controller.SetActive(false);
        keyboardUI.SetActive(true);
    }
}