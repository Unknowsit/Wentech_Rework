using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalloonHitText : MonoBehaviour
{
    [SerializeField] private int value;
    public int Value => value;

    private BalloonType balloonType;
    public BalloonType Type => balloonType;

    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Image balloonImage;

    [Header("Special Balloon Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite mysterySprite;
    [SerializeField] private Sprite goldenSprite;
    [SerializeField] private Sprite comboSprite;
    [SerializeField] private Sprite luckySprite;
    [SerializeField] private Sprite jokerSprite;

    [Header("Text Colors")]
    [SerializeField] private Color normalTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color mysteryTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color goldenTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color comboTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color luckyTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color jokerTextColor = new Color(1f, 1f, 1f, 1f);

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;

        int index = gameManager.currentBalloonIndex;

        DisplayBalloonHitCount(index);
        InitializeValue(index);
        balloonType = gameManager.BalloonHitTypes[index];

        UpdateVisuals();

        gameManager.currentBalloonIndex++;
    }

    public void DisplayBalloonHitCount(int i)
    {
        numberText.text = gameManager.BalloonHitCounts[i].ToString();
    }

    public void InitializeValue(int i)
    {
        value = gameManager.BalloonHitCounts[i];
    }

    private void UpdateVisuals()
    {
        if (balloonImage == null)
        {
            Debug.LogWarning($"[BalloonHitText] balloonImage is null on {gameObject.name}");
            return;
        }

        switch (balloonType)
        {
            case BalloonType.Golden:
                balloonImage.sprite = goldenSprite;
                numberText.color = goldenTextColor;
                break;
            case BalloonType.Mystery:
                balloonImage.sprite = mysterySprite;
                numberText.color = mysteryTextColor;
                break;
            case BalloonType.Combo:
                balloonImage.sprite = comboSprite;
                numberText.color = comboTextColor;
                break;
            case BalloonType.Lucky:
                balloonImage.sprite = luckySprite;
                numberText.color = luckyTextColor;
                break;
            case BalloonType.Joker:
                balloonImage.sprite = jokerSprite;
                numberText.color = jokerTextColor;
                break;
            default:
                balloonImage.sprite = normalSprite;
                numberText.color = normalTextColor;
                break;
        }
    }
}