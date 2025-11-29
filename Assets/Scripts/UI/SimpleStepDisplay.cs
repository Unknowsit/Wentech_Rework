using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleStepDisplay : MonoBehaviour
{
    [System.Serializable]
    public class StepTextSlot
    {
        public OperatorSlot operatorSlot;
        public TextMeshProUGUI resultText;
        public int priority;
    }

    [Header("Step Text Slots")]
    [SerializeField] private StepTextSlot[] stepTextSlots;

    [Header("Text Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color innermostParenthesisColor = new Color(1f, 0.4f, 0.8f, 1f);
    [SerializeField] private Color outerParenthesisColor = new Color(1f, 0.8f, 0f, 1f);
    [SerializeField] private Color multiplyDivideColor = new Color(1f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color plusMinusColor = new Color(0.3f, 1f, 0.3f, 1f);

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
        ResetAllTexts();
    }

    public void UpdateDisplay()
    {
        var numberSlots = gameManager.GetNumberSlots();
        var operatorSlots = gameManager.GetOperatorSlots();

        if (numberSlots == null || operatorSlots == null)
        {
            ResetAllTexts();
            return;
        }

        List<object> sequence = BuildSequence(numberSlots, operatorSlots);

        if (sequence.Count == 0)
        {
            ResetAllTexts();
            return;
        }

        if (!IsSequenceComplete(sequence))
        {
            ResetAllTexts();
            return;
        }

        List<object> floatSequence = ConvertToFloatSequence(sequence);

        if (!gameManager.IsParenthesesBalanced(floatSequence))
        {
            ResetAllTexts();
            return;
        }

        CalculateAndUpdateTexts(sequence);
    }

    private void ResetAllTexts()
    {
        foreach (var slot in stepTextSlots)
        {
            slot.resultText.text = "0";
            slot.resultText.color = defaultColor;
            slot.priority = 0;
        }
    }

    private List<object> BuildSequence(NumberSlot[] numberSlots, OperatorSlot[] operatorSlots)
    {
        List<object> sequence = new List<object>();

        for (int i = 0; i < numberSlots.Length; i++)
        {
            if (numberSlots[i] == null) continue;

            if (GameData.ShouldUseParentheses())
            {
                Parenthesis p = numberSlots[i].GetComponent<Parenthesis>();

                if (p != null && p.enabled)
                {
                    if (p.CurrentType == ParenthesisType.Open)
                        sequence.Add(ParenthesisType.Open);
                    else if (p.CurrentType == ParenthesisType.DoubleOpen)
                    {
                        sequence.Add(ParenthesisType.Open);
                        sequence.Add(ParenthesisType.Open);
                    }
                }
            }

            if (numberSlots[i].HasNumber())
            {
                sequence.Add(new NumberWithSlot
                {
                    value = numberSlots[i].GetBalloonValue(),
                    slotIndex = i
                });
            }

            if (GameData.ShouldUseParentheses())
            {
                Parenthesis p = numberSlots[i].GetComponent<Parenthesis>();

                if (p != null && p.enabled)
                {
                    if (p.CurrentType == ParenthesisType.Close)
                        sequence.Add(ParenthesisType.Close);
                    else if (p.CurrentType == ParenthesisType.DoubleClose)
                    {
                        sequence.Add(ParenthesisType.Close);
                        sequence.Add(ParenthesisType.Close);
                    }
                }
            }

            if (i < operatorSlots.Length && operatorSlots[i] != null && operatorSlots[i].HasOperator())
            {
                var op = operatorSlots[i].GetOperatorMode();

                if (op.HasValue)
                {
                    sequence.Add(new OperatorWithSlot
                    {
                        operatorMode = op.Value,
                        slotIndex = i
                    });
                }
            }
        }

        return sequence;
    }

    private bool IsSequenceComplete(List<object> sequence)
    {
        List<object> withoutParens = new List<object>();

        foreach (var item in sequence)
        {
            if (!(item is ParenthesisType))
            {
                withoutParens.Add(item);
            }
        }

        if (withoutParens.Count == 0)
            return false;

        if (withoutParens.Count == 1)
            return withoutParens[0] is NumberWithSlot;

        for (int i = 0; i < withoutParens.Count; i++)
        {
            if (i % 2 == 0)
            {
                if (!(withoutParens[i] is NumberWithSlot))
                    return false;
            }
            else
            {
                if (!(withoutParens[i] is OperatorWithSlot))
                    return false;
            }
        }

        return withoutParens[withoutParens.Count - 1] is NumberWithSlot;
    }

    private List<object> ConvertToFloatSequence(List<object> sequence)
    {
        List<object> result = new List<object>();

        foreach (var item in sequence)
        {
            if (item is NumberWithSlot num)
            {
                result.Add((float)num.value);
            }
            else if (item is OperatorWithSlot op)
            {
                result.Add(op.operatorMode);
            }
            else
            {
                result.Add(item);
            }
        }

        return result;
    }

    private void CalculateAndUpdateTexts(List<object> sequence)
    {
        foreach (var slot in stepTextSlots)
        {
            slot.priority = 0;
        }

        List<object> working = new List<object>(sequence);
        int maxDepth = GetMaxParenthesisDepth(working);

        while (HasParenthesis(working))
        {
            int openIdx = FindInnermostOpenParen(working);
            int closeIdx = FindMatchingCloseParen(working, openIdx);

            if (closeIdx == -1) break;

            List<object> subExpr = working.GetRange(openIdx + 1, closeIdx - openIdx - 1);
            int currentDepth = GetCurrentDepth(working, openIdx);
            int basePriority = (currentDepth == maxDepth) ? 1 : 2;

            ProcessExpression(subExpr, basePriority, true);

            float result = EvaluateExpression(subExpr);
            int leftIdx = GetFirstNumberSlotIndex(working, openIdx);

            working.RemoveRange(openIdx, closeIdx - openIdx + 1);
            working.Insert(openIdx, new NumberWithSlot { value = (int)result, slotIndex = leftIdx });
        }

        ProcessExpression(working, 0, false);
        UpdateUnusedTexts();
    }

    private int GetMaxParenthesisDepth(List<object> sequence)
    {
        int maxDepth = 0;
        int currentDepth = 0;

        foreach (var item in sequence)
        {
            if (item is ParenthesisType type)
            {
                if (type == ParenthesisType.Open)
                {
                    currentDepth++;
                    if (currentDepth > maxDepth)
                        maxDepth = currentDepth;
                }
                else if (type == ParenthesisType.Close)
                {
                    currentDepth--;
                }
            }
        }

        return maxDepth;
    }

    private int GetCurrentDepth(List<object> sequence, int openIndex)
    {
        int depth = 0;

        for (int i = 0; i <= openIndex; i++)
        {
            if (sequence[i] is ParenthesisType type)
            {
                if (type == ParenthesisType.Open)
                    depth++;
                else if (type == ParenthesisType.Close)
                    depth--;
            }
        }

        return depth;
    }

    private void ProcessExpression(List<object> seq, int parenthesisPriority, bool isInsideParenthesis)
    {
        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorWithSlot opSlot && (opSlot.operatorMode == OperatorMode.Multiply || opSlot.operatorMode == OperatorMode.Divide))
            {
                if (seq[i - 1] is NumberWithSlot left && seq[i + 1] is NumberWithSlot right)
                {
                    float result = 0;
                    bool hasDivide = opSlot.operatorMode == OperatorMode.Divide;

                    if (opSlot.operatorMode == OperatorMode.Multiply)
                        result = left.value * right.value;
                    else if (opSlot.operatorMode == OperatorMode.Divide && right.value != 0)
                        result = (float)left.value / right.value;

                    int priority = isInsideParenthesis ? parenthesisPriority : 3;
                    UpdateTextForOperatorSlot(opSlot.slotIndex, result, hasDivide, priority);

                    seq[i - 1] = new NumberWithSlot { value = (int)result, slotIndex = left.slotIndex };
                    seq.RemoveAt(i + 1);
                    seq.RemoveAt(i);
                    i -= 2;
                }
            }
        }

        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorWithSlot opSlot && (opSlot.operatorMode == OperatorMode.Plus || opSlot.operatorMode == OperatorMode.Minus))
            {
                if (seq[i - 1] is NumberWithSlot left && seq[i + 1] is NumberWithSlot right)
                {
                    float result = 0;

                    if (opSlot.operatorMode == OperatorMode.Plus)
                        result = left.value + right.value;
                    else if (opSlot.operatorMode == OperatorMode.Minus)
                        result = left.value - right.value;

                    int priority = isInsideParenthesis ? parenthesisPriority : 4;
                    UpdateTextForOperatorSlot(opSlot.slotIndex, result, false, priority);

                    seq[i - 1] = new NumberWithSlot { value = (int)result, slotIndex = left.slotIndex };
                    seq.RemoveAt(i + 1);
                    seq.RemoveAt(i);
                    i -= 2;
                }
            }
        }
    }

    private void UpdateTextForOperatorSlot(int operatorSlotIndex, float result, bool hasDivide, int priority)
    {
        var operatorSlots = gameManager.GetOperatorSlots();

        foreach (var slot in stepTextSlots)
        {
            if (slot.operatorSlot == null) continue;

            for (int i = 0; i < operatorSlots.Length; i++)
            {
                if (operatorSlots[i] == slot.operatorSlot && i == operatorSlotIndex)
                {
                    slot.resultText.text = hasDivide ? NumberFormatter.FormatWithCommas(result, 2) : NumberFormatter.FormatWithCommas((int)result);

                    switch (priority)
                    {
                        case 1:
                            slot.resultText.color = innermostParenthesisColor;
                            break;
                        case 2:
                            slot.resultText.color = outerParenthesisColor;
                            break;
                        case 3:
                            slot.resultText.color = multiplyDivideColor;
                            break;
                        case 4:
                            slot.resultText.color = plusMinusColor;
                            break;
                        default:
                            slot.resultText.color = defaultColor;
                            break;
                    }

                    slot.priority = priority;
                    return;
                }
            }
        }
    }

    private void UpdateUnusedTexts()
    {
        foreach (var slot in stepTextSlots)
        {
            if (slot.priority == 0)
            {
                slot.resultText.text = "0";
                slot.resultText.color = defaultColor;
            }
        }
    }

    private float EvaluateExpression(List<object> sequence)
    {
        if (sequence.Count == 0) return 0;

        List<object> seq = new List<object>(sequence);

        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorWithSlot opSlot && (opSlot.operatorMode == OperatorMode.Multiply || opSlot.operatorMode == OperatorMode.Divide))
            {
                if (seq[i - 1] is NumberWithSlot left && seq[i + 1] is NumberWithSlot right)
                {
                    float result = (opSlot.operatorMode == OperatorMode.Multiply) ? left.value * right.value : (right.value != 0 ? (float)left.value / right.value : 0);

                    seq[i - 1] = new NumberWithSlot { value = (int)result, slotIndex = left.slotIndex };
                    seq.RemoveAt(i + 1);
                    seq.RemoveAt(i);
                    i -= 2;
                }
            }
        }

        if (seq.Count == 0 || !(seq[0] is NumberWithSlot)) return 0;

        float sum = ((NumberWithSlot)seq[0]).value;

        for (int i = 1; i < seq.Count - 1; i += 2)
        {
            if (seq[i] is OperatorWithSlot opSlot && seq[i + 1] is NumberWithSlot next)
            {
                if (opSlot.operatorMode == OperatorMode.Plus)
                    sum += next.value;
                else if (opSlot.operatorMode == OperatorMode.Minus)
                    sum -= next.value;
            }
        }

        return sum;
    }

    private int GetFirstNumberSlotIndex(List<object> seq, int startIdx)
    {
        for (int i = startIdx; i < seq.Count; i++)
        {
            if (seq[i] is NumberWithSlot num)
                return num.slotIndex;
        }

        return 0;
    }

    private bool HasParenthesis(List<object> seq)
    {
        foreach (var item in seq)
        {
            if (item is ParenthesisType) return true;
        }

        return false;
    }

    private int FindInnermostOpenParen(List<object> seq)
    {
        int lastOpen = -1;

        for (int i = 0; i < seq.Count; i++)
        {
            if (seq[i] is ParenthesisType type)
            {
                if (type == ParenthesisType.Open)
                    lastOpen = i;
                else if (type == ParenthesisType.Close)
                    return lastOpen;
            }
        }

        return lastOpen;
    }

    private int FindMatchingCloseParen(List<object> seq, int openIdx)
    {
        if (openIdx < 0) return -1;

        int depth = 0;

        for (int i = openIdx; i < seq.Count; i++)
        {
            if (seq[i] is ParenthesisType type)
            {
                if (type == ParenthesisType.Open)
                    depth++;
                else if (type == ParenthesisType.Close)
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }
        }

        return -1;
    }

    private class NumberWithSlot
    {
        public int value;
        public int slotIndex;
    }

    private class OperatorWithSlot
    {
        public OperatorMode operatorMode;
        public int slotIndex;
    }
}