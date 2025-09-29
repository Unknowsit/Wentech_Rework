using System.Collections;
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

    [Header ("Scene Transition")]
    [SerializeField] private float delayTime = 3f;
    [SerializeField] private GameObject loadingCover;
    [SerializeField] private Image blackoutScreen;

    private AudioManager audioManager;
    public static UIController instance;

    private void Awake()
    {
        audioManager = AudioManager.instance;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
            StartCoroutine(TransitionToNextScene(delayTime));
        }
    }

    public void SetTurnCount()
    {
        if (int.TryParse(UIManager.instance.targetInputField.text, out int target))
        {
            GameManager.instance.SetTargetRounds(target);
            Time.timeScale = 1f;
        }
    }

    /*
    public void ConfirmTargetCount()
    {
        if (int.TryParse(UIManager.instance.targetInputField.text, out int target))
        {
            GameManager.instance.SetTargetBalloonCount(target);
        }
    }
    */

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

    private IEnumerator TransitionToNextScene(float delayTime)
    {
        loadingCover.SetActive(true);
        StartCoroutine(audioManager.FadeOut(audioManager.bgmSource, delayTime));
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene("Gameplay");
    }

    private IEnumerator FadeInUI(float duration)
    {
        Debug.Log("Start fade in UI.");
        for (float i = 0f; i <= 1f; i += Time.deltaTime / duration)
        {
            Debug.Log("Processing...");
            blackoutScreen.color = new Color(0f, 0f, 0f, i);
            yield return null;
        }
    }

    private IEnumerator FadeOutUI(float duration)
    {
        Debug.Log("Start fade out UI.");
        for (float i = 1f; i >= 0f; i -= Time.deltaTime / duration)
        {
            Debug.Log("Processing...");
            blackoutScreen.color = new Color(0f, 0f, 0f, i);
            yield return null;
        }
    }

    public void RunTransition()
    {
        StartCoroutine(TransitionRoutine());
    }

    public IEnumerator TransitionRoutine()
    {
        yield return StartCoroutine(FadeInUI(delayTime));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeOutUI(delayTime));
    }
}