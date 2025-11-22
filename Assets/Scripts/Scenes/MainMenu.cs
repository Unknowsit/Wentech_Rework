using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;
    private Animation anim;

    private void Start()
    {
        audioManager = AudioManager.instance;
        anim = GetComponent<Animation>();
    }

    public void NewGame()
    {
        audioManager.PlaySFX("SFX04");
        SceneManager.LoadSceneAsync("Operators"); // Transition Screen
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