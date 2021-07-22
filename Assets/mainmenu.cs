using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Starts the game.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
