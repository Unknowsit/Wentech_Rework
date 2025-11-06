using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private bool isConfirming = false;
    private bool isGamePaused = false;

    public Slider _bgmSlider, _sfxSlider;

    [Header ("Scene Transition")]
    [SerializeField] private float delayTime = 3f;
    [SerializeField] public GameObject loadingCover;
    [SerializeField] private Image blackoutScreen;

    private AudioManager audioManager;
    private UIManager uiManager;

    public static UIController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioManager = AudioManager.instance;
        uiManager = UIManager.instance;
    }

    public void OnAddModeSelected()
    {
        isConfirming = true;
        //GameData.SelectedMode = OperatorMode.Add;
        audioManager.PlaySFX("SFX02");
    }

    public void OnMinusModeSelected()
    {
        isConfirming = true;
        //GameData.SelectedMode = OperatorMode.Minus;
        audioManager.PlaySFX("SFX02");
    }

    public void OnMultiplyModeSelected()
    {
        isConfirming = true;
        //GameData.SelectedMode = OperatorMode.Multiply;
        audioManager.PlaySFX("SFX02");
    }

    public void OnDivideModeSelected()
    {
        isConfirming = true;
        //GameData.SelectedMode = OperatorMode.Divide;
        audioManager.PlaySFX("SFX02");
    }

    public void CancelConfirming()
    {
        isConfirming = false;
        audioManager.PlaySFX("SFX03");
    }

    public void OnConfirmButtonClicked()
    {
        if (isConfirming)
        {
            audioManager.PlaySFX("SFX04");
            StartCoroutine(TransitionToNextScene(delayTime));           
        }
    }

    /*
    public void SetTurnCount()
    {
        if (int.TryParse(uiManager.targetInputField.text, out int target))
        {
            audioManager.PlaySFX("SFX04");
            GameManager.instance.SetTargetRounds(target);
            Time.timeScale = 1f;
        }
    }
    */

    /*
    public void ConfirmTargetCount()
    {
        if (int.TryParse(uiManager.targetInputField.text, out int target))
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

    public IEnumerator FadeInUI(float duration)
    {
        Debug.Log("Start fade in UI.");
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            blackoutScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        blackoutScreen.color = new Color(0f, 0f, 0f, 1f);
    }

    public IEnumerator FadeOutUI(float duration)
    {
        Debug.Log("Start fade out UI.");
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / duration));
            blackoutScreen.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        blackoutScreen.color = new Color(0f, 0f, 0f, 0f);
    }

    /*
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
    */

    public void RunTransition()
    {
        if (GameData.IsSingleMode())
        {
            StartCoroutine(UITransition(showPanel: uiManager.singleModePanel));
        }
        else
        {
            StartCoroutine(UITransition(showPanel: uiManager.multiModePanel));
        }

        StartCoroutine(GameManager.instance.DestroyBalloonsAfterDelay(1f));
    }

    public IEnumerator UITransition(GameObject showPanel = null, params GameObject[] hidePanels)
    {
        yield return StartCoroutine(FadeInUI(delayTime));

        foreach (var panel in hidePanels)
        {
            panel.SetActive(false);
        }

        if (showPanel != null)
        {
            showPanel.SetActive(true);
        }

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(FadeOutUI(delayTime));
    }

    /*
    public IEnumerator UITransition(bool flag)
    {
        yield return StartCoroutine(FadeInUI(delayTime));

        uiManager.HideCalculationPanel();
        uiManager.HideScorePanel();

        if (flag)
        {
            uiManager.ShowCalculationPanel();
        }
        else
        {
            uiManager.ShowScorePanel();
            GameManager.instance.RestartGame();
        }

        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(FadeOutUI(delayTime));
    }
    */
}