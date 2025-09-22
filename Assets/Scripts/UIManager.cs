using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum OperatorMode
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public static class GameData
{
    public static OperatorMode SelectedMode = OperatorMode.Add;
}

public class UIManager : MonoBehaviour
{
    private bool isConfirming = false;

    [Header ("Timer Settings")]
    [SerializeField] private Slider _progressBar;
    public Slider _ProgressBar { get { return _progressBar; } set { _progressBar = value; } }
    // [SerializeField] private TextMeshProUGUI timerText;
    // public TextMeshProUGUI TimerText { get { return timerText; } set { timerText = value; } }
    [SerializeField] private float remainingTime;
    public float RemainingTime { get { return remainingTime; } set { remainingTime = value; } }

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

    public void OnAddModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Add;
    }

    public void OnSubtractModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Subtract;
    }

    public void OnMultiplyModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Multiply;
    }

    public void OnDivideModeSelected()
    {
        isConfirming = true;
        GameData.SelectedMode = OperatorMode.Divide;
    }

    public void CancelConfirming()
    {
        isConfirming = false;
    }

    public void OnConfirmButtonClicked()
    {
        if (isConfirming)
        {
            SceneManager.LoadScene("Gameplay");
        }
    }
}