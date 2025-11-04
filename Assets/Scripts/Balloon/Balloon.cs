using TMPro;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;

        if (gameManager.totalTurns % 2 == 0)
        {
            RandomNumber();
            gameManager.RegisterBalloonNumber(numText.text);
        }
        else
        {
            numText.text = gameManager.BalloonList[gameManager.currentBalloonIndex];
            gameManager.currentBalloonIndex++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            AudioManager.instance.PlaySFX("SFX02");
            gameManager.RegisterBalloonHit(int.Parse(numText.text));
            Destroy(gameObject);
        }
    }

    private void RandomNumber()
    {
        if (GameData.IsSingleMode())
        {
            switch (GameData.GetSingleMode())
            {
                case OperatorMode.Add:
                    numText.text = Random.Range(1, 101).ToString();
                    break;
                case OperatorMode.Minus:
                    numText.text = Random.Range(-100, 0).ToString();
                    break;
                case OperatorMode.Multiply:
                case OperatorMode.Divide:
                    numText.text = Random.Range(2, 13).ToString();
                    break;
            }
        }
        else
        {
            var randomMode = GameData.GetRandomMode();

            switch (randomMode)
            {
                case OperatorMode.Add:
                    numText.text = Random.Range(1, 101).ToString();
                    break;
                case OperatorMode.Minus:
                    numText.text = Random.Range(-100, 0).ToString();
                    break;
                case OperatorMode.Multiply:
                case OperatorMode.Divide:
                    numText.text = Random.Range(2, 13).ToString();
                    break;
            }
        }
    }
}