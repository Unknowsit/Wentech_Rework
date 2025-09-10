using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private bool isGamePaused = false;

    public Slider _bgmSlider, _sfxSlider;

    public void ToggleBGM()
    {
        AudioManager.instance.ToggleBGM();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void BGMVolume()
    {
        AudioManager.instance.BGMVolume(_bgmSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(_sfxSlider.value);
    }

    public void PauseGame()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}