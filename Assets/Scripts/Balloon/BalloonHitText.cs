using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalloonHitText : MonoBehaviour
{
    private int value;
    public int Value => value;

    private BalloonType balloonType;
    public BalloonType Type => balloonType;

    [SerializeField] private TextMeshProUGUI numText;
    [SerializeField] private Image balloonImage;

    [Header("Special Balloon Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite mysterySprite;
    [SerializeField] private Sprite goldenSprite;
    [SerializeField] private Sprite comboSprite;
    [SerializeField] private Sprite luckySprite;
    [SerializeField] private Sprite jokerSprite;

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
        numText.text = gameManager.BalloonHitCounts[i].ToString();
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
            case BalloonType.Mystery:
                balloonImage.sprite = mysterySprite;
                break;
            case BalloonType.Golden:
                balloonImage.sprite = goldenSprite;
                break;
            case BalloonType.Combo:
                balloonImage.sprite = comboSprite;
                break;
            case BalloonType.Lucky:
                balloonImage.sprite = luckySprite;
                break;
            case BalloonType.Joker:
                balloonImage.sprite = jokerSprite;
                break;
            default:
                balloonImage.sprite = normalSprite;
                break;
        }
    }
}