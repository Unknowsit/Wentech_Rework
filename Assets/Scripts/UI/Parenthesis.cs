using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Parenthesis : MonoBehaviour
{
    [Header("Parenthesis Button")]
    [SerializeField] private Button parenthesisButton;
    [SerializeField] private TextMeshProUGUI parenthesisText;
    [SerializeField] private Image parenthesisImage;

    [Header("Parenthesis Colors")]
    [SerializeField] private Color noneColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    [SerializeField] private Color openColor = new Color(0f, 1f, 0f, 0.8f);
    [SerializeField] private Color closeColor = new Color(1f, 0.5f, 0f, 0.8f);
    [SerializeField] private Color errorColor = new Color(1f, 0f, 0f, 0.8f);

    private ParenthesisType currentType = ParenthesisType.None;
    private GameManager gameManager;
    private bool isInitialized = false;

    public ParenthesisType CurrentType => currentType;

    private void OnEnable()
    {
        if (isInitialized)
        {
            UpdateVisual();
        }
    }

    private void Start()
    {
        InitializeIfNeeded();
        RefreshUI();
        UpdateVisual();
    }

    private void OnDestroy()
    {
        parenthesisButton.onClick.RemoveListener(CycleParenthesis);
    }

    private void InitializeIfNeeded()
    {
        if (isInitialized) return;

        gameManager = GameManager.instance;
        parenthesisButton.onClick.AddListener(CycleParenthesis);

        isInitialized = true;
    }

    public void CycleParenthesis()
    {
        if (!GameData.ShouldUseParentheses()) return;

        currentType = (ParenthesisType)(((int)currentType + 1) % 3);
        UpdateVisual();

        gameManager.ValidateParentheses();
        gameManager.UpdateBalloonSum(UIManager.instance.MultiTotalText);
        AudioManager.instance.PlaySFX("SFX04");
    }

    public void ResetType()
    {
        currentType = ParenthesisType.None;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        switch (currentType)
        {
            case ParenthesisType.None:
                parenthesisText.text = "X";
                parenthesisImage.color = noneColor;
                break;
            case ParenthesisType.Open:
                parenthesisText.text = "(";
                parenthesisImage.color = openColor;
                break;
            case ParenthesisType.Close:
                parenthesisText.text = ")";
                parenthesisImage.color = closeColor;
                break;
        }
    }

    public void SetErrorState(bool isError)
    {
        if (isError)
        {
            parenthesisImage.color = errorColor;
        }
        else
        {
            UpdateVisual();
        }
    }

    public void RefreshUI()
    {
        bool shouldShow = GameData.ShouldUseParentheses();

        parenthesisButton.gameObject.SetActive(shouldShow);
        enabled = shouldShow;

        if (!shouldShow)
        {
            currentType = ParenthesisType.None;
        }

        UpdateVisual();
    }
}