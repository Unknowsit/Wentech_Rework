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
                break;
            case OperatorMode.Minus:
                minusRange.minValue = min;
                minusRange.maxValue = max;
                break;
            case OperatorMode.Multiply:
                multiplyRange.minValue = min;
                multiplyRange.maxValue = max;
                break;
            case OperatorMode.Divide:
                divideRange.minValue = min;
                divideRange.maxValue = max;
                break;
        }
    }
}