using TMPro;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private BalloonRangeData rangeSettings;

    [Header("Special Balloon Visuals")]
    [SerializeField] private SpriteRenderer balloonSpriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite mysterySprite;
    [SerializeField] private Sprite goldenSprite;
    [SerializeField] private Sprite luckySprite;
    [SerializeField] private Sprite comboSprite;
    [SerializeField] private Sprite jokerSprite;

    [Header("Text Colors")]
    [SerializeField] private Color normalTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color mysteryTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color goldenTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color comboTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color luckyTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color jokerTextColor = new Color(1f, 1f, 1f, 1f);

    private GameManager gameManager;
    private BalloonType balloonType = BalloonType.Normal;
    private int actualValue;

    public BalloonType Type => balloonType;

    private void Awake()
    {
        gameManager = GameManager.instance;

        if (gameManager.totalTurns % 2 == 0)
        {
            DetermineBalloonType();
            RandomNumber();

            string valueToSave = (balloonType == BalloonType.Mystery) ? actualValue.ToString() : numberText.text;
            gameManager.RegisterBalloonNumber(valueToSave);
            gameManager.RegisterBalloonType(balloonType);
        }
        else
        {
            string savedValue = gameManager.BalloonList[gameManager.currentBalloonIndex];
            balloonType = gameManager.BalloonTypes[gameManager.currentBalloonIndex];

            if (balloonType == BalloonType.Joker)
            {
                RandomNumber();
                gameManager.BalloonList[gameManager.currentBalloonIndex] = numberText.text;
            }
            else if (balloonType == BalloonType.Mystery)
            {
                actualValue = int.Parse(savedValue);
                numberText.text = "?";
            }
            else
            {
                numberText.text = savedValue;
            }

            UpdateVisuals();
            gameManager.currentBalloonIndex++;
        }
    }

    private void DetermineBalloonType()
    {
        balloonType = gameManager.GetNextBalloonType();
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        switch (balloonType)
        {
            case BalloonType.Golden:
                balloonSpriteRenderer.sprite = goldenSprite;
                numberText.color = goldenTextColor;
                break;
            case BalloonType.Mystery:
                balloonSpriteRenderer.sprite = mysterySprite;
                numberText.color = mysteryTextColor;
                break;
            case BalloonType.Lucky:
                balloonSpriteRenderer.sprite = luckySprite;
                numberText.color = luckyTextColor;
                break;
            case BalloonType.Combo:
                balloonSpriteRenderer.sprite = comboSprite;
                numberText.color = comboTextColor;
                break;
            case BalloonType.Joker:
                balloonSpriteRenderer.sprite = jokerSprite;
                numberText.color = jokerTextColor;
                break;
            default:
                balloonSpriteRenderer.sprite = normalSprite;
                numberText.color = normalTextColor;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            AudioManager.instance.PlaySFX("SFX02");

            int valueToRegister;

            if (balloonType == BalloonType.Mystery)
            {
                valueToRegister = actualValue;
            }
            else
            {
                valueToRegister = int.Parse(numberText.text);
            }

            gameManager.RegisterBalloonHitType(valueToRegister, balloonType);
            Destroy(gameObject);
        }
    }

    private void RandomNumber()
    {
        if (balloonType == BalloonType.Golden)
        {
            int[] goldenValues = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            int randomValue = goldenValues[Random.Range(0, goldenValues.Length)];
            numberText.text = randomValue.ToString();
        }
        else if (balloonType == BalloonType.Joker)
        {
            if (GameData.IsSingleMode())
            {
                GenerateRandomNumber(GameData.GetSingleMode());
            }
            else
            {
                GenerateRandomNumber(GameData.GetRandomMode());
            }
        }
        else
        {
            if (GameData.IsSingleMode())
            {
                GenerateRandomNumber(GameData.GetSingleMode());
            }
            else
            {
                GenerateRandomNumber(GameData.GetRandomMode());
            }
        }

        if (balloonType == BalloonType.Mystery)
        {
            actualValue = int.Parse(numberText.text);
            numberText.text = "?";
        }
    }

    private void GenerateRandomNumber(OperatorMode mode)
    {
        int min = rangeSettings.GetMinValue(mode);
        int max = rangeSettings.GetMaxValue(mode);
        numberText.text = Random.Range(min, max + 1).ToString();
    }

    public string GetBalloonNumber()
    {
        if (balloonType == BalloonType.Mystery)
        {
            return actualValue.ToString();
        }
        return numberText.text;
    }
}