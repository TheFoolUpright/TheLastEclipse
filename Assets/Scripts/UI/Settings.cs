using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject Audio;
    [SerializeField] GameObject Controls;
    [SerializeField] GameObject Credits;

    [SerializeField] GameObject Keyboard;
    [SerializeField] GameObject Controller;

    [SerializeField] GameObject Setting;
    [SerializeField] GameObject Graphics;

    void Start()
    {
        CloseAll();
    }

    void Update()
    {
        // terug naar vorige scene
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!string.IsNullOrEmpty(SceneHistory.previousScene))
            {
                SceneManager.LoadScene(SceneHistory.previousScene);
            }
            else
            {
                Debug.LogWarning("Previous scene is EMPTY!");
            }
        }
    }

    void CloseAll()
    {
        Audio.SetActive(false);
        Controls.SetActive(false);
        Credits.SetActive(false);
        Graphics.SetActive(false);
    }

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