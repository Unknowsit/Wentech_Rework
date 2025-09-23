using TMPro;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
        RandomNumber();
        gameManager.RegisterBalloonNumber(numText.text);
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
        switch (GameData.SelectedMode)
        {
            case OperatorMode.Add or OperatorMode.Subtract:
                numText.text = Random.Range(0, 101).ToString();
                break;
            case OperatorMode.Multiply or OperatorMode.Divide:
                numText.text = Random.Range(2, 13).ToString();
                break;
        }
    }
}