using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeSelector : MonoBehaviour
{
    [Header("Mode Toggle Buttons")]
    [SerializeField] private Toggle addToggle;
    [SerializeField] private Toggle minusToggle;
    [SerializeField] private Toggle multiplyToggle;
    [SerializeField] private Toggle divideToggle;

    [Header("Parenthesis Settings")]
    [SerializeField] private Toggle parenthesisToggle;
    [SerializeField] private GameObject parenthesisPanel;

    [Header("Optional - Display Selected Modes")]
    [SerializeField] private TextMeshProUGUI selectedModesText;

    [Header("Confirmation")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private float delayTime = 0.5f;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;

        LoadCurrentModes();
        LoadParenthesisSetting();

        addToggle.onValueChanged.AddListener(isOn => UpdateMode(OperatorMode.Plus, isOn));
        minusToggle.onValueChanged.AddListener(isOn => UpdateMode(OperatorMode.Minus, isOn));
        multiplyToggle.onValueChanged.AddListener(isOn => UpdateMode(OperatorMode.Multiply, isOn));
        divideToggle.onValueChanged.AddListener(isOn => UpdateMode(OperatorMode.Divide, isOn));

        parenthesisToggle.onValueChanged.AddListener(OnParenthesesToggleChanged);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        UpdateSelectedModesDisplay();
        UpdateParenthesisAvailability();
    }

    private void OnDestroy()
    {
        addToggle.onValueChanged.RemoveAllListeners();
        minusToggle.onValueChanged.RemoveAllListeners();
        multiplyToggle.onValueChanged.RemoveAllListeners();
        divideToggle.onValueChanged.RemoveAllListeners();
    }

    private void LoadCurrentModes()
    {
        addToggle.SetIsOnWithoutNotify(GameData.HasMode(OperatorMode.Plus));
        minusToggle.SetIsOnWithoutNotify(GameData.HasMode(OperatorMode.Minus));
        multiplyToggle.SetIsOnWithoutNotify(GameData.HasMode(OperatorMode.Multiply));
        divideToggle.SetIsOnWithoutNotify(GameData.HasMode(OperatorMode.Divide));
    }

    private void UpdateMode(OperatorMode mode, bool isOn)
    {
        audioManager.PlaySFX("SFX02");

        if (isOn)
        {
            if (!GameData.SelectedModes.Contains(mode))
            {
                GameData.SelectedModes.Add(mode);
            }
        }
        else
        {
            if (GameData.SelectedModes.Count > 1)
            {
                GameData.SelectedModes.Remove(mode);
            }
            else
            {
                // Debug.LogWarning("Cannot remove the last mode! At least one mode must be selected.");

                switch (mode)
                {
                    case OperatorMode.Plus:
                        addToggle.SetIsOnWithoutNotify(true);
                        break;
                    case OperatorMode.Minus:
                        minusToggle.SetIsOnWithoutNotify(true);
                        break;
                    case OperatorMode.Multiply:
                        multiplyToggle.SetIsOnWithoutNotify(true);
                        break;
                    case OperatorMode.Divide:
                        divideToggle.SetIsOnWithoutNotify(true);
                        break;
                }
            }
        }

        UpdateSelectedModesDisplay();
        UpdateParenthesisAvailability();
    }

    private void UpdateSelectedModesDisplay()
    {
        if (selectedModesText == null) return;

        List<string> modeNames = new List<string>();

        foreach (var mode in GameData.SelectedModes)
        {
            switch (mode)
            {
                case OperatorMode.Plus:
                    modeNames.Add("(+)");
                    break;
                case OperatorMode.Minus:
                    modeNames.Add("(-)");
                    break;
                case OperatorMode.Multiply:
                    modeNames.Add("(×)");
                    break;
                case OperatorMode.Divide:
                    modeNames.Add("(÷)");
                    break;
            }
        }

        selectedModesText.text = string.Join(", ", modeNames);
    }

    private void LoadParenthesisSetting()
    {
        parenthesisToggle.SetIsOnWithoutNotify(GameData.UseParentheses);
    }

    public void OnParenthesesToggleChanged(bool isOn)
    {
        GameData.UseParentheses = isOn;
        audioManager.PlaySFX("SFX02");
    }

    private void UpdateParenthesisAvailability()
    {
        bool isMultiMode = GameData.SelectedModes.Count > 1;

        //parenthesisPanel.SetActive(isMultiMode);
        parenthesisToggle.interactable = isMultiMode;

        if (!isMultiMode)
        {
            GameData.UseParentheses = false;
            parenthesisToggle.SetIsOnWithoutNotify(false);
        }
    }

    public void SetSingleMode(OperatorMode mode)
    {
        GameData.SelectedModes.Clear();
        GameData.SelectedModes.Add(mode);
        LoadCurrentModes();
        UpdateSelectedModesDisplay();
    }

    public void OnAddModeSelected()
    {
        SetSingleMode(OperatorMode.Plus);
        audioManager.PlaySFX("SFX02");
    }

    public void OnMinusModeSelected()
    {
        SetSingleMode(OperatorMode.Minus);
        audioManager.PlaySFX("SFX02");
    }

    public void OnMultiplyModeSelected()
    {
        SetSingleMode(OperatorMode.Multiply);
        audioManager.PlaySFX("SFX02");
    }

    public void OnDivideModeSelected()
    {
        SetSingleMode(OperatorMode.Divide);
        audioManager.PlaySFX("SFX02");
    }

    public void OnConfirmButtonClicked()
    {
        audioManager.PlaySFX("SFX04");
        StartCoroutine(TransitionToNextScene(delayTime, "Gameplay"));
    }

    public void OnConfirmToTutorial()
    {
        audioManager.PlaySFX("SFX04");
        StartCoroutine(TransitionToNextScene(delayTime, "TutorialGameplay"));
    }

    private IEnumerator TransitionToNextScene(float delayTime, string sceneName)
    {
        UIController.instance.loadingCover.SetActive(true);
        StartCoroutine(audioManager.FadeOut(audioManager.bgmSource, delayTime));
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneName);
    }
}