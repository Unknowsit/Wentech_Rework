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

    [Header("Step Display")]
    [SerializeField] private SimpleStepDisplay simpleStepDisplay;

    [Header("Spawning Area")]
    public Vector2 areaSize = new Vector2(10f, 5f);

    [Header("Cannon Switch Settings")]
    [SerializeField] private Transform cannonTransform;
    [SerializeField] private Vector3 leftCannonPosition = new Vector3(-6.965f, -3.33f, 0f);
    [SerializeField] private Vector3 rightCannonPosition = new Vector3(6.965f, -3.33f, 0f);
    [SerializeField] private bool isCannonOnLeft = true;

    [Header("Fortress Sprite")]
    [SerializeField] private SpriteRenderer fortressSprite;
    [SerializeField] private Sprite leftCannonSprite;
    [SerializeField] private Sprite rightCannonSprite;

    [Header("Base Sprite")]
    [SerializeField] private SpriteRenderer baseSprite;
    [SerializeField] private Sprite leftBaseSprite;
    [SerializeField] private Sprite rightBaseSprite;

    [Header("Wall Control Setting")]
    public float rotationSpeed = 50f;

#if UNITY_ANDROID || UNITY_EDITOR
    [Header("Android ShootButton")]
    [SerializeField] private RectTransform shootButtonRect;
    [SerializeField] private float shootButtonLeftX = -1554f;
    [SerializeField] private float shootButtonRightX = 1554f;
