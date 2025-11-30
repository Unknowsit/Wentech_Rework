using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalloonManager : MonoBehaviour
{
    [Header("Special Balloon Toggles")]
    [SerializeField] private Toggle mysteryBalloonToggle;
    [SerializeField] private Toggle goldenBalloonToggle;
    [SerializeField] private Toggle comboBalloonToggle;
    [SerializeField] private Toggle luckyBalloonToggle;
    [SerializeField] private Toggle jokerBalloonToggle;

    [Header("Special Balloon Count Sliders")]
    [SerializeField] private Slider mysteryCountSlider;
    [SerializeField] private TextMeshProUGUI mysteryCountText;

    [SerializeField] private Slider goldenCountSlider;
    [SerializeField] private TextMeshProUGUI goldenCountText;

    [SerializeField] private Slider comboCountSlider;
    [SerializeField] private TextMeshProUGUI comboCountText;

    [SerializeField] private Slider luckyCountSlider;
    [SerializeField] private TextMeshProUGUI luckyCountText;

    [SerializeField] private Slider jokerCountSlider;
    [SerializeField] private TextMeshProUGUI jokerCountText;

    [Header("Info Text")]
    [SerializeField] private TextMeshProUGUI totalSpecialBalloonsText;

    private void Start()
    {
        LoadSettings();
        SetupListeners();
        UpdateTotalSpecialBalloons();
    }

    private void SetupListeners()
    {
        mysteryBalloonToggle.onValueChanged.AddListener(OnMysteryToggleChanged);
        goldenBalloonToggle.onValueChanged.AddListener(OnGoldenToggleChanged);
        comboBalloonToggle.onValueChanged.AddListener(OnComboToggleChanged);
        luckyBalloonToggle.onValueChanged.AddListener(OnLuckyToggleChanged);
        jokerBalloonToggle.onValueChanged.AddListener(OnJokerToggleChanged);

        mysteryCountSlider.onValueChanged.AddListener(OnMysteryCountChanged);
        goldenCountSlider.onValueChanged.AddListener(OnGoldenCountChanged);
        comboCountSlider.onValueChanged.AddListener(OnComboCountChanged);
        luckyCountSlider.onValueChanged.AddListener(OnLuckyCountChanged);
        jokerCountSlider.onValueChanged.AddListener(OnJokerCountChanged);
    }

    private void LoadSettings()
    {
        mysteryBalloonToggle.isOn = GameData.EnableMysteryBalloon;
        goldenBalloonToggle.isOn = GameData.EnableGoldenBalloon;
        comboBalloonToggle.isOn = GameData.EnableComboBalloon;
        luckyBalloonToggle.isOn = GameData.EnableLuckyBalloon;
        jokerBalloonToggle.isOn = GameData.EnableJokerBalloon;

        mysteryCountSlider.value = GameData.MysteryBalloonCount;
        goldenCountSlider.value = GameData.GoldenBalloonCount;
        jokerCountSlider.value = GameData.JokerBalloonCount;
        luckyCountSlider.value = GameData.LuckyBalloonCount;
        comboCountSlider.value = GameData.ComboBalloonCount;

        UpdateCountText(mysteryCountText, GameData.MysteryBalloonCount);
        UpdateCountText(goldenCountText, GameData.GoldenBalloonCount);
        UpdateCountText(comboCountText, GameData.ComboBalloonCount);
        UpdateCountText(luckyCountText, GameData.LuckyBalloonCount);
        UpdateCountText(jokerCountText, GameData.JokerBalloonCount);
    }

    private void OnMysteryToggleChanged(bool value)
    {
        GameData.EnableMysteryBalloon = value;
        PlayerPrefs.SetInt("EnableMystery", value ? 1 : 0);
        mysteryCountSlider.interactable = value;
        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnGoldenToggleChanged(bool value)
    {
        GameData.EnableGoldenBalloon = value;
        PlayerPrefs.SetInt("EnableGolden", value ? 1 : 0);
        goldenCountSlider.interactable = value;
        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnComboToggleChanged(bool value)
    {
        GameData.EnableComboBalloon = value;
        PlayerPrefs.SetInt("EnableCombo", value ? 1 : 0);
        comboCountSlider.interactable = value;
        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnLuckyToggleChanged(bool value)
    {
        GameData.EnableLuckyBalloon = value;
        PlayerPrefs.SetInt("EnableLucky", value ? 1 : 0);
        luckyCountSlider.interactable = value;
        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnJokerToggleChanged(bool value)
    {
        GameData.EnableJokerBalloon = value;
        PlayerPrefs.SetInt("EnableWild", value ? 1 : 0);
        jokerCountSlider.interactable = value;
        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnMysteryCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.MysteryBalloonCount = count;
        PlayerPrefs.SetInt("MysteryCount", count);
        UpdateCountText(mysteryCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnGoldenCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.GoldenBalloonCount = count;
        PlayerPrefs.SetInt("GoldenCount", count);
        UpdateCountText(goldenCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnComboCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.ComboBalloonCount = count;
        PlayerPrefs.SetInt("ComboCount", count);
        UpdateCountText(comboCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnLuckyCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.LuckyBalloonCount = count;
        PlayerPrefs.SetInt("LuckyCount", count);
        UpdateCountText(luckyCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnJokerCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.JokerBalloonCount = count;
        PlayerPrefs.SetInt("WildCount", count);
        UpdateCountText(jokerCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void UpdateCountText(TextMeshProUGUI textUI, int count)
    {
        if (count == 0)
            textUI.text = "Random";
        else
            textUI.text = count.ToString();
    }

    private void UpdateTotalSpecialBalloons()
    {
        int total = 0;

        if (GameData.EnableMysteryBalloon && GameData.MysteryBalloonCount > 0)
            total += GameData.MysteryBalloonCount;

        if (GameData.EnableGoldenBalloon && GameData.GoldenBalloonCount > 0)
            total += GameData.GoldenBalloonCount;

        if (GameData.EnableLuckyBalloon && GameData.LuckyBalloonCount > 0)
            total += GameData.LuckyBalloonCount;

        if (GameData.EnableComboBalloon && GameData.ComboBalloonCount > 0)
            total += GameData.ComboBalloonCount;

        if (GameData.EnableJokerBalloon && GameData.JokerBalloonCount > 0)
            total += GameData.JokerBalloonCount;

        if (total == 0)
            totalSpecialBalloonsText.text = "Random";
        else
            totalSpecialBalloonsText.text = $"{total}";
    }
}