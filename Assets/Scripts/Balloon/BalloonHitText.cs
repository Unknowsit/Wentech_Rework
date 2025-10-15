using TMPro;
using UnityEngine;

public class BalloonHitText : MonoBehaviour
{
    private int value;
    public int Value => value;

    [SerializeField] private TextMeshProUGUI numText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
        DisplayBalloonHitCount(gameManager.currentBalloonIndex);
        InitializeValue(gameManager.currentBalloonIndex);
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
}