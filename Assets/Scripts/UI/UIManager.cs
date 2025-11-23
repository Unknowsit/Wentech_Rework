using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool shouldReset = false;

    private int currentRound = 1;
    private int scoreP1 = 0, scoreP2 = 0;

    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerText;

    [Header("Turn Setup")]
    [SerializeField] private Slider turnCountSlider;
    [SerializeField] private TextMeshProUGUI turnCountText;

    [Header("Total Balloon Setup")]
    [SerializeField] private Slider balloonSpawnSlider;
    [SerializeField] private TextMeshProUGUI balloonSpawnText;

    [Header("Target Amount Setup")]
    [SerializeField] private Slider targetCountSlider;
    [SerializeField] private TextMeshProUGUI targetCountText;

    [Header("Initialization Phase")]
    [SerializeField] private float delayTime;
    [SerializeField] private Button confirmButton;

    [Header("Gameplay Info")]
    [SerializeField] private TextMeshProUGUI targetText;
    public TextMeshProUGUI TargetText => targetText;

    [SerializeField] private TextMeshProUGUI roundText;
    public TextMeshProUGUI RoundText => roundText;

    [Header("Timer Settings")]
    [SerializeField] private Slider _progressBar;
    public Slider _ProgressBar { get { return _progressBar; } set { _progressBar = value; } }

    [SerializeField] private float remainingTime;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    [Header("Single-Mode Panel UI")]
    [SerializeField] public GameObject singleModePanel;

    [SerializeField] private TextMeshProUGUI totalText;
    public TextMeshProUGUI TotalText => totalText;

    [SerializeField] private Button submitButton;
    public Button SubmitButton => submitButton;

    [Header("Multi-Mode Panel UI")]
    [SerializeField] public GameObject multiModePanel;

    [SerializeField] private TextMeshProUGUI multiTotalText;
    public TextMeshProUGUI MultiTotalText => multiTotalText;

    [SerializeField] private Button multiSubmitButton;
    public Button MultiSubmitButton => multiSubmitButton;

    [Header("Score Panel UI")]
    [SerializeField] public GameObject scorePanel;

    [SerializeField] private TextMeshProUGUI resultP1Text;
    public TextMeshProUGUI ResultP1Text => resultP1Text;

    [SerializeField] private TextMeshProUGUI scoreP1Text;
    public TextMeshProUGUI ScoreP1Text => scoreP1Text;

    [SerializeField] private TextMeshProUGUI resultP2Text;
    public TextMeshProUGUI ResultP2Text => resultP2Text;

    [SerializeField] private TextMeshProUGUI scoreP2Text;
    public TextMeshProUGUI ScoreP2Text => scoreP2Text;

    [SerializeField] private TextMeshProUGUI objectiveText;
    public TextMeshProUGUI ObjectiveText => objectiveText;

    [SerializeField] private Button nextButton;
    public Button NextButton => nextButton;

    [Header("Score History UI")]
    [SerializeField] private GameObject scoreHistoryPrefab;
    [SerializeField] private Transform p1HistoryContent;
    [SerializeField] private Transform p2HistoryContent;

    [Header("Result Panel UI")]
    [SerializeField] private GameObject resultUI;

    [SerializeField] private TextMeshProUGUI p1ResultText;
    [SerializeField] private TextMeshProUGUI p1ScoreText;
    [SerializeField] private Image p1Image;

    [SerializeField] private TextMeshProUGUI p2ResultText;
    [SerializeField] private TextMeshProUGUI p2ScoreText;
    [SerializeField] private Image p2Image;

    [SerializeField] private Button exitButton;

    [Header("Sprite Renderer")]
    [SerializeField] private Sprite loseRabbit;
    [SerializeField] private Sprite winRabbit;
    [SerializeField] private Sprite drawRabbit;

    [Header("Android Button")]
    [SerializeField] private GameObject shootButton;
    public GameObject ShootButton => shootButton;

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

#if UNITY_ANDROID
        shootButton.SetActive(true);
