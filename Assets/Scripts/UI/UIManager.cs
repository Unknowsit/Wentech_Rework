using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool shouldReset = false;

    private int scoreP1 = 0, scoreP2 = 0;

    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerText;

    [Header("Turn Setup")]
    [SerializeField] private Slider turnCountSlider;
    [SerializeField] private TextMeshProUGUI turnCountText;

    [Header("Balloon Setup")]
    [SerializeField] private Slider balloonSpawnSlider;
    [SerializeField] private TextMeshProUGUI balloonSpawnText;

    [Header("Initialization Phase")]
    [SerializeField] private float delayTime;
    [SerializeField] private Button confirmButton;

    [Header("Gameplay Info")]
    [SerializeField] private TextMeshProUGUI targetText;
    public TextMeshProUGUI TargetText => targetText;

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

        StartCoroutine(DelayTimeToStart(delayTime));
    }

    private IEnumerator DelayTimeToStart(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        gameManager.cannonAim.enabled = true;
        gameManager.cannonShooter.enabled = true;
        gameManager.timer.enabled = true;
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
        StartCoroutine(uiController.UITransition(hidePanels: scorePanel));
        gameManager.RestartGame();

        if (shouldReset)
        {
            StartCoroutine(ClearResultTexts());
        }
    }

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
                case OperatorMode.Add:
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

        scoreP1Text.text = this.scoreP1.ToString();
        scoreP2Text.text = this.scoreP2.ToString();

        shouldReset = true;
    }

    private IEnumerator ClearResultTexts()
    {
        shouldReset = false;

        yield return new WaitForSecondsRealtime(1f);

        resultP1Text.text = "0";
        resultP2Text.text = "0";
    }
}