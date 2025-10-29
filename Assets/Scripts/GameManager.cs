using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool hasSpawned = false;

    private int attempts = 0;
    private int spawnedCount = 0;

    public int p1, p2;
    public int totalTurns = 1;

    [HideInInspector] public int currentBalloonIndex = 0;

    [Header ("Balloon Properties")]
    [Range(1, 20)] [SerializeField] private int totalBalloons = 10;
    [SerializeField] private int targetBalloonCount = 3;
    [SerializeField] private float balloonCollisionRadius = 0.5f;
    [SerializeField] private GameObject balloonPrefab;
    [SerializeField] private Transform balloonParent;

    [Header("Balloon Game Data")]
    [SerializeField] private List<string> balloonList = new List<string>();
    public List<string> BalloonList { get { return balloonList; } }

    [SerializeField] private List<int> balloonHitCounts = new List<int>();
    public List<int> BalloonHitCounts { get { return balloonHitCounts; } }

    [SerializeField] private BalloonSlot[] balloonSlots;

    [Header("Balloon UI Prefabs")]
    [SerializeField] private GameObject balloonHitPrefab;
    [SerializeField] private Transform balloonHitParent;

    [Header("Spawning Area")]
    public Vector2 areaSize = new Vector2(10f, 5f);

    [Header("Other Settings")]
    public float rotationSpeed = 50f;

    public CannonAim cannonAim;
    public CannonShooter cannonShooter;
    public Timer timer;

    public static GameManager instance;
    private UIManager uiManager;

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

    private void Start()
    {
        uiManager = UIManager.instance;
    }

    public void RestartGame()
    {
        ResetGameState();
        GenerateBalloon();
        CalculateTargetSum();
        currentBalloonIndex = 0;
    }

    private void ResetGameState()
    {
        DestroyChildren(balloonParent);
        DestroyChildren(balloonHitParent);
        ClearBalloonSlots();

        if (totalTurns % 2 != 0) balloonList.Clear();
        balloonHitCounts.Clear();
        ResetCounters();

        totalTurns--;
        hasSpawned = false;
        cannonShooter.enabled = true;
        ResetUI();
    }

    private void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    public void ClearBalloonSlots()
    {
        foreach (BalloonSlot slot in balloonSlots)
        {
            for (int i = slot.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.transform.GetChild(i).gameObject);
            }
        }
    }

    private void ResetCounters()
    {
        spawnedCount = 0;
        attempts = 0;
        currentBalloonIndex = 0;
    }

    private void ResetUI()
    {
        uiManager.TotalText.text = "0";
        uiManager.RemainingTime = 100;
    }

    public void SetPlayerValues()
    {
        if (totalTurns % 2 == 0)
        {
            p1 = int.Parse(uiManager.TotalText.text);
            uiManager.ResultP1Text.text = $"Answer : {p1.ToString()}";
            uiManager.PlayerText.text = "P2";
        }
        else
        {
            p2 = int.Parse(uiManager.TotalText.text);
            uiManager.ResultP2Text.text = $"Answer : {p2.ToString()}";
            uiManager.PlayerText.text = "P1";
            uiManager.CountScore(p1, p2);
        }
    }

    public void SetTargetRounds(int count)
    {
        totalTurns = count * 2;
    }

    public void SetBalloonSpawnCount(int count)
    {
        totalBalloons = Mathf.Clamp(count, 10, 20);
        GenerateBalloon();
        CalculateTargetSum();
    }

    /*
    public void SetTargetBalloonCount(int count)
    {
        targetBalloonCount = Mathf.Clamp(count, 1, totalBalloons);
    }
    */

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
                    case OperatorMode.Minus:
                        sum -= value;
                        resultText += $" - ({value})";
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
        uiManager.TargetText.text = sum.ToString();
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
                case OperatorMode.Minus:
                    sum -= balloonHitCounts[i];
                    break;
                case OperatorMode.Multiply:
                    sum *= balloonHitCounts[i];
                    //sum = (i == 0) ? balloonHitCounts[i] : sum * balloonHitCounts[i];
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

    public void SpawnBalloonHitTexts()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        for (int i = 0; i < balloonHitCounts.Count; i++)
        {
            Instantiate(balloonHitPrefab, transform.position, Quaternion.identity, balloonHitParent);
        }
    }

    public void UpdateBalloonSum()
    {
        int sum = 0;

        switch (GameData.SelectedMode)
        {
            case OperatorMode.Add:
                foreach (var slot in balloonSlots)
                {
                    sum += slot.GetBalloonValue();
                }

                uiManager.TotalText.text = sum.ToString();
                break;
            case OperatorMode.Minus:
                for (int i = 0; i < balloonSlots.Length; i++)
                {
                    int value = balloonSlots[i].GetBalloonValue();
                    if (value == 0) continue;
                    sum = (sum == 0) ? value : sum - value;
                }

                uiManager.TotalText.text = sum.ToString();
                break;
            case OperatorMode.Multiply:
            {
                int sumMultiply = 1;
                bool hasBalloon = false;

                foreach (var slot in balloonSlots)
                {
                    int value = slot.GetBalloonValue();

                    if (value != 0)
                    {
                        sumMultiply *= value;
                        hasBalloon = true;
                    }
                }

                sum = hasBalloon ? sumMultiply : 0;
                uiManager.TotalText.text = sum.ToString();
                break;
            }
            case OperatorMode.Divide:
            {
                float sumDivide = 0;
                bool firstFound = false;

                foreach (var slot in balloonSlots)
                {
                    int value = slot.GetBalloonValue();
                    if (value == 0) continue;

                    sumDivide = firstFound ? sumDivide / value : value;
                    firstFound = true;
                }

                uiManager.TotalText.text = firstFound ? sumDivide.ToString("F2") : "0.00";
                break;
            }
        }
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