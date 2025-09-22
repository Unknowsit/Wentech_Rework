using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum OperatorMode
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public static class GameData
{
    public static OperatorMode SelectedMode = OperatorMode.Add;
}

public class UIController : MonoBehaviour
{
    private bool isConfirming = false;
    private bool isGamePaused = false;

    public Slider _bgmSlider, _sfxSlider;

    public void OnAddModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Add;
    }

    public void OnSubtractModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Subtract;
    }

    public void OnMultiplyModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Multiply;
    }

    public void OnDivideModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Divide;
    }

    public void CancelConfirming()
    {
        isConfirming = false;
    }

    public void OnConfirmButtonClicked()
    {
        if (isConfirming)
        {
            SceneManager.LoadScene("Gameplay");
        }
    }

    public void ConfirmTargetCount()
    {
        if (int.TryParse(UIManager.instance.targetInputField.text, out int target))
        {
            GameManager.instance.SetTargetBalloonCount(target);
        }
    }

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