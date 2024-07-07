using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace "GameScene" with your game scene name
    }

    public void OpenSettings()
    {
        // Logic to open settings menu
        Debug.Log("Settings button clicked");
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked");
        Application.Quit();
    }
}
