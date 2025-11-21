using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private bool isGamePaused = false;

    public Slider _bgmSlider, _sfxSlider;

    [Header("Scene Transition")]
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

    private void Start()
    {
        LoadSliderValues();
    }

    private void LoadSliderValues()
    {
        _bgmSlider.value = audioManager.bgmSource.volume;
        Debug.Log($"BGM Slider loaded: {_bgmSlider.value}");

        _sfxSlider.value = audioManager.sfxSource.volume;
        Debug.Log($"SFX Slider loaded: {_sfxSlider.value}");
    }

    public void ToggleBGM()
    {
        audioManager.ToggleBGM();
        SaveSettings();
    }

    public void ToggleSFX()
    {
        audioManager.ToggleSFX();
        SaveSettings();
    }

    public void BGMVolume()
    {
        audioManager.BGMVolume(_bgmSlider.value);
        SaveSettings();
    }

    public void SFXVolume()
    {
        audioManager.SFXVolume(_sfxSlider.value);
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", audioManager.savedBGMVolume);
        PlayerPrefs.SetFloat("SFXVolume", audioManager.savedSFXVolume);
        PlayerPrefs.SetInt("BGMMute", audioManager.bgmSource.mute ? 1 : 0);
        PlayerPrefs.SetInt("SFXMute", audioManager.sfxSource.mute ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"Settings saved! BGM: {audioManager.savedBGMVolume}, SFX: {audioManager.savedSFXVolume}");
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