#endif

    [Header("Component References")]
    public CannonAim cannonAim;
    public CannonShooter cannonShooter;
    public Timer timer;
    public WallInventory wallInventory;

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

        RefreshAllParenthesisUI();
        wallInventory.ResetWalls();
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
            if (slot == null) continue;

            for (int i = slot.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = slot.transform.GetChild(i);

                if (child.GetComponent<BalloonHitText>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            Parenthesis parenthesis = slot.GetComponent<Parenthesis>();

            if (parenthesis != null)
            {
                parenthesis.ResetType();
            }
        }

        /*
        foreach (NumberSlot slot in numberSlots)
        {
            for (int i = slot.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.transform.GetChild(i).gameObject);
            }
        }
        */
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

    public NumberSlot[] GetNumberSlots()
    {
        return numberSlots;
    }

    public OperatorSlot[] GetOperatorSlots()
    {
        return operatorSlots;
    }

    public void UpdateStepDisplay()
    {
        if (simpleStepDisplay != null && !GameData.IsSingleMode())
        {
            simpleStepDisplay.UpdateDisplay();
        }
    }

    private void ResetCounters()
    {
        spawnedCount = 0;
        attempts = 0;
        currentBalloonIndex = 0;
    }

    public void RefreshAllParenthesisUI()
    {
        if (!GameData.ShouldUseParentheses() || numberSlots == null)
        {
            return;
        }

        foreach (var slot in numberSlots)
        {
            if (slot == null) continue;

            Parenthesis parenthesis = slot.GetComponent<Parenthesis>();

            if (parenthesis != null && parenthesis.enabled)
            {
                parenthesis.RefreshUI();
            }
        }
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
            //uiManager.ResultP1Text.text = $"{p1.ToString()}";
            uiManager.ResultP1Text.text = NumberFormatter.FormatSmart(p1);
            playerText.text = "P2";
        }
        else
        {
            p2 = float.Parse(playerInputText.text);
            p2HasAnswered = HasAnswer(p2);
            //Debug.Log($"[P2] Answer: {p2}, HasBalloon: {PlayerHasBalloon()}, p2HasAnswered: {p2HasAnswered}");
            //Debug.Log($"[CountScore] Calling with P1:{p1} ({p1HasAnswered}), P2:{p2} ({p2HasAnswered})");
            //uiManager.ResultP2Text.text = $"{p2.ToString()}";
            uiManager.ResultP2Text.text = NumberFormatter.FormatSmart(p2);
            playerText.text = "P1";
            uiManager.CountScore(p1, p2, p1HasAnswered, p2HasAnswered);

            p1HasAnswered = false;
            p2HasAnswered = false;

            UpdateRoundDisplay();
        }
    }

    public void SwitchCannonSide()
    {
        isCannonOnLeft = !isCannonOnLeft;

        if (isCannonOnLeft)
        {
            cannonTransform.position = leftCannonPosition;
            cannonTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            fortressSprite.sprite = leftCannonSprite;
            baseSprite.sprite = leftBaseSprite;

#if UNITY_ANDROID
            shootButtonRect.anchoredPosition = new Vector2(shootButtonLeftX, shootButtonRect.anchoredPosition.y);
#endif
        }
        else
        {
            cannonTransform.position = rightCannonPosition;
            cannonTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
            fortressSprite.sprite = rightCannonSprite;
            baseSprite.sprite = rightBaseSprite;

#if UNITY_ANDROID
            shootButtonRect.anchoredPosition = new Vector2(shootButtonRightX, shootButtonRect.anchoredPosition.y);
#endif
        }

        cannonAim.transform.localEulerAngles = new Vector3(0f, 0f, 25f);
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

        //targetText.text = hasDivide ? sum.ToString("F2") : sum.ToString();
        //objectiveText.text = hasDivide ? sum.ToString("F2") : sum.ToString();

        targetText.text = hasDivide ? NumberFormatter.FormatWithCommas(sum, 2) : NumberFormatter.FormatWithCommas((int)sum);
        objectiveText.text = hasDivide ? NumberFormatter.FormatWithCommas(sum, 2) : NumberFormatter.FormatWithCommas((int)sum);

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
        string resultText = "";
        int modeIndex = 0;

        List<object> sequence = new List<object>();

        if (int.TryParse(balloonList[0], out int firstValue))
        {
            sequence.Add((float)firstValue);
            resultText = firstValue.ToString();
        }

        for (int i = 1; i < targetBalloonCount; i++)
        {
            if (int.TryParse(balloonList[i], out int value))
            {
                var mode = GameData.SelectedModes[modeIndex % GameData.SelectedModes.Count];
                modeIndex++;

                sequence.Add(mode);
                sequence.Add((float)value);

                string displayValue = value < 0 ? $"({value})" : value.ToString();
                string operatorSymbol = mode == OperatorMode.Plus ? "+" : mode == OperatorMode.Minus ? "-" : mode == OperatorMode.Multiply ? "*" : "/";

                resultText += $" {operatorSymbol} {displayValue}";

                if (mode == OperatorMode.Divide)
                {
                    hasDivide = true;
                }
            }
        }

        float sum = EvaluateSequence(sequence);

        //targetText.text = hasDivide ? sum.ToString("F2") : sum.ToString();
        //objectiveText.text = hasDivide ? sum.ToString("F2") : sum.ToString();

        targetText.text = hasDivide ? NumberFormatter.FormatWithCommas(sum, 2) : NumberFormatter.FormatWithCommas((int)sum);
        objectiveText.text = hasDivide ? NumberFormatter.FormatWithCommas(sum, 2) : NumberFormatter.FormatWithCommas((int)sum);

        Debug.Log($"Expression: {resultText} = {(hasDivide ? sum.ToString("F2") : sum.ToString())}");
    }

    private float EvaluateSequence(List<object> sequence)
    {
        if (sequence.Count == 0) return 0;

        List<object> seq = new List<object>(sequence);

        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorMode op && seq[i - 1] is float left && seq[i + 1] is float right)
            {
                if (op == OperatorMode.Multiply || op == OperatorMode.Divide)
                {
                    float result = 0;

                    if (op == OperatorMode.Multiply)
                    {
                        result = left * right;
                    }
                    else if (op == OperatorMode.Divide)
                    {
                        if (!Mathf.Approximately(right, 0f))
                        {
                            result = left / right;
                        }
                        else
                        {
                            Debug.LogWarning("Division by zero in target calculation!");
                            result = 0;
                        }
                    }

                    seq[i - 1] = result;
                    seq.RemoveAt(i + 1);
                    seq.RemoveAt(i);
                    i -= 2;
                }
            }
        }

        if (seq.Count == 0 || !(seq[0] is float))
            return 0;

        float sum = (float)seq[0];

        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorMode op && seq[i + 1] is float nextNum)
            {
                switch (op)
                {
                    case OperatorMode.Plus:
                        sum += nextNum;
                        break;
                    case OperatorMode.Minus:
                        sum -= nextNum;
                        break;
                }
            }
        }

        return sum;
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
            if (GameData.ShouldUseParentheses())
            {
                UpdateBalloonSumMultiModeWithParentheses(totalText);
            }
            else
            {
                UpdateBalloonSumMultiMode(totalText);
            }
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
                //totalText.text = sum.ToString();
                totalText.text = NumberFormatter.FormatWithCommas(sum);
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
                //totalText.text = sum.ToString();
                totalText.text = NumberFormatter.FormatWithCommas(sum);
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
                    //totalText.text = sum.ToString();
                    totalText.text = NumberFormatter.FormatWithCommas(sum);
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
                    //totalText.text = firstFound ? sumDivide.ToString("F2") : "0.00";
                    totalText.text = firstFound ? NumberFormatter.FormatWithCommas(sumDivide, 2) : "0.00";
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

        if (!IsExpressionValid(sequence))
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
        //totalText.text = hasValue ? (hasDivide ? sum.ToString("F2") : sum.ToString()) : "0";
        totalText.text = hasValue ? (hasDivide ? NumberFormatter.FormatWithCommas(sum, 2) : NumberFormatter.FormatWithCommas((int)sum)) : "0";
    }

    private void UpdateBalloonSumMultiModeWithParentheses(TextMeshProUGUI totalText)
    {
        if (numberSlots == null)
        {
            totalText.text = "0";
            return;
        }

        bool hasDivide = GameData.HasMode(OperatorMode.Divide);
        List<object> sequence = BuildSequenceWithParentheses();

        if (sequence.Count == 0)
        {
            totalText.text = "0";
            return;
        }

        if (!IsParenthesesBalanced(sequence))
        {
            totalText.text = "0";
            return;
        }

        float sum = EvaluateExpressionWithParentheses(sequence);
        //totalText.text = hasDivide ? sum.ToString("F2") : sum.ToString();
        totalText.text = hasDivide ? NumberFormatter.FormatWithCommas(sum, 2) : NumberFormatter.FormatWithCommas((int)sum);
    }

    public bool IsExpressionValid(List<object> sequence)
    {
        if (sequence.Count == 0)
            return false;

        if (sequence[0] is OperatorMode)
            return false;

        if (sequence[sequence.Count - 1] is OperatorMode)
            return false;

        for (int i = 0; i < sequence.Count - 1; i++)
        {
            object a = sequence[i];
            object b = sequence[i + 1];

            if (a is float && b is float)
                return false;

            if (a is OperatorMode && b is OperatorMode)
                return false;

            if (a is OperatorMode && !(b is float))
                return false;

            if (b is OperatorMode && !(a is float))
                return false;
        }

        return true;
    }

    public bool IsParenthesesBalanced(List<object> sequence)
    {
        int openCount = 0;

        foreach (var item in sequence)
        {
            if (item is ParenthesisType type)
            {
                if (type == ParenthesisType.Open)
                {
                    openCount++;
                }
                else if (type == ParenthesisType.Close)
                {
                    openCount--;

                    if (openCount < 0)
                    {
                        //Debug.LogWarning("Found closing parenthesis without matching opening parenthesis");
                        return false;
                    }
                }
            }
        }

        if (openCount != 0)
        {
            //Debug.LogWarning($"Unbalanced parentheses: {openCount} unclosed opening parenthesis");
            return false;
        }

        return true;
    }

    public void ValidateParentheses()
    {
        if (!GameData.ShouldUseParentheses() || numberSlots == null) return;

        foreach (var slot in numberSlots)
        {
            if (slot == null) continue;

            Parenthesis parenthesis = slot.GetComponent<Parenthesis>();

            if (parenthesis != null)
            {
                parenthesis.SetErrorState(false);
            }
        }

        Stack<int> openStack = new Stack<int>();
        List<int> errorIndices = new List<int>();

        for (int i = 0; i < numberSlots.Length; i++)
        {
            if (numberSlots[i] == null) continue;

            Parenthesis parenthesis = numberSlots[i].GetComponent<Parenthesis>();

            if (parenthesis == null) continue;

            var type = parenthesis.CurrentType;

            if (type == ParenthesisType.Open)
            {
                openStack.Push(i);
            }
            else if (type == ParenthesisType.DoubleOpen)
            {
                openStack.Push(i);
                openStack.Push(i);
            }
            else if (type == ParenthesisType.Close)
            {
                if (openStack.Count > 0)
                {
                    openStack.Pop();
                }
                else
                {
                    if (!errorIndices.Contains(i))
                    {
                        errorIndices.Add(i);
                    }
                }
            }
            else if (type == ParenthesisType.DoubleClose)
            {
                if (openStack.Count >= 2)
                {
                    openStack.Pop();
                    openStack.Pop();
                }
                else if (openStack.Count == 1)
                {
                    openStack.Pop();

                    if (!errorIndices.Contains(i))
                    {
                        errorIndices.Add(i);
                    }
                }
                else
                {
                    if (!errorIndices.Contains(i))
                    {
                        errorIndices.Add(i);
                    }
                }
            }
        }

        while (openStack.Count > 0)
        {
            int unclosedIndex = openStack.Pop();

            if (!errorIndices.Contains(unclosedIndex))
            {
                errorIndices.Add(unclosedIndex);
            }
        }

        foreach (int errorIndex in errorIndices)
        {
            if (errorIndex >= 0 && errorIndex < numberSlots.Length && numberSlots[errorIndex] != null)
            {
                Parenthesis parenthesis = numberSlots[errorIndex].GetComponent<Parenthesis>();
                if (parenthesis != null)
                {
                    parenthesis.SetErrorState(true);
                }
            }
        }
    }

    private List<object> BuildSequenceWithParentheses()
    {
        List<object> sequence = new List<object>();

        for (int i = 0; i < numberSlots.Length; i++)
        {
            if (numberSlots[i] == null) continue;

            Parenthesis parenthesis = numberSlots[i].GetComponent<Parenthesis>();

            if (parenthesis != null && parenthesis.enabled)
            {
                if (parenthesis.CurrentType == ParenthesisType.Open)
                {
                    sequence.Add(ParenthesisType.Open);
                }
                else if (parenthesis.CurrentType == ParenthesisType.DoubleOpen)
                {
                    sequence.Add(ParenthesisType.Open);
                    sequence.Add(ParenthesisType.Open);
                }
            }

            if (numberSlots[i].HasNumber())
            {
                int value = numberSlots[i].GetBalloonValue();
                sequence.Add((float)value);
            }

            if (parenthesis != null && parenthesis.enabled)
            {
                if (parenthesis.CurrentType == ParenthesisType.Close)
                {
                    sequence.Add(ParenthesisType.Close);
                }
                else if (parenthesis.CurrentType == ParenthesisType.DoubleClose)
                {
                    sequence.Add(ParenthesisType.Close);
                    sequence.Add(ParenthesisType.Close);
                }
            }

            if (operatorSlots != null && i < operatorSlots.Length)
            {
                if (operatorSlots[i] != null && operatorSlots[i].HasOperator())
                {
                    var op = operatorSlots[i].GetOperatorMode();

                    if (op.HasValue)
                    {
                        sequence.Add(op.Value);
                    }
                }
            }
        }

        int idx = 0;

        while (idx < sequence.Count)
        {
            if (idx + 1 < sequence.Count)
            {
                object current = sequence[idx];
                object next = sequence[idx + 1];

                bool shouldRemoveCurrent = false;
                bool shouldRemoveNext = false;
                bool shouldRemoveNextGroup = false;
                int groupEndIndex = -1;

                if (current is float && next is float)
                {
                    shouldRemoveNext = true;
                    //Debug.Log($"Pattern: Number-Number at [{idx}]-[{idx + 1}]  Remove [{idx + 1}]");
                }
                else if (current is ParenthesisType cType && cType == ParenthesisType.Close && next is float)
                {
                    shouldRemoveNext = true;
                    //Debug.Log($"Pattern: )-Number at [{idx}]-[{idx + 1}]  Remove [{idx + 1}]");
                }
                else if (current is float && next is ParenthesisType nType && nType == ParenthesisType.Open)
                {
                    shouldRemoveCurrent = true;
                    //Debug.Log($"Pattern: Number-( at [{idx}]-[{idx + 1}]  Remove [{idx}]");
                }
                else if (current is ParenthesisType cType2 && cType2 == ParenthesisType.Close && next is ParenthesisType nType2 && nType2 == ParenthesisType.Open)
                {
                    groupEndIndex = FindMatchingCloseParen(sequence, idx + 1);

                    if (groupEndIndex > idx + 1)
                    {
                        shouldRemoveNextGroup = true;
                        //Debug.Log($"Pattern: )-( at [{idx}]-[{idx + 1}]  Remove [{idx + 1}] to [{groupEndIndex}]");
                    }
                }

                if (shouldRemoveCurrent)
                {
                    sequence.RemoveAt(idx);
                }
                else if (shouldRemoveNextGroup && groupEndIndex > idx)
                {
                    int removeCount = groupEndIndex - idx;
                    sequence.RemoveRange(idx + 1, removeCount);
                }
                else if (shouldRemoveNext)
                {
                    sequence.RemoveAt(idx + 1);
                }
                else
                {
                    idx++;
                }
            }
            else
            {
                idx++;
            }
        }

        while (sequence.Count > 0 && sequence[sequence.Count - 1] is OperatorMode)
        {
            //Debug.Log($"Removing trailing operator");
            sequence.RemoveAt(sequence.Count - 1);
        }

        return sequence;
    }

    private float EvaluateExpressionWithParentheses(List<object> sequence)
    {
        while (HasParenthesisInSequence(sequence))
        {
            int openIdx = FindInnermostOpenParen(sequence);
            int closeIdx = FindMatchingCloseParen(sequence, openIdx);

            if (closeIdx == -1) break;

            List<object> subExpr = sequence.GetRange(openIdx + 1, closeIdx - openIdx - 1);
            float subResult = EvaluateSimpleExpression(subExpr);

            sequence.RemoveRange(openIdx, closeIdx - openIdx + 1);
            sequence.Insert(openIdx, subResult);
        }

        return EvaluateSimpleExpression(sequence);
    }

    private bool HasParenthesisInSequence(List<object> sequence)
    {
        foreach (var item in sequence)
        {
            if (item is ParenthesisType)
                return true;
        }
        return false;
    }

    private int FindInnermostOpenParen(List<object> sequence)
    {
        int lastOpen = -1;

        for (int i = 0; i < sequence.Count; i++)
        {
            if (sequence[i] is ParenthesisType type && type == ParenthesisType.Open)
            {
                lastOpen = i;
            }
            else if (sequence[i] is ParenthesisType closeType && closeType == ParenthesisType.Close)
            {
                return lastOpen;
            }
        }

        return lastOpen;
    }

    private int FindMatchingCloseParen(List<object> sequence, int openIndex)
    {
        if (openIndex < 0) return -1;

        int depth = 0;

        for (int i = openIndex; i < sequence.Count; i++)
        {
            if (sequence[i] is ParenthesisType type)
            {
                if (type == ParenthesisType.Open)
                    depth++;
                else if (type == ParenthesisType.Close)
                {
                    depth--;
                    if (depth == 0)
                        return i;
                }
            }
        }

        return -1;
    }

    private float EvaluateSimpleExpression(List<object> sequence)
    {
        if (sequence.Count == 0) return 0;

        List<object> seq = new List<object>(sequence);

        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorMode op && seq[i - 1] is float left && seq[i + 1] is float right)
            {
                if (op == OperatorMode.Multiply || op == OperatorMode.Divide)
                {
                    if (op == OperatorMode.Divide && Mathf.Approximately(right, 0f))
                    {
                        seq.RemoveAt(i + 1);
                        seq.RemoveAt(i);
                        i -= 2;
                        continue;
                    }

                    float result = (op == OperatorMode.Multiply) ? left * right : left / right;

                    seq[i - 1] = result;
                    seq.RemoveAt(i + 1);
                    seq.RemoveAt(i);
                    i -= 2;
                }
            }
        }

        if (seq.Count == 0) return 0;
        if (!(seq[0] is float)) return 0;

        float sum = (float)seq[0];

        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorMode op && seq[i + 1] is float nextNum)
            {
                switch (op)
                {
                    case OperatorMode.Plus:
                        sum += nextNum;
                        break;
                    case OperatorMode.Minus:
                        sum -= nextNum;
                        break;
                }
            }
        }

        return sum;
    }

#if UNITY_EDITOR
    /*
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
    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector2.zero, areaSize);
    }
#endif
}