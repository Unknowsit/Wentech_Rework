using TMPro;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numText;
    [SerializeField] private BalloonRangeData rangeSettings;

    [Header("Special Balloon Visuals")]
    [SerializeField] private SpriteRenderer balloonSpriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite mysterySprite;
    [SerializeField] private Sprite goldenSprite;
    [SerializeField] private Sprite luckySprite;
    [SerializeField] private Sprite comboSprite;
    [SerializeField] private Sprite jokerSprite;

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

            string valueToSave = (balloonType == BalloonType.Mystery) ? actualValue.ToString() : numText.text;
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
                gameManager.BalloonList[gameManager.currentBalloonIndex] = numText.text;
            }
            else if (balloonType == BalloonType.Mystery)
            {
                actualValue = int.Parse(savedValue);
                numText.text = "?";
            }
            else
            {
                numText.text = savedValue;
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
            case BalloonType.Mystery:
                balloonSpriteRenderer.sprite = mysterySprite;
                break;
            case BalloonType.Golden:
                balloonSpriteRenderer.sprite = goldenSprite;
                break;
            case BalloonType.Lucky:
                balloonSpriteRenderer.sprite = luckySprite;
                break;
            case BalloonType.Combo:
                balloonSpriteRenderer.sprite = comboSprite;
                break;
            case BalloonType.Joker:
                balloonSpriteRenderer.sprite = jokerSprite;
                break;
            default:
                balloonSpriteRenderer.sprite = normalSprite;
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
                valueToRegister = int.Parse(numText.text);
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
            numText.text = randomValue.ToString();
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
            actualValue = int.Parse(numText.text);
            numText.text = "?";
        }
    }

    private void GenerateRandomNumber(OperatorMode mode)
    {
        int min = rangeSettings.GetMinValue(mode);
        int max = rangeSettings.GetMaxValue(mode);
        numText.text = Random.Range(min, max + 1).ToString();
    }

    public string GetBalloonNumber()
    {
        if (balloonType == BalloonType.Mystery)
        {
            return actualValue.ToString();
        }
        return numText.text;
    }
}