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
    [Header("Game Setup UI")]
    public TMP_InputField targetInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI targetText;
    public TextMeshProUGUI TargetText => targetText;
    [SerializeField] private TextMeshProUGUI totatText;
    public TextMeshProUGUI TotalText => totatText;

    [Header("Timer Settings")]
    [SerializeField] private Slider _progressBar;
    public Slider _ProgressBar { get { return _progressBar; } set { _progressBar = value; } }

    [SerializeField] private float remainingTime;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    [Header ("Gameplay Panel UI")]
    [SerializeField] public GameObject calculationPanel;

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
        gameManager = GameManager.instance;
        uiController = UIController.instance;
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
        StartCoroutine(uiController.TransitionRoutine(false));
        gameManager.SetPlayerValues();
    }
}