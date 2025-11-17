using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 direction;

    private GameManager gameManager;
    private UIController uiController;
    private UIManager uiManager;
    private AudioManager audioManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
        uiController = UIController.instance;
        uiManager = UIManager.instance;
        audioManager = AudioManager.instance;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //gameManager.CalculateBalloon();
            uiController.RunTransition();

            if (GameData.IsSingleMode())
            {
                uiManager.SubmitButton.interactable = true;
            }
            else
            {
                uiManager.MultiSubmitButton.interactable = true;
            }

            gameManager.SpawnBalloonHitTexts();
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Box"))
        {
            audioManager.PlaySFX("SFX06");
        }

        var firstContact = collision.contacts[0];
        Vector2 incoming = rb.linearVelocity.normalized;
        Vector2 reflected = Vector2.Reflect(incoming, firstContact.normal).normalized;

        transform.position = firstContact.point + reflected * 0.05f;
        rb.linearVelocity = reflected * speed;
    }

    public void Shoot(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
    }
}