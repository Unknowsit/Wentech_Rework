using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Setup UI")]
    public TMP_InputField targetInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI numTarget;
    public TextMeshProUGUI NumTarget => numTarget;

    [Header("Timer Settings")]
    [SerializeField] private Slider _progressBar;
    public Slider _ProgressBar { get { return _progressBar; } set { _progressBar = value; } }

    [SerializeField] private float remainingTime;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

    [Header ("Gameplay Panel UI")]
    [SerializeField] private GameObject calculationPanel;

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

    public void ShowCalculationPanel()
    {
        calculationPanel.SetActive(true);
    }
}