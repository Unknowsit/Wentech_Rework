using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadSceneAsync("Operators"); // Transition Screen
    }

    public void Back()
    {
        SceneManager.LoadSceneAsync("Mainmenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}