#endif

        UpdateTurnText(turnCountSlider.value);
        turnCountSlider.onValueChanged.AddListener(UpdateTurnText);
        UpdateTargetCountText(targetCountSlider.value);
        targetCountSlider.onValueChanged.AddListener(UpdateTargetCountText);
        UpdateBalloonCountText(balloonSpawnSlider.value);
        balloonSpawnSlider.onValueChanged.AddListener(UpdateBalloonCountText);
    }

    private void OnDestroy()
    {
        turnCountSlider.onValueChanged.RemoveListener(UpdateTurnText);
        targetCountSlider.onValueChanged.RemoveListener(UpdateBalloonCountText);
        balloonSpawnSlider.onValueChanged.RemoveListener(UpdateBalloonCountText);
    }

    public void ApplyTurnSetup()
    {
        int targetTurns = Mathf.RoundToInt(turnCountSlider.value);
        int targetBalloons = Mathf.RoundToInt(balloonSpawnSlider.value);
        int targetCounts = Mathf.RoundToInt(targetCountSlider.value);

        audioManager.PlaySFX("SFX04");
        gameManager.SetTargetRounds(targetTurns);
        gameManager.SetTargetBalloonCount(targetCounts);
        gameManager.SetBalloonSpawnCount(targetBalloons);

        StartCoroutine(DelayTimeToStart(delayTime));
    }
  
    private IEnumerator DelayTimeToStart(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        gameManager.cannonAim.enabled = true;
        gameManager.cannonShooter.enabled = true;
        gameManager.timer.enabled = true;
    }

    private IEnumerator DelayTimeToRestart()
    {
        yield return new WaitForSecondsRealtime(delayTime);
        gameManager.cannonShooter.enabled = true;
        gameManager.timer.isCounting = true;
    }

    private void UpdateTurnText(float value)
    {
        turnCountText.text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateBalloonCountText(float value)
    {
        balloonSpawnText.text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateTargetCountText(float value)
    {
        targetCountText.text = Mathf.RoundToInt(value).ToString();
    }

    public void ShowCalculationPanel()
    {
        if (GameData.IsSingleMode())
        {
            singleModePanel.SetActive(true);
        }
        else
        {
            multiModePanel.SetActive(true);
        }
    }

    public void HideCalculationPanel()
    {
        if (GameData.IsSingleMode())
        {
            singleModePanel.SetActive(false);
        }
        else
        {
            multiModePanel.SetActive(false);
        }
    }

    public void OnSubmitButtonClicked()
    {
        audioManager.PlaySFX("SFX04");
        if (GameData.IsSingleMode())
        {
            StartCoroutine(uiController.UITransition(scorePanel, singleModePanel));
            gameManager.SetPlayerValues(totalText, playerText);
        }
        else
        {
            StartCoroutine(uiController.UITransition(scorePanel, multiModePanel));
            gameManager.SetPlayerValues(multiTotalText, playerText);
        }

        NextButton.interactable = true;
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
        audioManager.PlaySFX("SFX04");
        if (gameManager.totalTurns == 1)
        {
            UpdateWinnerPanel();
            StartCoroutine(uiController.UITransition(showPanel: resultUI, hidePanels: scorePanel));
        }
        else
        {
            StartCoroutine(uiController.UITransition(hidePanels: scorePanel));
            gameManager.SwitchCannonSide();
            gameManager.RestartGame();
            StartCoroutine(DelayTimeToRestart());

#if UNITY_ANDROID
            StartCoroutine(ShootButtonActive());
#endif

            if (shouldReset)
            {
                StartCoroutine(ClearResultTexts());
            }
        }
    }

    public void UpdateWinnerPanel()
    {
        int finalScoreP1 = scoreP1;
        int finalScoreP2 = scoreP2;

        //p1ScoreText.text = finalScoreP1.ToString();
        //p2ScoreText.text = finalScoreP2.ToString();

        p1ScoreText.text = NumberFormatter.FormatWithCommas(finalScoreP1);
        p2ScoreText.text = NumberFormatter.FormatWithCommas(finalScoreP2);

        audioManager.PlayAmbient("ABS02");

        if (finalScoreP1 > finalScoreP2)
        {
            p1ResultText.text = "WINNER";
            p2ResultText.text = "LOSE";
            p1Image.sprite = winRabbit;
            p2Image.sprite = loseRabbit;
        }
        else if (finalScoreP2 > finalScoreP1)
        {
            p1ResultText.text = "LOSE";
            p2ResultText.text = "WINNER";
            p1Image.sprite = loseRabbit;
            p2Image.sprite = winRabbit;
        }
        else
        {
            p1ResultText.text = "DRAW";
            p2ResultText.text = "DRAW";
            p1Image.sprite = drawRabbit;
            p2Image.sprite = drawRabbit;
        }
    }

    public void OnExitButtonClicked()
    {
        exitButton.interactable = false;
        audioManager.PlaySFX("SFX04");
        StartCoroutine(uiController.UITransition(hidePanels: scorePanel));
        SceneManager.LoadSceneAsync("Operators");
    }

#if UNITY_ANDROID
    private IEnumerator ShootButtonActive()
    {
        yield return new WaitForSecondsRealtime(1f);
        shootButton.SetActive(true);
    }
#endif

    public void CountScore(float p1, float p2, bool p1Answered, bool p2Answered)
    {
        //Debug.Log($"[CountScore] Received - P1:{p1} ({p1Answered}), P2:{p2} ({p2Answered})");
        float current = float.Parse(targetText.text);
        int scoreP1 = 0, scoreP2 = 0;

        float diffP1 = p1Answered ? Mathf.Abs(p1 - current) : float.MaxValue;
        float diffP2 = p2Answered ? Mathf.Abs(p2 - current) : float.MaxValue;

        //Debug.Log($"[CountScore] Target:{current}, DiffP1:{diffP1}, DiffP2:{diffP2}");

        if (GameData.IsSingleMode())
        {
            var mode = GameData.GetSingleMode();

            switch (mode)
            {
                case OperatorMode.Plus:
                case OperatorMode.Multiply:
                    {
                        if (current > Mathf.Epsilon)
                        {
                            if (p1Answered) scoreP1 = Mathf.RoundToInt((1f - diffP1 / current) * 1000f);
                            if (p2Answered) scoreP2 = Mathf.RoundToInt((1f - diffP2 / current) * 1000f);
                        }
                        break;
                    }
                case OperatorMode.Minus:
                case OperatorMode.Divide:
                    {
                        float absCurrent = Mathf.Abs(current);

                        if (absCurrent > Mathf.Epsilon)
                        {
                            if (p1Answered) scoreP1 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP1) * 1000f);
                            if (p2Answered) scoreP2 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP2) * 1000f);
                        }
                        else
                        {
                            Debug.LogWarning("Target is zero! Can't calculate score.");
                        }
                        break;
                    }
            }
        }
        else
        {
            //Debug.LogWarning("Multiple modes selected - using safe scoring method");

            float absCurrent = Mathf.Abs(current);

            if (absCurrent > Mathf.Epsilon)
            {
                if (p1Answered) scoreP1 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP1) * 1000f);
                if (p2Answered) scoreP2 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP2) * 1000f);
            }
            else
            {
                Debug.LogWarning("Target is zero! Can't calculate score.");
            }
        }

        //Debug.Log($"[CountScore] Before Clamp - ScoreP1:{scoreP1}, ScoreP2:{scoreP2}");

        scoreP1 = Mathf.Clamp(scoreP1, 0, 1000);
        scoreP2 = Mathf.Clamp(scoreP2, 0, 1000);

        //Debug.Log($"[CountScore] After Clamp - ScoreP1:{scoreP1}, ScoreP2:{scoreP2}");

        this.scoreP1 += scoreP1;
        this.scoreP2 += scoreP2;

        //Debug.Log($"[CountScore] Total Score - P1:{this.scoreP1}, P2:{this.scoreP2}");

        //scoreP1Text.text = this.scoreP1.ToString();
        //scoreP2Text.text = this.scoreP2.ToString();

        scoreP1Text.text = NumberFormatter.FormatWithCommas(this.scoreP1);
        scoreP2Text.text = NumberFormatter.FormatWithCommas(this.scoreP2);

        AddScoreHistory(currentRound, p1, scoreP1, p2, scoreP2, current);
        currentRound++;

        shouldReset = true;
    }

    private void AddScoreHistory(int round, float p1Answer, int p1Score, float p2Answer, int p2Score, float target)
    {
        // Player 1 History
        GameObject p1History = Instantiate(scoreHistoryPrefab, p1HistoryContent);
        TextMeshProUGUI p1Text = p1History.GetComponent<TextMeshProUGUI>();

        //p1Text.text = $"Round {round}\nScore : +{p1Score}\nTarget : {target}\nAnswer : {p1Answer}";
        p1Text.text = $"Round {round}\nScore : +{NumberFormatter.FormatWithCommas(p1Score)}\nTarget : {NumberFormatter.FormatSmart(target)}\nAnswer : {NumberFormatter.FormatSmart(p1Answer)}";

        // Player 2 History
        GameObject p2History = Instantiate(scoreHistoryPrefab, p2HistoryContent);
        TextMeshProUGUI p2Text = p2History.GetComponent<TextMeshProUGUI>();

        //p2Text.text = $"Round {round}\nScore : +{p2Score}\nTarget : {target}\nAnswer : {p2Answer}";
        p2Text.text = $"Round {round}\nScore : +{NumberFormatter.FormatWithCommas(p2Score)}\nTarget : {NumberFormatter.FormatSmart(target)}\nAnswer : {NumberFormatter.FormatSmart(p2Answer)}";
    }

    private IEnumerator ClearResultTexts()
    {
        shouldReset = false;

        yield return new WaitForSecondsRealtime(1f);

        resultP1Text.text = "0";
        resultP2Text.text = "0";
    }
}