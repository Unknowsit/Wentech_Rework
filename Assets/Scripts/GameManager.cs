using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool hasSpawned = false;

    private bool p1HasAnswered = false;
    private bool p2HasAnswered = false;

    private int attempts = 0;
    private int spawnedCount = 0;

    public float p1, p2;
    public int totalTurns = 1;
    private int maxRounds = 1;

    [HideInInspector] public int currentBalloonIndex = 0;

    [Header ("Balloon Properties")]
    [Range(1, 20)] [SerializeField] private int totalBalloons = 10;
    [Range(2, 8)] [SerializeField] private int targetBalloonCount = 2;
    [SerializeField] private float balloonCollisionRadius = 0.5f;
    [SerializeField] private GameObject balloonPrefab;
    [SerializeField] private Transform balloonParent;

    [Header("Balloon Game Data")]
    [SerializeField] private List<string> balloonList = new List<string>();
    public List<string> BalloonList { get { return balloonList; } }

    [SerializeField] private List<int> balloonHitCounts = new List<int>();
    public List<int> BalloonHitCounts { get { return balloonHitCounts; } }

    [Header("Single-Mode Slots")]
    [SerializeField] private BalloonSlot[] balloonSlots;

    [Header("Multi-Mode Slots")]
    [SerializeField] private NumberSlot[] numberSlots;
    [SerializeField] private OperatorSlot[] operatorSlots;

    [Header("Balloon UI Prefabs")]
    [SerializeField] private GameObject balloonHitPrefab;
    [SerializeField] private Transform balloonHitParent;

    [Header("Multi-Mode UI Prefabs")]
    [SerializeField] private Transform numberBalloonParent;
    [SerializeField] private GameObject operatorBalloonPrefab;
    [SerializeField] private Transform operatorBalloonParent;

    [Header("Spawning Area")]
    public Vector2 areaSize = new Vector2(10f, 5f);

    [Header("Wall Control Setting")]
    public float rotationSpeed = 50f;

    [Header("Component References")]
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
        CalculateTargetSum(uiManager.TargetText, uiManager.ObjectiveText);
        UpdateRoundDisplay();
        currentBalloonIndex = 0;
    }

    private void ResetGameState()
    {
        DestroyChildren(balloonParent);

        if (!GameData.IsSingleMode())
        {
            DestroyChildren(numberBalloonParent);
            DestroyChildren(operatorBalloonParent);
            ClearNumberSlots();
            ClearOperatorSlots();
        }
        else
        {
            DestroyChildren(balloonHitParent);
            ClearBalloonSlots();
        }

        if (totalTurns % 2 != 0) balloonList.Clear();
        balloonHitCounts.Clear();
        ResetCounters();

        totalTurns--;
        hasSpawned = false;
        ResetUI();
    }

    private void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    public IEnumerator DestroyBalloonsAfterDelay(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        foreach (Transform child in balloonParent)
            Destroy(child.gameObject);
    }

    private void ClearBalloonSlots()
    {
        foreach (BalloonSlot slot in balloonSlots)
        {
            for (int i = slot.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.transform.GetChild(i).gameObject);
            }
        }
    }

    private void ClearNumberSlots()
    {
        foreach (NumberSlot slot in numberSlots)
        {
            for (int i = slot.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.transform.GetChild(i).gameObject);
            }
        }
    }

    private void ClearOperatorSlots()
    {
        foreach (OperatorSlot slot in operatorSlots)
        {
            slot.ClearOperator();
        }
    }

    private bool HasAnswer(float value)
    {
        return value != 0f || PlayerHasBalloon();
    }

    private bool PlayerHasBalloon()
    {
        if (GameData.IsSingleMode())
            return HasNumberSlotBalloon();
        else
            return HasMultiSlotBalloon();
    }

    public bool HasNumberSlotBalloon()
    {
        foreach (var slot in balloonSlots)
        {
            if (slot.HasBalloon())
                return true;
        }
        return false;
    }

    public bool HasMultiSlotBalloon()
    {
        foreach (var slot in numberSlots)
        {
            if (slot.HasNumber())
                return true;
        }
        return false;
    }

    private void ResetCounters()
    {
        spawnedCount = 0;
        attempts = 0;
        currentBalloonIndex = 0;
    }

    private void ResetUI()
    {
        if (GameData.IsSingleMode())
        {
            uiManager.TotalText.text = "0";
        }
        else
        {
            uiManager.MultiTotalText.text = "0";
        }

        uiManager.RemainingTime = uiManager._ProgressBar.maxValue;
        uiManager._ProgressBar.value = uiManager.RemainingTime;
    }

    private void UpdateRoundDisplay()
    {
        int remainingRounds = Mathf.CeilToInt(totalTurns / 2f);
        int currentRound = maxRounds - remainingRounds + 1;

        uiManager.RoundText.text = $"{currentRound}/{maxRounds}";
    }

    public void SetPlayerValues(TextMeshProUGUI playerInputText, TextMeshProUGUI playerText)
    {
        if (totalTurns % 2 == 0)
        {
            p1 = float.Parse(playerInputText.text);
            p1HasAnswered = HasAnswer(p1);
            //Debug.Log($"[P1] Answer: {p1}, HasBalloon: {PlayerHasBalloon()}, p1HasAnswered: {p1HasAnswered}");
            uiManager.ResultP1Text.text = $"{p1.ToString()}";
            playerText.text = "P2";
        }
        else
        {
            p2 = float.Parse(playerInputText.text);
            p2HasAnswered = HasAnswer(p2);
            //Debug.Log($"[P2] Answer: {p2}, HasBalloon: {PlayerHasBalloon()}, p2HasAnswered: {p2HasAnswered}");
            //Debug.Log($"[CountScore] Calling with P1:{p1} ({p1HasAnswered}), P2:{p2} ({p2HasAnswered})");
            uiManager.ResultP2Text.text = $"{p2.ToString()}";
            playerText.text = "P1";
            uiManager.CountScore(p1, p2, p1HasAnswered, p2HasAnswered);

            p1HasAnswered = false;
            p2HasAnswered = false;

            UpdateRoundDisplay();
        }
    }

    public void SetTargetRounds(int count)
    {
        totalTurns = count * 2;
        maxRounds = count;
        UpdateRoundDisplay();
    }

    public void SetTargetBalloonCount(int count)
    {
        targetBalloonCount = Mathf.Clamp(count, 2, totalBalloons);
    }

    public void SetBalloonSpawnCount(int count)
    {
        totalBalloons = Mathf.Clamp(count, 10, 20);
        GenerateBalloon();
        CalculateTargetSum(uiManager.TargetText, uiManager.ObjectiveText);
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

    private void CalculateTargetSum(TextMeshProUGUI targetText, TextMeshProUGUI objectiveText)
    {
        if (GameData.IsSingleMode())
        {
            CalculateTargetSumSingleMode(targetText, objectiveText);
        }
        else
        {
            CalculateTargetSumMultiMode(targetText, objectiveText);
        }
    }

    private void CalculateTargetSumSingleMode(TextMeshProUGUI targetText, TextMeshProUGUI objectiveText)
    {
        bool hasDivide = false;
        float sum = int.Parse(balloonList[0]);
        string resultText = sum.ToString();
        var mode = GameData.GetSingleMode();

        for (int i = 1; i < targetBalloonCount; i++)
        {
            if (int.TryParse(balloonList[i], out int value))
            {
                switch (mode)
                {
                    case OperatorMode.Plus:
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
                        if (value != 0)
                        {
                            sum /= value;
                            resultText += " / " + value;
                            hasDivide = true;
                        }
                        break;
                }
            }
        }

        targetText.text = hasDivide ? sum.ToString("F2") : sum.ToString();
        objectiveText.text = hasDivide ? sum.ToString("F2") : sum.ToString();

#if UNITY_EDITOR
        if (mode == OperatorMode.Divide)
        {
            Debug.Log($"Expression: {resultText} = {sum:F2}");
        }
        else
        {
            Debug.Log($"Expression: {resultText} = {sum}");
        }
#endif
    }

    private void CalculateTargetSumMultiMode(TextMeshProUGUI targetText, TextMeshProUGUI objectiveText)
    {
        bool hasDivide = false;
        float sum = int.Parse(balloonList[0]);
        string resultText = sum.ToString();
        int modeIndex = 0;

        for (int i = 1; i < targetBalloonCount; i++)
        {
            if (int.TryParse(balloonList[i], out int value))
            {
                var mode = GameData.SelectedModes[modeIndex % GameData.SelectedModes.Count];
                modeIndex++;

                string displayValue = value < 0 ? $"({value})" : value.ToString();

                switch (mode)
                {
                    case OperatorMode.Plus:
                        sum += value;
                        resultText += " + " + displayValue;
                        break;

                    case OperatorMode.Minus:
                        sum -= value;
                        resultText += " - " + displayValue;
                        break;

                    case OperatorMode.Multiply:
                        sum *= value;
                        resultText += " * " + displayValue;
                        break;

                    case OperatorMode.Divide:
                        if (value != 0)
                        {
                            sum /= value;
                            resultText += " / " + displayValue;
                            hasDivide = true;
                        }
                        break;
                }
            }
        }

        targetText.text = hasDivide ? sum.ToString("F2") : sum.ToString();
        objectiveText.text = hasDivide ? sum.ToString("F2") : sum.ToString();
        Debug.Log($"Expression: {resultText} = {(hasDivide ? sum.ToString("F2") : sum.ToString())}");
    }

    public void CalculateBalloon()
    {
        if (GameData.IsSingleMode())
        {
            CalculateBalloonSingleMode();
        }
        else
        {
            CalculateBalloonMultiMode();
        }
    }

    private void CalculateBalloonSingleMode()
    {
        int sum = 0;
        var mode = GameData.GetSingleMode();

        for (int i = 0; i < balloonHitCounts.Count; i++)
        {
            switch (mode)
            {
                case OperatorMode.Plus:
                    sum += balloonHitCounts[i];
                    break;
                case OperatorMode.Minus:
                    sum -= balloonHitCounts[i];
                    break;
                case OperatorMode.Multiply:
                    sum *= balloonHitCounts[i];
                    break;
                case OperatorMode.Divide:
                    if (balloonHitCounts[i] != 0)
                        sum = (i == 0) ? balloonHitCounts[i] : sum / balloonHitCounts[i];
                    break;
            }
        }

        Debug.Log("Player Result: " + sum);
    }

    private void CalculateBalloonMultiMode()
    {
        float sum = 0;
        bool isFirst = true;
        int modeIndex = 0;

        for (int i = 0; i < balloonHitCounts.Count; i++)
        {
            if (isFirst)
            {
                sum = balloonHitCounts[i];
                isFirst = false;
                continue;
            }

            var mode = GameData.SelectedModes[modeIndex % GameData.SelectedModes.Count];
            modeIndex++;

            switch (mode)
            {
                case OperatorMode.Plus:
                    sum += balloonHitCounts[i];
                    break;
                case OperatorMode.Minus:
                    sum -= balloonHitCounts[i];
                    break;
                case OperatorMode.Multiply:
                    sum *= balloonHitCounts[i];
                    break;
                case OperatorMode.Divide:
                    if (balloonHitCounts[i] != 0)
                        sum /= balloonHitCounts[i];
                    break;
            }
        }

        Debug.Log("Player Result: " + sum);
    }

    public void SpawnBalloonHitTexts()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        Debug.Log($"Spawning balloons. IsSingleMode: {GameData.IsSingleMode()}, Selected Modes: {string.Join(", ", GameData.SelectedModes)}");

        if (GameData.IsSingleMode())
        {
            for (int i = 0; i < balloonHitCounts.Count; i++)
            {
                GameObject balloon = Instantiate(balloonHitPrefab, transform.position, Quaternion.identity, balloonHitParent);
                BalloonDragHandler dragHandler = balloon.GetComponent<BalloonDragHandler>();

                if (dragHandler != null)
                {
                    dragHandler.originalSlot = balloonHitParent;
                }
            }

            StartCoroutine(InitializeSlotsAfterLayout());
        }
        else
        {
            for (int i = 0; i < balloonHitCounts.Count; i++)
            {
                GameObject balloon = Instantiate(balloonHitPrefab, transform.position, Quaternion.identity, numberBalloonParent);
                BalloonDragHandler dragHandler = balloon.GetComponent<BalloonDragHandler>();

                if (dragHandler != null)
                {
                    dragHandler.originalSlot = numberBalloonParent;
                }
            }

            StartCoroutine(InitializeSlotsAfterLayout());
        }

        if (!GameData.IsSingleMode())
        {
            SpawnOperatorBalloons();
        }
    }

    private IEnumerator InitializeSlotsAfterLayout()
    {
        yield return new WaitForEndOfFrame();

        foreach (var slot in numberSlots)
        {
            slot.InitializeBalloon();
        }
    }

    private int GetOperatorPriority(OperatorMode mode)
    {
        switch (mode)
        {
            case OperatorMode.Plus:
                return 0;
            case OperatorMode.Minus:
                return 1;
            case OperatorMode.Multiply:
                return 2;
            case OperatorMode.Divide:
                return 3;
            default:
                return -1;
        }
    }

    private void SpawnOperatorBalloons()
    {
        List<OperatorMode> orderedModes = new List<OperatorMode>(GameData.SelectedModes);
        orderedModes.Sort((a, b) => GetOperatorPriority(a).CompareTo(GetOperatorPriority(b)));

        foreach (var mode in orderedModes)
        {
            Debug.Log($"Creating operator balloon for mode: {mode}");

            GameObject operatorObj = Instantiate(operatorBalloonPrefab, Vector3.zero, Quaternion.identity, operatorBalloonParent);
            OperatorBalloon operatorBalloon = operatorObj.GetComponent<OperatorBalloon>();
            operatorBalloon.Initialize(mode);
        }
    }

    public void RespawnOperatorBalloon(OperatorMode mode, int insertIndex = -1)
    {
        GameObject operatorObj = Instantiate(operatorBalloonPrefab, Vector3.zero, Quaternion.identity, operatorBalloonParent);
        OperatorBalloon operatorBalloon = operatorObj.GetComponent<OperatorBalloon>();
        operatorBalloon.Initialize(mode);

        if (insertIndex >= 0 && insertIndex < operatorBalloonParent.childCount)
        {
            operatorObj.transform.SetSiblingIndex(insertIndex);
        }
    }

    public void UpdateBalloonSum(TextMeshProUGUI totalText)
    {
        if (GameData.IsSingleMode())
        {
            UpdateBalloonSumSingleMode(totalText);
        }
        else
        {
            UpdateBalloonSumMultiMode(totalText);
        }
    }

    private void UpdateBalloonSumSingleMode(TextMeshProUGUI totalText)
    {
        int sum = 0;
        var mode = GameData.GetSingleMode();

        switch (mode)
        {
            case OperatorMode.Plus:
                foreach (var slot in balloonSlots)
                {
                    if (slot.transform.childCount > 0)
                    {
                        BalloonHitText balloon = slot.transform.GetChild(0).GetComponent<BalloonHitText>();

                        if (balloon != null)
                        {
                            sum += balloon.Value;
                        }
                    }
                }
                totalText.text = sum.ToString();
                break;

            case OperatorMode.Minus:
                for (int i = 0; i < balloonSlots.Length; i++)
                {
                    if (balloonSlots[i].transform.childCount > 0)
                    {
                        BalloonHitText balloon = balloonSlots[i].transform.GetChild(0).GetComponent<BalloonHitText>();

                        if (balloon != null)
                        {
                            int value = balloon.Value;
                            sum = (sum == 0) ? value : sum - value;
                        }
                    }
                }
                totalText.text = sum.ToString();
                break;

            case OperatorMode.Multiply:
                {
                    int sumMultiply = 1;
                    bool hasBalloon = false;

                    foreach (var slot in balloonSlots)
                    {
                        if (slot.transform.childCount > 0)
                        {
                            BalloonHitText balloon = slot.transform.GetChild(0).GetComponent<BalloonHitText>();

                            if (balloon != null)
                            {
                                sumMultiply *= balloon.Value;
                                hasBalloon = true;
                            }
                        }
                    }
                    sum = hasBalloon ? sumMultiply : 0;
                    totalText.text = sum.ToString();
                    break;
                }

            case OperatorMode.Divide:
                {
                    float sumDivide = 0;
                    bool firstFound = false;

                    foreach (var slot in balloonSlots)
                    {
                        if (slot.transform.childCount > 0)
                        {
                            BalloonHitText balloon = slot.transform.GetChild(0).GetComponent<BalloonHitText>();

                            if (balloon != null)
                            {
                                int value = balloon.Value;
                                sumDivide = firstFound ? sumDivide / value : value;
                                firstFound = true;
                            }
                        }
                    }
                    totalText.text = firstFound ? sumDivide.ToString("F2") : "0.00";
                    break;
                }
        }
    }

    private void UpdateBalloonSumMultiMode(TextMeshProUGUI totalText)
    {
        float sum = 0;
        bool hasValue = false;
        bool hasDivide = GameData.HasMode(OperatorMode.Divide);

        List<object> sequence = new List<object>();

        for (int i = 0; i < numberSlots.Length; i++)
        {
            if (numberSlots[i].HasNumber())
            {
                int value = numberSlots[i].GetBalloonValue();

                sequence.Add((float)value);
                Debug.Log($"NumberSlot {i}: Number = {value}");
            }

            if (operatorSlots != null && i < operatorSlots.Length)
            {
                if (operatorSlots[i].HasOperator())
                {
                    var op = operatorSlots[i].GetOperatorMode();

                    if (op.HasValue)
                    {
                        sequence.Add(op.Value);
                        Debug.Log($"OperatorSlot {i}: Operator = {op.Value}");
                    }
                }
            }
        }

        if (sequence.Count == 0)
        {
            totalText.text = "0";
            return;
        }

        for (int i = 1; i < sequence.Count - 1; i += 2)
        {
            if (sequence[i] is OperatorMode op && sequence[i - 1] is float left && sequence[i + 1] is float right)
            {
                if (op == OperatorMode.Multiply || op == OperatorMode.Divide)
                {
                    if (op == OperatorMode.Divide && Mathf.Approximately(right, 0f))
                    {
                        Debug.LogWarning($"Division by zero at index {i} — skipping this operation.");

                        sequence.RemoveAt(i + 1);
                        sequence.RemoveAt(i);
                        i -= 2;
                        continue;
                    }

                    float result = (op == OperatorMode.Multiply) ? left * right : left / right;
                    Debug.Log($"Evaluated (priority) {left} {op} {right} = {result}");

                    sequence[i - 1] = result;
                    sequence.RemoveAt(i + 1);
                    sequence.RemoveAt(i);

                    i -= 2;
                }
            }
            else
            {
                Debug.LogWarning($"Invalid sequence pattern at index {i}. Stopping priority pass.");
                break;
            }
        }

        if (sequence[0] is float firstNumber)
        {
            sum = firstNumber;
            hasValue = true;
            Debug.Log($"Starting with: {sum}");

            int i = 1;
            while (i < sequence.Count)
            {
                if (i < sequence.Count - 1 && sequence[i] is OperatorMode op && sequence[i + 1] is float nextNumber)
                {
                    float oldSum = sum;
                    switch (op)
                    {
                        case OperatorMode.Plus:
                            sum += nextNumber;
                            break;
                        case OperatorMode.Minus:
                            sum -= nextNumber;
                            break;
                        case OperatorMode.Multiply:
                            sum *= nextNumber;
                            break;
                        case OperatorMode.Divide:
                            if (!Mathf.Approximately(nextNumber, 0f))
                            {
                                sum /= nextNumber;
                            }
                            else
                            {
                                Debug.LogWarning("Division by zero in fallback step!");
                            }
                            break;
                    }

                    i += 2;
                }
                else if (sequence[i] is float lonelyNumber)
                {
                    Debug.LogWarning($"Missing operator before number {lonelyNumber}!");
                    i++;
                }
                else
                {
                    Debug.LogWarning($"Invalid sequence at index {i}");
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("First element must be a number!");
        }

        Debug.Log($"Final sum: {sum}");
        totalText.text = hasValue ? (hasDivide ? sum.ToString("F2") : sum.ToString()) : "0";
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector2.zero, areaSize);
    }
#endif
}