using TMPro;
using UnityEngine;

public class OperatorBalloon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI operatorText;

    private OperatorMode operatorMode;
    public OperatorMode OperatorMode => operatorMode;

    public bool IsOperator => true;

    public void Initialize(OperatorMode mode)
    {
        operatorMode = mode;

        switch (mode)
        {
            case OperatorMode.Add:
                operatorText.text = "+";
                break;
            case OperatorMode.Minus:
                operatorText.text = "-";
                break;
            case OperatorMode.Multiply:
                operatorText.text = "×";
                break;
            case OperatorMode.Divide:
                operatorText.text = "÷";
                break;
        }
    }
}