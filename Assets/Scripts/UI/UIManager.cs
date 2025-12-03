using System.Collections;
using System.Collections.Generic;
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

    [Header("Hint System")]
    private bool p1HintUsedThisTurn = false;
    private bool p2HintUsedThisTurn = false;
    private string p1CurrentHint = "";
    private string p2CurrentHint = "";
    private int p1HintsRemaining = 0;
    private int p2HintsRemaining = 0;

    [Header("Hint Setup")]
    [SerializeField] private GameObject hintPanel;
    public GameObject HintPanel => hintPanel;

    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintCountText;

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

    [Header("Balloon Game Data")]
    public List<BalloonType> p1BalloonTypes = new List<BalloonType>();
    public List<BalloonType> p2BalloonTypes = new List<BalloonType>();

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

        InitializeHints(targetTurns);

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
        StoreBalloonTypesForCurrentPlayer();

        if (gameManager.totalTurns % 2 == 0)
        {
            gameManager.SaveBalloons();
        }

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
        hintPanel.SetActive(false);

        if (gameManager.totalTurns % 2 == 0)
        {
            p1HintUsedThisTurn = false;
            p1CurrentHint = "";
        }
        else
        {
            p2HintUsedThisTurn = false;
            p2CurrentHint = "";
        }

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
            UpdateHintButtonUI();

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
        p1BalloonTypes.Clear();
        p2BalloonTypes.Clear();
        audioManager.ambSource.Stop();
        audioManager.PlaySFX("SFX04");
        StartCoroutine(uiController.UITransition(hidePanels: scorePanel));
        StartCoroutine(LoadMainMenuAfterDelay());
    }

    private IEnumerator LoadMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("Mainmenu");
    }

#if UNITY_ANDROID
    private IEnumerator ShootButtonActive()
    {
        yield return new WaitForSecondsRealtime(1f);
        shootButton.SetActive(true);
    }
