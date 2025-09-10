using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int attempts = 0;
    private int spawnedCount = 0;

    [Header ("Balloon Settings")]
    [SerializeField] private int totalBalloons = 10;
    [SerializeField] private int targetBalloonCount = 3;
    [SerializeField] private float balloonCollisionRadius = 0.5f;
    [SerializeField] private GameObject prefabBalloon;
    [SerializeField] private List<string> balloonList = new List<string>();

    [Header ("Building...")]
    [SerializeField] private List<short> hitList = new List<short>();

    [Header("Spawning Area")]
    public Vector2 areaSize = new Vector2(10f, 5f);

    [Header("Cannon Settings")]
    public float rotationCannonSpeed = 100f;

    [Header("Other Settings")]
    public float rotationSpeed = 50f;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        GenerateBalloon();
        CalculateTargetSum();
    }

    public void RegisterBalloonNumber(string num)
    {
        balloonList.Add(num);
    }

    public void RegisterBalloonHit(short num)
    {
        hitList.Add(num);
    }

    private void GenerateBalloon()
    {
        while (spawnedCount < totalBalloons && attempts < totalBalloons)
        {
            float randomX = Random.Range(-areaSize.x / 2, areaSize.x / 2);
            float randomY = Random.Range(-areaSize.y / 2, areaSize.y / 2);

            Vector2 targetPos = new Vector2(randomX, randomY);

            if (Physics2D.OverlapCircle(targetPos, balloonCollisionRadius) == null)
            {
                Instantiate(prefabBalloon, targetPos, Quaternion.identity);
                spawnedCount++;
            }
            attempts++;
        }
    }

    private void CalculateTargetSum()
    {
        short sum = 0;
        string resultText = "";

        for (int i = 0; i < targetBalloonCount; i++)
        {
            if (short.TryParse(balloonList[i], out short value))
            {
                sum += value;
                resultText += value;

                if (i < targetBalloonCount - 1)
                {
                    resultText += " + ";
                }
            }
        }
        Debug.Log("Result: " + sum);
        Debug.Log($"Expression: {resultText} = {sum}");
    }

    public void CalculateBalloon()
    {
        short sum = 0;

        for (int i = 0; i < hitList.Count; i++)
        {
            sum += hitList[i];
        }

        Debug.Log(sum);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector2.zero, areaSize);
    }
}