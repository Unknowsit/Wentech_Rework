using TMPro;
using UnityEngine;

public class BalloonHitText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
        DisplayBalloonHitCount(gameManager.currentBalloonIndex);
        gameManager.currentBalloonIndex++;
    }

    public void DisplayBalloonHitCount(int i)
    {
        numText.text = gameManager.BalloonHitCounts[i].ToString();
    }
}