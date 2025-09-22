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
        numText.text = Random.Range(0, 101).ToString();
    }
}