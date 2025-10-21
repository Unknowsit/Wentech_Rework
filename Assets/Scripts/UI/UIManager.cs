using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum OperatorMode
{
    Add,
    Minus,
    Multiply,
    Divide
}

public static class GameData
{
    public static OperatorMode SelectedMode = OperatorMode.Add;
}

public class UIManager : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerText;
    public TextMeshProUGUI PlayerText => playerText;

    [Header("Turn Setup")]
    [SerializeField] private Slider turnCountSlider;
    [SerializeField] private TextMeshProUGUI turnCountText;

    [Header("Balloon Setup")]
    [SerializeField] private Slider balloonSpawnSlider;
    [SerializeField] private TextMeshProUGUI balloonSpawnText;

    [Header("Confirm Button")]
    [SerializeField] private Button confirmButton;

    [Header("Gameplay Info")]
    [SerializeField] private TextMeshProUGUI targetText;
    public TextMeshProUGUI TargetText => targetText;

    [Header("Timer Settings")]
    [SerializeField] private Slider _progressBar;
    public Slider _ProgressBar { get { return _progressBar; } set { _progressBar = value; } }

    [SerializeField] private float remainingTime;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    [Header("Gameplay Panel UI")]
    [SerializeField] public GameObject calculationPanel;

    [SerializeField] private TextMeshProUGUI totalText;
    public TextMeshProUGUI TotalText => totalText;

    [SerializeField] public GameObject scorePanel;

    [SerializeField] private TextMeshProUGUI player01;
    public TextMeshProUGUI Player01 { get { return player01; } set { player01 = value; } }

    [SerializeField] private TextMeshProUGUI player02;
    public TextMeshProUGUI Player02 => player02;

    [Header("Useless")]
    public TMP_InputField targetInputField;

    private AudioManager audioManager;
    private GameManager gameManager;
    private UIController uiController;

    public static UIManager instance;

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
    }

    private void Start()
    {
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;
        uiController = UIController.instance;

        UpdateTurnText(turnCountSlider.value);
        turnCountSlider.onValueChanged.AddListener(UpdateTurnText);
        UpdateBalloonCountText(balloonSpawnSlider.value);
        balloonSpawnSlider.onValueChanged.AddListener(UpdateBalloonCountText);
    }

    private void OnDestroy()
    {
        turnCountSlider.onValueChanged.RemoveListener(UpdateTurnText);
        balloonSpawnSlider.onValueChanged.RemoveListener(UpdateBalloonCountText);
    }

    public void ApplyTurnSetup()
    {
        int targetTurns = Mathf.RoundToInt(turnCountSlider.value);
        int targetBalloons = Mathf.RoundToInt(balloonSpawnSlider.value);

        audioManager.PlaySFX("SFX04");
        gameManager.SetTargetRounds(targetTurns);
        gameManager.SetBalloonSpawnCount(targetBalloons);
        Time.timeScale = 1f;
    }

    private void UpdateTurnText(float value)
    {
        turnCountText.text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateBalloonCountText(float value)
    {
        balloonSpawnText.text = Mathf.RoundToInt(value).ToString();
    }

    public void ShowCalculationPanel()
    {
        calculationPanel.SetActive(true);
    }

    public void HideCalculationPanel()
    {
        calculationPanel.SetActive(false);
    }

    public void OnSubmitButtonClicked()
    {
        StartCoroutine(uiController.UITransition(scorePanel, calculationPanel));
        gameManager.SetPlayerValues();
    }

    public void ShowScorePanel()
    {
        scorePanel.SetActive(true);
    }

    public void HideScorePanel()
    {
        scorePanel.SetActive(false);
    }

    public void OnTapButtonClicked()
    {
        StartCoroutine(uiController.UITransition(hidePanels: scorePanel));
        gameManager.RestartGame();
    }
}