using UnityEngine;
using UnityEngine.EventSystems;

public class OperatorSlot : MonoBehaviour, IDropHandler
{
    private GameManager gameManager;
    private UIManager uiManager;

    private OperatorBalloon currentOperator;

    private void Start()
    {
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
        {
            Debug.LogError("Dropped object is null!");
            return;
        }

        var droppedOperator = droppedObject.GetComponent<OperatorBalloon>();

        if (droppedOperator == null)
        {
            Debug.LogWarning("OperatorSlot: This slot only accepts Operator balloons!");
            return;
        }

        OperatorDragHandler draggedOperator = droppedObject.GetComponent<OperatorDragHandler>();

        if (transform.childCount == 0 || currentOperator == null)
        {
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;
            draggedOperator.originalSlot = transform;
            currentOperator = droppedOperator;

            Debug.Log($"Placed {droppedOperator.OperatorMode} in {gameObject.name}");
        }
        else
        {
            Debug.Log($"Replacing {currentOperator.OperatorMode} with {droppedOperator.OperatorMode}");

            GameObject oldOperator = currentOperator.gameObject;
            currentOperator = null;
            Destroy(oldOperator);

            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;
            draggedOperator.originalSlot = transform;
            currentOperator = droppedOperator;

            Debug.Log($"Replaced with {droppedOperator.OperatorMode} in {gameObject.name}");
        }

        RefreshSum();
    }

    public OperatorMode? GetOperatorMode()
    {
        return currentOperator != null ? currentOperator.OperatorMode : (OperatorMode?)null;
    }

    public bool HasOperator()
    {
        return currentOperator != null && transform.childCount > 0;
    }

    public void OnOperatorRemoved()
    {
        currentOperator = null;
        RefreshSum();
    }

    public void SetOperator(OperatorBalloon operatorBalloon)
    {
        currentOperator = operatorBalloon;

        if (operatorBalloon != null)
        {
            operatorBalloon.transform.SetParent(transform);
            operatorBalloon.transform.localPosition = Vector3.zero;
        }

        RefreshSum();
    }

    private void RefreshSum()
    {
        if (GameData.IsSingleMode())
        {
            gameManager.UpdateBalloonSum(uiManager.TotalText);
        }
        else
        {
            gameManager.UpdateBalloonSum(uiManager.MultiTotalText);
        }
    }
}