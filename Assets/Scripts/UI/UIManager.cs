using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private int scoreP1 = 0, scoreP2 = 0;

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

    [Header("Math Panel UI")]
    [SerializeField] public GameObject calculationPanel;

    [SerializeField] private TextMeshProUGUI totalText;
    public TextMeshProUGUI TotalText => totalText;

    [SerializeField] private Button submitButton;
    public Button SubmitButton => submitButton;

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
    }

    public IEnumerator CountScore(int p1, int p2)
    {
        int current;

        if (gameManager.totalTurns % 2 == 0)
            current = scoreP1;
        else
            current = scoreP2;

        //int target = current + 1000;

        while (true)
        {
            int targetNum = int.Parse(targetText.text);

            int p1Num = p1 - targetNum;
            int p2Num = p2 - targetNum;

            Debug.Log("P1: " + p1Num);
            Debug.Log("P2: " + p2Num);
            /*
            int diff = target - current;
            int step = Mathf.Max(1, Mathf.CeilToInt(diff * 0.3f));
            */

            //current += step;

            //if (current > target) current = target;

            if (p1 > p2)
            {
                scoreP1 += 1000;
                scoreP1Text.text = scoreP1.ToString();
                break;
            }
            else if (p1 < p2)
            {
                scoreP2 += 1000;
                scoreP2Text.text = scoreP2.ToString();
                break;
            }

            yield return new WaitForSeconds(0.05f);
            //yield return null;
            //break;
        }
    }
}