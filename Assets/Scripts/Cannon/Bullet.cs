using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 direction;

    private GameManager gameManager;
    private UIController uiController;
    private UIManager uiManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
        uiController = UIController.instance;
        uiManager = UIManager.instance;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            gameManager.CalculateBalloon();
            uiController.RunTransition();
            uiManager.SubmitButton.interactable = true;
            gameManager.SpawnBalloonHitTexts();
            Destroy(gameObject);
        }

        var firstContact = collision.contacts[0];
        Vector2 newVelocity = Vector2.Reflect(direction.normalized, firstContact.normal);
        Shoot(newVelocity.normalized);
    }

    public void Shoot(Vector2 direction)
    {
        this.direction = direction;
        rb.linearVelocity = this.direction * speed;
    }
}