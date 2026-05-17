using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Demo");
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