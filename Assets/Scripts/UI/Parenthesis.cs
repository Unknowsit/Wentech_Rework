using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Parenthesis : MonoBehaviour
{
    [Header("Parenthesis Button")]
    [SerializeField] private Button parenthesisButton;
    //[SerializeField] private TextMeshProUGUI parenthesisText;
    [SerializeField] private Image parenthesisImage;
    [SerializeField] private Image parenthesisBackground;

    [Header("Parenthesis Sprites")]
    [SerializeField] private Sprite parenthesisNone;
    [SerializeField] private Sprite parenthesisOpen;
    [SerializeField] private Sprite parenthesisDoubleOpen;
    [SerializeField] private Sprite parenthesisClose;
    [SerializeField] private Sprite parenthesisDoubleClose;

    [Header("Parenthesis Colors")]
    [SerializeField] private Color noneColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    [SerializeField] private Color openColor = new Color(0f, 1f, 0f, 0.8f);
    [SerializeField] private Color closeColor = new Color(1f, 0.5f, 0f, 0.8f);
    [SerializeField] private Color errorColor = new Color(1f, 0f, 0f, 0.8f);

    private ParenthesisType currentType = ParenthesisType.None;
    private GameManager gameManager;
    private bool isInitialized = false;
    private int slotIndex = -1;

    public ParenthesisType CurrentType => currentType;
    public int SlotIndex => slotIndex;

    private void Awake()
    {
        FindSlotIndex();
    }

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

    private void FindSlotIndex()
    {
        var numberSlotsField = typeof(GameManager).GetField("numberSlots", BindingFlags.NonPublic | BindingFlags.Instance);

        if (numberSlotsField != null)
        {
            var numberSlots = numberSlotsField.GetValue(GameManager.instance) as NumberSlot[];

            if (numberSlots != null)
            {
                NumberSlot mySlot = GetComponent<NumberSlot>();

                for (int i = 0; i < numberSlots.Length; i++)
                {
                    if (numberSlots[i] == mySlot)
                    {
                        slotIndex = i;
                        //Debug.Log($"[Parenthesis] Found slot index: {slotIndex} for {gameObject.name}");
                        return;
                    }
                }
            }
        }

        Transform parent = transform.parent;

        if (parent != null)
        {
            int numberSlotCount = 0;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);

                if (child.GetComponent<NumberSlot>() != null)
                {
                    if (child == transform)
                    {
                        slotIndex = numberSlotCount;
                        //Debug.Log($"[Parenthesis] Fallback - Found slot index: {slotIndex}");
                        return;
                    }
                    numberSlotCount++;
                }
            }
        }

        //Debug.LogWarning($"[Parenthesis] Could not find slot index for {gameObject.name}");
    }

    public void CycleParenthesis()
    {
        if (!GameData.ShouldUseParentheses()) return;

        //Debug.Log($"Cycling parenthesis for slot {slotIndex}, current type: {currentType}");

        if (slotIndex == 0)
        {
            switch (currentType)
            {
                case ParenthesisType.None:
                    currentType = ParenthesisType.Open;
                    break;
                case ParenthesisType.Open:
                    currentType = ParenthesisType.DoubleOpen;
                    break;
                default:
                    currentType = ParenthesisType.None;
                    break;
            }
        }
        else if (slotIndex == 3)
        {
            switch (currentType)
            {
                case ParenthesisType.None:
                    currentType = ParenthesisType.Close;
                    break;
                case ParenthesisType.Close:
                    currentType = ParenthesisType.DoubleClose;
                    break;
                default:
                    currentType = ParenthesisType.None;
                    break;
            }
        }
        else
        {
            switch (currentType)
            {
                case ParenthesisType.None:
                    currentType = ParenthesisType.Open;
                    break;
                case ParenthesisType.Open:
                    currentType = ParenthesisType.Close;
                    break;
                case ParenthesisType.Close:
                case ParenthesisType.DoubleOpen:
                case ParenthesisType.DoubleClose:
                    currentType = ParenthesisType.None;
                    break;
                default:
                    currentType = ParenthesisType.None;
                    break;
            }
        }

        //Debug.Log($"New type: {currentType}");

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
                //parenthesisText.text = "X";
                parenthesisImage.sprite = parenthesisNone;
                parenthesisBackground.color = noneColor;
                break;
            case ParenthesisType.Open:
                //parenthesisText.text = "(";
                parenthesisImage.sprite = parenthesisOpen;
                parenthesisBackground.color = openColor;
                break;
            case ParenthesisType.Close:
                //parenthesisText.text = ")";
                parenthesisImage.sprite = parenthesisClose;
                parenthesisBackground.color = closeColor;
                break;
            case ParenthesisType.DoubleOpen:
                //parenthesisText.text = "((";
                parenthesisImage.sprite = parenthesisDoubleOpen;
                parenthesisBackground.color = openColor;
                break;
            case ParenthesisType.DoubleClose:
                //parenthesisText.text = "))";
                parenthesisImage.sprite = parenthesisDoubleClose;
                parenthesisBackground.color = closeColor;
                break;
        }
    }

    public void SetErrorState(bool isError)
    {
        if (isError)
        {
            parenthesisBackground.color = errorColor;
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