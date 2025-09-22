using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int attempts = 0;
    private int spawnedCount = 0;

    [Header ("Balloon Properties")]
    [SerializeField] private int totalBalloons = 10;
    [SerializeField] private int targetBalloonCount = 3;
    [SerializeField] private float balloonCollisionRadius = 0.5f;
    [SerializeField] private GameObject balloonPrefab;
    [SerializeField] private Transform balloonParent;

    [Header("Balloon Game Data")]
    [SerializeField] private List<string> balloonList = new List<string>();
    [SerializeField] private List<int> balloonHitCounts = new List<int>();

    [Header("Spawning Area")]
    public Vector2 areaSize = new Vector2(10f, 5f);

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
            Destroy(gameObject);
        }
    }

    private void StartGame()
    {
        GenerateBalloon();
        CalculateTargetSum();
    }

    public void SetTargetBalloonCount(int count)
    {
        targetBalloonCount = Mathf.Clamp(count, 1, totalBalloons);
        StartGame();
    }

    public void RegisterBalloonNumber(string num)
    {
        balloonList.Add(num);
    }

    public void RegisterBalloonHit(int num)
    {
        balloonHitCounts.Add(num);
    }

    private void GenerateBalloon()
    {
        while (spawnedCount < totalBalloons && attempts < 101)
        {
            float randomX = Random.Range(-areaSize.x / 2, areaSize.x / 2);
            float randomY = Random.Range(-areaSize.y / 2, areaSize.y / 2);

            Vector2 targetPos = new Vector2(randomX, randomY);

            if (Physics2D.OverlapCircle(targetPos, balloonCollisionRadius) == null)
            {
                Instantiate(balloonPrefab, targetPos, Quaternion.identity, balloonParent);
                spawnedCount++;
            }
            attempts++;
        }
    }

    private void CalculateTargetSum()
    {
        int sum = int.Parse(balloonList[0]);
        string resultText = sum.ToString();

        for (int i = 1; i < targetBalloonCount; i++)
        {
            if (int.TryParse(balloonList[i], out int value))
            {
                switch (GameData.SelectedMode)
                {
                    case OperatorMode.Add:
                        sum += value;
                        resultText += " + " + value;
                        break;
                    case OperatorMode.Subtract:
                        sum -= value;
                        resultText += " - " + value;
                        break;
                    case OperatorMode.Multiply:
                        sum *= value;
                        resultText += " * " + value;
                        break;
                    case OperatorMode.Divide:
                        if (value != 0) sum /= value;
                        resultText += " / " + value;
                        break;
                }
            }
        }

        Debug.Log($"Expression: {resultText} = {sum}");
    }

    public void CalculateBalloon()
    {
        int sum = 0;

        for (int i = 0; i < balloonHitCounts.Count; i++)
        {
            switch (GameData.SelectedMode)
            {
                case OperatorMode.Add:
                    sum += balloonHitCounts[i];
                    break;
                case OperatorMode.Subtract:
                    sum -= balloonHitCounts[i];
                    break;
                case OperatorMode.Multiply:
                    sum = (i == 0) ? balloonHitCounts[i] : sum * balloonHitCounts[i];
                    break;
                case OperatorMode.Divide:
                    if (balloonHitCounts[i] != 0)
                        sum = (i == 0) ? balloonHitCounts[i] : sum / balloonHitCounts[i];
                    break;
            }
        }

        Debug.Log("Player Result: " + sum);

        /*
        if (GameData.SelectedMode == OperatorMode.Divide)
        {
            Debug.Log("Player Result: " + sum.ToString("F3"));
        }
        else
        {
            Debug.Log("Player Result: " + sum);
        }
        */
    }

    /*
    private void CalculateTargetSum()
    {
        int sum = 0;
        string resultText = "";
        
        for (int i = 0; i < targetBalloonCount; i++)
        {
            if (int.TryParse(balloonList[i], out int value))
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
        int sum = 0;

        for (int i = 0; i < balloonHitCounts.Count; i++)
        {
            sum += balloonHitCounts[i];
        }

        Debug.Log(sum);
    }
    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector2.zero, areaSize);
    }
}