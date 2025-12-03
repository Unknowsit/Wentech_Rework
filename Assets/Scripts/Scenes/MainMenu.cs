using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    public void NewGame()
    {
        audioManager.PlaySFX("SFX04");
        StartCoroutine(LoadNewGameWithDelay());
    }

    IEnumerator LoadNewGameWithDelay()
    {
        yield return new WaitForSeconds(1f);  
        SceneManager.LoadSceneAsync("Operators");
    }

    public void Setting()
    {
        audioManager.PlaySFX("SFX04");
    }

    public void SoundButtotInTutorial()
    {
        audioManager.PlaySFX("SFX04");
    }

    public void Tutorial()
    {
        audioManager.PlaySFX("SFX04");
        StartCoroutine(TutorialDelay());
    }

    IEnumerator TutorialDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("TutorialS");
    }

    public void Back()
    {
        audioManager.PlaySFX("SFX04");
        SceneManager.LoadSceneAsync("Mainmenu");
    }

    public void ExitGame()
    {
        audioManager.PlaySFX("SFX04");
        Application.Quit();
    }
}
