using UnityEngine;

[CreateAssetMenu(fileName = "BalloonRangeData", menuName = "Scriptable Objects/BalloonRangeData")]
public class BalloonRangeData : ScriptableObject
{
    [System.Serializable]
    public class ModeRange
    {
        public OperatorMode mode;
        public int minValue = 1;
        public int maxValue = 100;
    }

    public ModeRange plusRange = new ModeRange { mode = OperatorMode.Plus, minValue = 1, maxValue = 100 };
    public ModeRange minusRange = new ModeRange { mode = OperatorMode.Minus, minValue = -100, maxValue = -1 };
    public ModeRange multiplyRange = new ModeRange { mode = OperatorMode.Multiply, minValue = 2, maxValue = 12 };
    public ModeRange divideRange = new ModeRange { mode = OperatorMode.Divide, minValue = 2, maxValue = 12 };

    public void LoadFromPlayerPrefs()
    {
        plusRange.minValue = PlayerPrefs.GetInt("PlusMin", 1);
        plusRange.maxValue = PlayerPrefs.GetInt("PlusMax", 100);

        minusRange.minValue = PlayerPrefs.GetInt("MinusMin", -100);
        minusRange.maxValue = PlayerPrefs.GetInt("MinusMax", -1);

        multiplyRange.minValue = PlayerPrefs.GetInt("MultiplyMin", 2);
        multiplyRange.maxValue = PlayerPrefs.GetInt("MultiplyMax", 12);

        divideRange.minValue = PlayerPrefs.GetInt("DivideMin", 2);
        divideRange.maxValue = PlayerPrefs.GetInt("DivideMax", 12);

        Debug.Log("BalloonRangeData loaded from PlayerPrefs");
    }

    public int GetMinValue(OperatorMode mode)
    {
        switch (mode)
        {
            case OperatorMode.Plus: return plusRange.minValue;
            case OperatorMode.Minus: return minusRange.minValue;
            case OperatorMode.Multiply: return multiplyRange.minValue;
            case OperatorMode.Divide: return divideRange.minValue;
            default: return 1;
        }
    }

    public int GetMaxValue(OperatorMode mode)
    {
        switch (mode)
        {
            case OperatorMode.Plus: return plusRange.maxValue;
            case OperatorMode.Minus: return minusRange.maxValue;
            case OperatorMode.Multiply: return multiplyRange.maxValue;
            case OperatorMode.Divide: return divideRange.maxValue;
            default: return 100;
        }
    }

    public void SetRange(OperatorMode mode, int min, int max)
    {
        switch (mode)
        {
            case OperatorMode.Plus:
                plusRange.minValue = min;
                plusRange.maxValue = max;
                PlayerPrefs.SetInt("PlusMin", min);
                PlayerPrefs.SetInt("PlusMax", max);
                break;
            case OperatorMode.Minus:
                minusRange.minValue = min;
                minusRange.maxValue = max;
                PlayerPrefs.SetInt("MinusMin", min);
                PlayerPrefs.SetInt("MinusMax", max);
                break;
            case OperatorMode.Multiply:
                multiplyRange.minValue = min;
                multiplyRange.maxValue = max;
                PlayerPrefs.SetInt("MultiplyMin", min);
                PlayerPrefs.SetInt("MultiplyMax", max);
                break;
            case OperatorMode.Divide:
                divideRange.minValue = min;
                divideRange.maxValue = max;
                PlayerPrefs.SetInt("DivideMin", min);
                PlayerPrefs.SetInt("DivideMax", max);
                break;
        }

        PlayerPrefs.Save();
    }

    public void ResetToDefault()
    {
        SetRange(OperatorMode.Plus, 1, 100);
        SetRange(OperatorMode.Minus, -100, -1);
        SetRange(OperatorMode.Multiply, 2, 12);
        SetRange(OperatorMode.Divide, 2, 12);
    }
}