using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    private void Update()
    {
        IntroGame();
    }

    private void IntroGame()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync("MainMenu"); // Transition Screen to Main Menu
        }
    }
}