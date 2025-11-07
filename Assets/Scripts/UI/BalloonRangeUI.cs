using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalloonRangeUI : MonoBehaviour
{
    [Header("Range Settings")]
    [SerializeField] private BalloonRangeData rangeSettings;

    [Header("Plus Range")]
    [SerializeField] private Slider plusMinSlider;
    [SerializeField] private Slider plusMaxSlider;
    [SerializeField] private TextMeshProUGUI plusMinText;
    [SerializeField] private TextMeshProUGUI plusMaxText;

    [Header("Minus Range")]
    [SerializeField] private Slider minusMinSlider;
    [SerializeField] private Slider minusMaxSlider;
    [SerializeField] private TextMeshProUGUI minusMinText;
    [SerializeField] private TextMeshProUGUI minusMaxText;

    [Header("Multiply Range")]
    [SerializeField] private Slider multiplyMinSlider;
    [SerializeField] private Slider multiplyMaxSlider;
    [SerializeField] private TextMeshProUGUI multiplyMinText;
    [SerializeField] private TextMeshProUGUI multiplyMaxText;

    [Header("Divide Range")]
    [SerializeField] private Slider divideMinSlider;
    [SerializeField] private Slider divideMaxSlider;
    [SerializeField] private TextMeshProUGUI divideMinText;
    [SerializeField] private TextMeshProUGUI divideMaxText;

    private void Start()
    {
        InitializeSliders();
        AddListeners();
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }

    private void InitializeSliders()
    {
        // Plus Range (1 - 1000)
        plusMinSlider.minValue = 1;
        plusMinSlider.maxValue = 1000;
        plusMinSlider.value = rangeSettings.plusRange.minValue;
        plusMaxSlider.minValue = 1;
        plusMaxSlider.maxValue = 1000;
        plusMaxSlider.value = rangeSettings.plusRange.maxValue;

        // Minus Range (-1000 - -1)
        minusMinSlider.minValue = -1000;
        minusMinSlider.maxValue = -1;
        minusMinSlider.value = rangeSettings.minusRange.minValue;
        minusMaxSlider.minValue = -1000;
        minusMaxSlider.maxValue = -1;
        minusMaxSlider.value = rangeSettings.minusRange.maxValue;

        // Multiply Range (2 - 100)
        multiplyMinSlider.minValue = 2;
        multiplyMinSlider.maxValue = 100;
        multiplyMinSlider.value = rangeSettings.multiplyRange.minValue;
        multiplyMaxSlider.minValue = 2;
        multiplyMaxSlider.maxValue = 100;
        multiplyMaxSlider.value = rangeSettings.multiplyRange.maxValue;

        // Divide Range (2 - 100)
        divideMinSlider.minValue = 2;
        divideMinSlider.maxValue = 100;
        divideMinSlider.value = rangeSettings.divideRange.minValue;
        divideMaxSlider.minValue = 2;
        divideMaxSlider.maxValue = 100;
        divideMaxSlider.value = rangeSettings.divideRange.maxValue;

        UpdateAllTexts();
    }

    private void AddListeners()
    {
        plusMinSlider.onValueChanged.AddListener(OnPlusMinChanged);
        plusMaxSlider.onValueChanged.AddListener(OnPlusMaxChanged);
        minusMinSlider.onValueChanged.AddListener(OnMinusMinChanged);
        minusMaxSlider.onValueChanged.AddListener(OnMinusMaxChanged);
        multiplyMinSlider.onValueChanged.AddListener(OnMultiplyMinChanged);
        multiplyMaxSlider.onValueChanged.AddListener(OnMultiplyMaxChanged);
        divideMinSlider.onValueChanged.AddListener(OnDivideMinChanged);
        divideMaxSlider.onValueChanged.AddListener(OnDivideMaxChanged);
    }

    private void RemoveListeners()
    {
        plusMinSlider.onValueChanged.RemoveListener(OnPlusMinChanged);
        plusMaxSlider.onValueChanged.RemoveListener(OnPlusMaxChanged);
        minusMinSlider.onValueChanged.RemoveListener(OnMinusMinChanged);
        minusMaxSlider.onValueChanged.RemoveListener(OnMinusMaxChanged);
        multiplyMinSlider.onValueChanged.RemoveListener(OnMultiplyMinChanged);
        multiplyMaxSlider.onValueChanged.RemoveListener(OnMultiplyMaxChanged);
        divideMinSlider.onValueChanged.RemoveListener(OnDivideMinChanged);
        divideMaxSlider.onValueChanged.RemoveListener(OnDivideMaxChanged);
    }

    private void OnPlusMinChanged(float value)
    {
        int min = Mathf.RoundToInt(value);
        int max = rangeSettings.plusRange.maxValue;

        if (min > max)
        {
            min = max;
            plusMinSlider.value = min;
        }

        rangeSettings.SetRange(OperatorMode.Plus, min, max);
        plusMinText.text = min.ToString();
    }

    private void OnPlusMaxChanged(float value)
    {
        int min = rangeSettings.plusRange.minValue;
        int max = Mathf.RoundToInt(value);

        if (max < min)
        {
            max = min;
            plusMaxSlider.value = max;
        }

        rangeSettings.SetRange(OperatorMode.Plus, min, max);
        plusMaxText.text = max.ToString();
    }

    private void OnMinusMinChanged(float value)
    {
        int min = Mathf.RoundToInt(value);
        int max = rangeSettings.minusRange.maxValue;

        if (min > max)
        {
            min = max;
            minusMinSlider.value = min;
        }

        rangeSettings.SetRange(OperatorMode.Minus, min, max);
        minusMinText.text = min.ToString();
    }

    private void OnMinusMaxChanged(float value)
    {
        int min = rangeSettings.minusRange.minValue;
        int max = Mathf.RoundToInt(value);

        if (max < min)
        {
            max = min;
            minusMaxSlider.value = max;
        }

        rangeSettings.SetRange(OperatorMode.Minus, min, max);
        minusMaxText.text = max.ToString();
    }

    private void OnMultiplyMinChanged(float value)
    {
        int min = Mathf.RoundToInt(value);
        int max = rangeSettings.multiplyRange.maxValue;

        if (min > max)
        {
            min = max;
            multiplyMinSlider.value = min;
        }

        rangeSettings.SetRange(OperatorMode.Multiply, min, max);
        multiplyMinText.text = min.ToString();
    }

    private void OnMultiplyMaxChanged(float value)
    {
        int min = rangeSettings.multiplyRange.minValue;
        int max = Mathf.RoundToInt(value);

        if (max < min)
        {
            max = min;
            multiplyMaxSlider.value = max;
        }

        rangeSettings.SetRange(OperatorMode.Multiply, min, max);
        multiplyMaxText.text = max.ToString();
    }

    private void OnDivideMinChanged(float value)
    {
        int min = Mathf.RoundToInt(value);
        int max = rangeSettings.divideRange.maxValue;

        if (min > max)
        {
            min = max;
            divideMinSlider.value = min;
        }

        rangeSettings.SetRange(OperatorMode.Divide, min, max);
        divideMinText.text = min.ToString();
    }

    private void OnDivideMaxChanged(float value)
    {
        int min = rangeSettings.divideRange.minValue;
        int max = Mathf.RoundToInt(value);

        if (max < min)
        {
            max = min;
            divideMaxSlider.value = max;
        }

        rangeSettings.SetRange(OperatorMode.Divide, min, max);
        divideMaxText.text = max.ToString();
    }

    private void UpdateAllTexts()
    {
        plusMinText.text = rangeSettings.plusRange.minValue.ToString();
        plusMaxText.text = rangeSettings.plusRange.maxValue.ToString();
        minusMinText.text = rangeSettings.minusRange.minValue.ToString();
        minusMaxText.text = rangeSettings.minusRange.maxValue.ToString();
        multiplyMinText.text = rangeSettings.multiplyRange.minValue.ToString();
        multiplyMaxText.text = rangeSettings.multiplyRange.maxValue.ToString();
        divideMinText.text = rangeSettings.divideRange.minValue.ToString();
        divideMaxText.text = rangeSettings.divideRange.maxValue.ToString();
    }

    public void ResetToDefault()
    {
        rangeSettings.SetRange(OperatorMode.Plus, 1, 100);
        rangeSettings.SetRange(OperatorMode.Minus, -100, -1);
        rangeSettings.SetRange(OperatorMode.Multiply, 2, 12);
        rangeSettings.SetRange(OperatorMode.Divide, 2, 12);

        InitializeSliders();
    }
}