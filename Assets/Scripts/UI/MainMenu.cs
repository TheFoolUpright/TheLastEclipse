using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    void Start()
    {
        AudioManager.Instance.PlayMusic("MainMenu");
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Hub");
    }

    public void GoToSettings()
    {
        SceneHistory.previousScene = SceneManager.GetActiveScene().name;

        Debug.Log("Previous scene: " + SceneHistory.previousScene);

        SceneManager.LoadScene("Settings");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}