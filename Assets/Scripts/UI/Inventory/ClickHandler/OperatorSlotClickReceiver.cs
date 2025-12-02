using UnityEngine;
using UnityEngine.EventSystems;

public class OperatorSlotClickReceiver : MonoBehaviour, IPointerClickHandler
{
    private OperatorSlot operatorSlot;

    private void Awake()
    {
        operatorSlot = GetComponent<OperatorSlot>();

        if (operatorSlot == null)
        {
            Debug.LogError($"[OperatorSlotClickReceiver] OperatorSlot not found on {gameObject.name}!");
            enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (operatorSlot == null) return;

        OperatorBalloonClickHandler selected = OperatorBalloonClickHandler.GetCurrentlySelected();

        if (selected == null)
        {
            return;
        }

        if (IsOperatorInThisSlot(selected.gameObject))
        {
            return;
        }

        selected.MoveToSlot(operatorSlot);
        eventData.Use();
    }

    private bool IsOperatorInThisSlot(GameObject operatorObj)
    {
        if (operatorObj == null || operatorSlot == null) return false;

        return operatorObj.transform.parent == operatorSlot.transform;
    }
}