using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Settings2 : MonoBehaviour
{
    [SerializeField] GameObject Audio;
    [SerializeField] GameObject Controls;
    [SerializeField] GameObject Credits;

    [SerializeField] GameObject Keyboard;
    [SerializeField] GameObject Controller;

    [SerializeField] GameObject Setting;

    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject pauseMenu;

    void Start()
    {
        CloseAll();
    }

    void CloseAll()
    {
        Audio.SetActive(false);
        Controls.SetActive(false);
        Credits.SetActive(false);
    }


    //public void CloseSettings()
    //{ 
    //    if (inputs.PauseMenu)
    //    {
    //        settingsMenu.SetActive(false);
    //        pauseMenu.SetActive(true);
    //    }
    //}


    public void OpenAudio()
    {
        CloseAll();
        Audio.SetActive(true);
    }

    public void OpenControls()
    {
        CloseAll();
        Controls.SetActive(true);

        Keyboard.SetActive(false);
        Controller.SetActive(false);
    }

    public void OpenCredits()
    {
        CloseAll();
        Credits.SetActive(true);
        Setting.SetActive(false);
    }

    public void ControllerControls()
    {
        Keyboard.SetActive(false);
        Controller.SetActive(true);
    }

    public void KeyboardControls()
    {
        Controller.SetActive(false);
        Keyboard.SetActive(true);
    }
}