using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartNav()
    {
        SceneManager.LoadScene(1);  //  Start Navigation
    }
    public void GoToSettings()
    {
        SceneManager.LoadScene(2);  // Settings
    }
    public void QuitApp()
    {
        Application.Quit();     // Quit 
    }
}