#endif

    /*
    public void CountScore(float p1, float p2, bool p1Answered, bool p2Answered)
    {
        //Debug.Log($"[CountScore] Received - P1:{p1} ({p1Answered}), P2:{p2} ({p2Answered})");
        float current = float.Parse(targetText.text);
        int baseScoreP1 = 0, baseScoreP2 = 0;
        int bonusScoreP1 = 0, bonusScoreP2 = 0;

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
                            if (p1Answered) baseScoreP1 = Mathf.RoundToInt((1f - diffP1 / current) * 1000f);
                            if (p2Answered) baseScoreP2 = Mathf.RoundToInt((1f - diffP2 / current) * 1000f);
                        }
                        break;
                    }
                case OperatorMode.Minus:
                case OperatorMode.Divide:
                    {
                        float absCurrent = Mathf.Abs(current);

                        if (absCurrent > Mathf.Epsilon)
                        {
                            if (p1Answered) baseScoreP1 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP1) * 1000f);
                            if (p2Answered) baseScoreP2 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP2) * 1000f);
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
                if (p1Answered) baseScoreP1 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP1) * 1000f);
                if (p2Answered) baseScoreP2 = Mathf.RoundToInt(absCurrent / (absCurrent + diffP2) * 1000f);
            }
            else
            {
                Debug.LogWarning("Target is zero! Can't calculate score.");
            }
        }

        //Debug.Log($"[CountScore] Before Clamp - ScoreP1:{scoreP1}, ScoreP2:{scoreP2}");
        baseScoreP1 = Mathf.Clamp(baseScoreP1, 0, 1000);
        baseScoreP2 = Mathf.Clamp(baseScoreP2, 0, 1000);

        if (p1Answered)
        {
            bonusScoreP1 = CalculateSpecialBalloonBonus(p1BalloonTypes, baseScoreP1, diffP1);
        }

        if (p2Answered)
        {
            bonusScoreP2 = CalculateSpecialBalloonBonus(p2BalloonTypes, baseScoreP2, diffP2);
        }

        //Debug.Log($"[CountScore] After Clamp - ScoreP1:{scoreP1}, ScoreP2:{scoreP2}");

        int totalScoreP1 = baseScoreP1 + bonusScoreP1;
        int totalScoreP2 = baseScoreP2 + bonusScoreP2;

        this.scoreP1 += totalScoreP1;
        this.scoreP2 += totalScoreP2;

        //Debug.Log($"[CountScore] Total Score - P1:{this.scoreP1}, P2:{this.scoreP2}");

        //scoreP1Text.text = this.scoreP1.ToString();
        //scoreP2Text.text = this.scoreP2.ToString();

        scoreP1Text.text = NumberFormatter.FormatWithCommas(this.scoreP1);
        scoreP2Text.text = NumberFormatter.FormatWithCommas(this.scoreP2);

        AddScoreHistory(currentRound, p1, baseScoreP1, bonusScoreP1, p2, baseScoreP2, bonusScoreP2, current);
        currentRound++;

        shouldReset = true;
    }
    */

    public void InitializeHints(int totalRounds)
    {
        int hintsPerPlayer = Mathf.FloorToInt(totalRounds / 2f);
        p1HintsRemaining = hintsPerPlayer;
        p2HintsRemaining = hintsPerPlayer;

        UpdateHintButtonUI();
    }

    public void OnHintButtonClicked()
    {
        bool isP1Turn = (gameManager.totalTurns % 2 == 0);
        int remaining = isP1Turn ? p1HintsRemaining : p2HintsRemaining;
        bool alreadyUsed = isP1Turn ? p1HintUsedThisTurn : p2HintUsedThisTurn;
        string currentHint = isP1Turn ? p1CurrentHint : p2CurrentHint;

        if (!alreadyUsed)
        {
            audioManager.PlaySFX("SFX04");
            if (remaining <= 0)
            {
                return;
            }

            currentHint = GenerateHintText();

            if (isP1Turn)
            {
                p1HintsRemaining--;
                p1CurrentHint = currentHint;
                p1HintUsedThisTurn = true;
            }
            else
            {
                p2HintsRemaining--;
                p2CurrentHint = currentHint;
                p2HintUsedThisTurn = true;
            }

            UpdateHintButtonUI();
        }
        else
        {
            if (string.IsNullOrEmpty(currentHint))
            {
                return;
            }
        }

        hintText.text = currentHint;
        hintPanel.SetActive(true);
        audioManager.PlaySFX("SFX04");
    }

    private string GenerateHintText()
    {
        List<string> balloons = new List<string>(gameManager.BalloonList);
        int targetCount = Mathf.Min(gameManager.TargetBalloonCount, balloons.Count);

        int numbersToReveal = CalculateNumbersToReveal(targetCount);
        List<int> revealIndices = GetRandomHintIndices(targetCount, numbersToReveal);

        string hintExpression = BuildHintExpression(balloons, revealIndices, targetCount);

        float target = float.Parse(TargetText.text);
        hintExpression += $" = {NumberFormatter.FormatSmart(target)}";

        return $"Hint:\n{hintExpression}";
    }

    private int CalculateNumbersToReveal(int total)
    {
        if (total <= 2) return 1;
        return Mathf.FloorToInt(total / 2f);
    }

    private List<int> GetRandomHintIndices(int maxIndex, int count)
    {
        List<int> result = new List<int>();
        List<int> available = new List<int>();

        for (int i = 0; i < maxIndex; i++)
            available.Add(i);

        for (int i = 0; i < count && available.Count > 0; i++)
        {
            int randIdx = Random.Range(0, available.Count);
            result.Add(available[randIdx]);
            available.RemoveAt(randIdx);
        }

        result.Sort();
        return result;
    }

    private string BuildHintExpression(List<string> balloons, List<int> revealIndices, int targetCount)
    {
        string expression = "";

        if (GameData.IsSingleMode())
        {
            var mode = GameData.GetSingleMode();
            string opSymbol = mode == OperatorMode.Plus ? "+" : mode == OperatorMode.Minus ? "-" : mode == OperatorMode.Multiply ? "×" : "÷";

            for (int i = 0; i < targetCount; i++)
            {
                if (i > 0) expression += $" {opSymbol} ";

                if (revealIndices.Contains(i))
                    expression += balloons[i];
                else
                    expression += "...";
            }
        }
        else
        {
            int modeIndex = 0;

            for (int i = 0; i < targetCount; i++)
            {
                if (i > 0)
                {
                    var mode = GameData.SelectedModes[modeIndex % GameData.SelectedModes.Count];
                    string opSymbol = mode == OperatorMode.Plus ? "+" : mode == OperatorMode.Minus ? "-" : mode == OperatorMode.Multiply ? "×" : "÷";
                    expression += $" {opSymbol} ";
                    modeIndex++;
                }

                if (revealIndices.Contains(i))
                    expression += balloons[i];
                else
                    expression += "...";
            }
        }

        return expression;
    }

    private void UpdateHintButtonUI()
    {
        bool isP1Turn = (gameManager.totalTurns % 2 == 0);
        int remaining = isP1Turn ? p1HintsRemaining : p2HintsRemaining;
        bool alreadyUsed = isP1Turn ? p1HintUsedThisTurn : p2HintUsedThisTurn;

        hintCountText.text = $"x{remaining}";
        hintButton.interactable = (remaining > 0 || alreadyUsed);
    }

    public void CloseHintPanel()
    {
        hintPanel.SetActive(false);
        audioManager.PlaySFX("SFX02");
    }

    public void CountScore(float answer, bool hasAnswered, float target, bool isP1)
    {
        int baseScore = 0;
        int bonusScore = 0;

        float diff = hasAnswered ? Mathf.Abs(answer - target) : float.MaxValue;

        if (GameData.IsSingleMode())
        {
            var mode = GameData.GetSingleMode();

            switch (mode)
            {
                case OperatorMode.Plus:
                case OperatorMode.Multiply:
                    {
                        if (target > Mathf.Epsilon && hasAnswered)
                        {
                            baseScore = Mathf.RoundToInt((1f - diff / target) * 1000f);
                        }
                    }
                    break;
                case OperatorMode.Minus:
                case OperatorMode.Divide:
                    {
                        float absCurrent = Mathf.Abs(target);

                        if (absCurrent > Mathf.Epsilon && hasAnswered)
                        {
                            baseScore = Mathf.RoundToInt(absCurrent / (absCurrent + diff) * 1000f);
                        }
                        break;
                    }
            }
        }
        else
        {
            bool hasPlusOrMultiply = GameData.HasMode(OperatorMode.Plus) || GameData.HasMode(OperatorMode.Multiply);

            if (hasPlusOrMultiply && target > Mathf.Epsilon && hasAnswered)
            {
                baseScore = Mathf.RoundToInt((1f - diff / target) * 1000f);
            }
            else
            {
                float absTarget = Mathf.Abs(target);

                if (absTarget > Mathf.Epsilon && hasAnswered)
                {
                    baseScore = Mathf.RoundToInt(absTarget / (absTarget + diff) * 1000f);
                }
            }
        }

        baseScore = Mathf.Clamp(baseScore, 0, 1000);

        if (hasAnswered)
        {
            List<BalloonType> usedBalloons = isP1 ? p1BalloonTypes : p2BalloonTypes;
            bonusScore = CalculateSpecialBalloonBonus(usedBalloons, baseScore, diff);
        }

        int totalScore = baseScore + bonusScore;

        if (isP1)
        {
            scoreP1 += totalScore;
            scoreP1Text.text = NumberFormatter.FormatWithCommas(scoreP1);
            AddScoreHistory(currentRound, answer, baseScore, bonusScore, target, true);
            //p1BalloonTypes.Clear();
        }
        else
        {
            scoreP2 += totalScore;
            scoreP2Text.text = NumberFormatter.FormatWithCommas(scoreP2);
            AddScoreHistory(currentRound, answer, baseScore, bonusScore, target, false);
            //p2BalloonTypes.Clear();
            shouldReset = true;
            currentRound++;
        }
    }

    private int CalculateSpecialBalloonBonus(List<BalloonType> usedBalloons, int baseScore, float diff)
    {
        int goldenCount = 0;
        int mysteryCount = 0;
        int comboCount = 0;
        int luckyCount = 0;
        int jokerCount = 0;

        foreach (var type in usedBalloons)
        {
            if (type == BalloonType.Golden) goldenCount++;
            if (type == BalloonType.Mystery) mysteryCount++;
            if (type == BalloonType.Combo) comboCount++;
            if (type == BalloonType.Lucky) luckyCount++;
            if (type == BalloonType.Joker) jokerCount++;
        }

        int bonusScore = 0;

        if (goldenCount > 0)
        {
            int goldenBonus = Mathf.RoundToInt(baseScore * 0.15f * goldenCount);
            bonusScore += goldenBonus;
        }

        if (mysteryCount > 0)
        {
            int mysteryBonus = Mathf.RoundToInt(baseScore * 0.2f * mysteryCount);
            bonusScore += mysteryBonus;
        }

        if (comboCount >= 2)
        {
            int comboBonus = (comboCount >= 3) ? 300 : 100;
            bonusScore += comboBonus;
        }

        if (luckyCount > 0 && Mathf.Approximately(diff, 0f))
        {
            bonusScore += 200 * luckyCount;
        }

        if (jokerCount > 0 && Mathf.Approximately(diff, 0f))
        {
            bonusScore += 1000 * jokerCount;
        }

        return bonusScore;
    }

    public void StoreBalloonTypesForCurrentPlayer()
    {
        List<BalloonType> usedTypes = new List<BalloonType>();

        if (GameData.IsSingleMode())
        {
            BalloonSlot[] balloonSlots = gameManager.GetBalloonSlots();

            if (balloonSlots != null)
            {
                foreach (var slot in balloonSlots)
                {
                    if (slot != null && slot.transform.childCount > 0)
                    {
                        BalloonHitText balloon = slot.transform.GetChild(0).GetComponent<BalloonHitText>();

                        if (balloon != null)
                        {
                            usedTypes.Add(balloon.Type);
                        }
                    }
                }
            }
        }
        else
        {
            NumberSlot[] numberSlots = gameManager.GetNumberSlots();

            if (numberSlots != null)
            {
                foreach (var slot in numberSlots)
                {
                    if (slot != null && slot.HasNumber())
                    {
                        foreach (Transform child in slot.transform)
                        {
                            BalloonHitText balloon = child.GetComponent<BalloonHitText>();

                            if (balloon != null)
                            {
                                usedTypes.Add(balloon.Type);
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (gameManager.totalTurns % 2 == 0)
        {
            p1BalloonTypes = new List<BalloonType>(usedTypes);
        }
        else
        {
            p2BalloonTypes = new List<BalloonType>(usedTypes);
        }
    }

    private void AddScoreHistory(int round, float answer, int baseScore, int bonusScore, float target, bool isP1)
    {
        if (isP1)
        {
            GameObject p1History = Instantiate(scoreHistoryPrefab, p1HistoryContent);
            TextMeshProUGUI p1Text = p1History.GetComponent<TextMeshProUGUI>();
            p1Text.text = $"Round {round}\nScore : +{NumberFormatter.FormatWithCommas(baseScore)}\nBonus : +{NumberFormatter.FormatSmart(bonusScore)}\nTarget : {NumberFormatter.FormatSmart(target)}\nAnswer : {NumberFormatter.FormatSmart(answer)}";
        }
        else
        {
            GameObject p2History = Instantiate(scoreHistoryPrefab, p2HistoryContent);
            TextMeshProUGUI p2Text = p2History.GetComponent<TextMeshProUGUI>();
            p2Text.text = $"Round {round}\nScore : +{NumberFormatter.FormatWithCommas(baseScore)}\nBonus : +{NumberFormatter.FormatSmart(bonusScore)}\nTarget : {NumberFormatter.FormatSmart(target)}\nAnswer : {NumberFormatter.FormatSmart(answer)}";
        }
    }

    /*
    private void AddScoreHistory(int round, float p1Answer, int p1BaseScore, int p1BonusScore, float p2Answer, int p2BaseScore, int p2BonusScore, float target)
    {
        // Player 1 History
        GameObject p1History = Instantiate(scoreHistoryPrefab, p1HistoryContent);
        TextMeshProUGUI p1Text = p1History.GetComponent<TextMeshProUGUI>();

        //p1Text.text = $"Round {round}\nScore : +{p1Score}\nTarget : {target}\nAnswer : {p1Answer}";
        p1Text.text = $"Round {round}\nScore : +{NumberFormatter.FormatWithCommas(p1BaseScore)}\nBonus : +{NumberFormatter.FormatSmart(p1BonusScore)}\nTarget : {NumberFormatter.FormatSmart(target)}\nAnswer : {NumberFormatter.FormatSmart(p1Answer)}";

        // Player 2 History
        GameObject p2History = Instantiate(scoreHistoryPrefab, p2HistoryContent);
        TextMeshProUGUI p2Text = p2History.GetComponent<TextMeshProUGUI>();

        //p2Text.text = $"Round {round}\nScore : +{p2Score}\nTarget : {target}\nAnswer : {p2Answer}";
        p2Text.text = $"Round {round}\nScore : +{NumberFormatter.FormatWithCommas(p2BaseScore)}\nBonus : +{NumberFormatter.FormatSmart(p2BonusScore)}\nTarget : {NumberFormatter.FormatSmart(target)}\nAnswer : {NumberFormatter.FormatSmart(p2Answer)}";
    }
    */

    private IEnumerator ClearResultTexts()
    {
        shouldReset = false;

        yield return new WaitForSecondsRealtime(1f);

        resultP1Text.text = "0";
        resultP2Text.text = "0";
    }
}