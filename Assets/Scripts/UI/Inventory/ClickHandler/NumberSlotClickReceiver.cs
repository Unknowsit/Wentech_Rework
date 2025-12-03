using UnityEngine;
using UnityEngine.EventSystems;

public class NumberSlotClickReceiver : MonoBehaviour, IPointerClickHandler
{
    private NumberSlot numberSlot;

    private void Awake()
    {
        numberSlot = GetComponent<NumberSlot>();

        if (numberSlot == null)
        {
            enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (numberSlot == null) return;

        BalloonClickHandler selectedBalloon = BalloonClickHandler.GetCurrentlySelected();
        OperatorBalloonClickHandler selectedOperator = OperatorBalloonClickHandler.GetCurrentlySelected();

        if (selectedBalloon != null)
        {
            selectedBalloon.MoveToNumberSlot(numberSlot);
            eventData.Use();
        }
    }
}