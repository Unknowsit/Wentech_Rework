using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalloonManager : MonoBehaviour
{
    [Header("Special Balloon Toggles")]
    [SerializeField] private Toggle goldenBalloonToggle;
    [SerializeField] private Toggle mysteryBalloonToggle;
    [SerializeField] private Toggle comboBalloonToggle;
    [SerializeField] private Toggle luckyBalloonToggle;
    [SerializeField] private Toggle jokerBalloonToggle;

    [Header("Special Balloon Count Sliders")]
    [SerializeField] private Slider goldenCountSlider;
    [SerializeField] private TextMeshProUGUI goldenCountText;

    [SerializeField] private Slider mysteryCountSlider;
    [SerializeField] private TextMeshProUGUI mysteryCountText;

    [SerializeField] private Slider comboCountSlider;
    [SerializeField] private TextMeshProUGUI comboCountText;

    [SerializeField] private Slider luckyCountSlider;
    [SerializeField] private TextMeshProUGUI luckyCountText;

    [SerializeField] private Slider jokerCountSlider;
    [SerializeField] private TextMeshProUGUI jokerCountText;

    private void Start()
    {
        LoadSettings();
        SetupListeners();
        UpdateTotalSpecialBalloons();
    }

    private void SetupListeners()
    {
        goldenBalloonToggle.onValueChanged.AddListener(OnGoldenToggleChanged);
        mysteryBalloonToggle.onValueChanged.AddListener(OnMysteryToggleChanged);
        comboBalloonToggle.onValueChanged.AddListener(OnComboToggleChanged);
        luckyBalloonToggle.onValueChanged.AddListener(OnLuckyToggleChanged);
        jokerBalloonToggle.onValueChanged.AddListener(OnJokerToggleChanged);

        goldenCountSlider.onValueChanged.AddListener(OnGoldenCountChanged);
        mysteryCountSlider.onValueChanged.AddListener(OnMysteryCountChanged);
        comboCountSlider.onValueChanged.AddListener(OnComboCountChanged);
        luckyCountSlider.onValueChanged.AddListener(OnLuckyCountChanged);
        jokerCountSlider.onValueChanged.AddListener(OnJokerCountChanged);
    }

    private void LoadSettings()
    {
        goldenBalloonToggle.isOn = GameData.EnableGoldenBalloon;
        mysteryBalloonToggle.isOn = GameData.EnableMysteryBalloon;
        comboBalloonToggle.isOn = GameData.EnableComboBalloon;
        luckyBalloonToggle.isOn = GameData.EnableLuckyBalloon;
        jokerBalloonToggle.isOn = GameData.EnableJokerBalloon;

        goldenCountSlider.value = GameData.GoldenBalloonCount;
        mysteryCountSlider.value = GameData.MysteryBalloonCount;
        jokerCountSlider.value = GameData.JokerBalloonCount;
        luckyCountSlider.value = GameData.LuckyBalloonCount;
        comboCountSlider.value = GameData.ComboBalloonCount;

        goldenCountSlider.interactable = GameData.EnableGoldenBalloon;
        mysteryCountSlider.interactable = GameData.EnableMysteryBalloon;
        comboCountSlider.interactable = GameData.EnableComboBalloon;
        luckyCountSlider.interactable = GameData.EnableLuckyBalloon;
        jokerCountSlider.interactable = GameData.EnableJokerBalloon;

        UpdateCountText(mysteryCountText, GameData.MysteryBalloonCount);
        UpdateCountText(goldenCountText, GameData.GoldenBalloonCount);
        UpdateCountText(comboCountText, GameData.ComboBalloonCount);
        UpdateCountText(luckyCountText, GameData.LuckyBalloonCount);
        UpdateCountText(jokerCountText, GameData.JokerBalloonCount);
    }

    private void OnGoldenToggleChanged(bool value)
    {
        GameData.EnableGoldenBalloon = value;
        goldenCountSlider.interactable = value;
        PlayerPrefs.SetInt("EnableGolden", value ? 1 : 0);
        PlayerPrefs.SetInt("GoldenCount", GameData.GoldenBalloonCount);
        PlayerPrefs.Save();

        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnMysteryToggleChanged(bool value)
    {
        GameData.EnableMysteryBalloon = value;
        mysteryCountSlider.interactable = value;
        PlayerPrefs.SetInt("EnableMystery", value ? 1 : 0);
        PlayerPrefs.SetInt("MysteryCount", GameData.MysteryBalloonCount);
        PlayerPrefs.Save();

        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnComboToggleChanged(bool value)
    {
        GameData.EnableComboBalloon = value;
        comboCountSlider.interactable = value;
        PlayerPrefs.SetInt("EnableCombo", value ? 1 : 0);
        PlayerPrefs.SetInt("ComboCount", GameData.ComboBalloonCount);
        PlayerPrefs.Save();

        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnLuckyToggleChanged(bool value)
    {
        GameData.EnableLuckyBalloon = value;
        luckyCountSlider.interactable = value;
        PlayerPrefs.SetInt("EnableLucky", value ? 1 : 0);
        PlayerPrefs.SetInt("LuckyCount", GameData.LuckyBalloonCount);
        PlayerPrefs.Save();

        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnJokerToggleChanged(bool value)
    {
        GameData.EnableJokerBalloon = value;
        jokerCountSlider.interactable = value;
        PlayerPrefs.SetInt("EnableJoker", value ? 1 : 0);
        PlayerPrefs.SetInt("JokerCount", GameData.JokerBalloonCount);
        PlayerPrefs.Save();

        AudioManager.instance.PlaySFX("SFX02");
        UpdateTotalSpecialBalloons();
    }

    private void OnMysteryCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.MysteryBalloonCount = count;
        PlayerPrefs.SetInt("MysteryCount", count);
        PlayerPrefs.Save();
        UpdateCountText(mysteryCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnGoldenCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.GoldenBalloonCount = count;
        PlayerPrefs.SetInt("GoldenCount", count);
        PlayerPrefs.Save();
        UpdateCountText(goldenCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnComboCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.ComboBalloonCount = count;
        PlayerPrefs.SetInt("ComboCount", count);
        PlayerPrefs.Save();
        UpdateCountText(comboCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnLuckyCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.LuckyBalloonCount = count;
        PlayerPrefs.SetInt("LuckyCount", count);
        PlayerPrefs.Save();
        UpdateCountText(luckyCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void OnJokerCountChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        GameData.JokerBalloonCount = count;
        PlayerPrefs.SetInt("JokerCount", count);
        PlayerPrefs.Save();
        UpdateCountText(jokerCountText, count);
        UpdateTotalSpecialBalloons();
    }

    private void UpdateCountText(TextMeshProUGUI textUI, int count)
    {
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
    }
}