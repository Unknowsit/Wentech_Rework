using UnityEngine;
using UnityEngine.EventSystems;

public class NumberSlot : MonoBehaviour, IDropHandler
{
    private GameManager gameManager;
    private UIManager uiManager;
    private BalloonHitText currentBalloon;

    private void Start()
    {
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        var droppedNumber = droppedObject.GetComponent<BalloonHitText>();

        if (droppedNumber == null)
        {
            Debug.LogWarning("NumberSlot: Rejected non-number balloon");
            return;
        }

        BalloonDragHandler draggedBalloon = droppedObject.GetComponent<BalloonDragHandler>();

        int balloonCount = GetBalloonChildCount();

        if (balloonCount == 0)
        {
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;
            draggedBalloon.originalSlot = transform;
            currentBalloon = droppedNumber;
        }
        else
        {
            Transform existingBalloonTransform = GetBalloonInChildren().transform;
            BalloonDragHandler existingBalloon = existingBalloonTransform.GetComponent<BalloonDragHandler>();

            Transform previousSlot = draggedBalloon.originalSlot;

            if (previousSlot != null && previousSlot != transform)
            {
                existingBalloon.originalSlot = previousSlot;
                draggedBalloon.originalSlot = transform;

                existingBalloonTransform.SetParent(previousSlot);
                existingBalloonTransform.localPosition = Vector3.zero;

                droppedObject.transform.SetParent(transform);
                droppedObject.transform.localPosition = Vector3.zero;

                NumberSlot prevSlot = previousSlot.GetComponent<NumberSlot>();

                if (prevSlot != null)
                {
                    prevSlot.SetBalloon(existingBalloonTransform.GetComponent<BalloonHitText>());
                }

                currentBalloon = droppedNumber;
            }
            else
            {
                droppedObject.transform.SetParent(transform);
                droppedObject.transform.localPosition = Vector3.zero;
                draggedBalloon.originalSlot = transform;
                currentBalloon = droppedNumber;
            }
        }

        RefreshSum();
    }

    public void InitializeBalloon()
    {
        if (currentBalloon == null)
        {
            currentBalloon = GetBalloonInChildren();

            if (currentBalloon != null)
            {
                BalloonDragHandler dragHandler = currentBalloon.GetComponent<BalloonDragHandler>();

                if (dragHandler != null)
                {
                    dragHandler.originalSlot = transform;
                }
            }
        }
    }

    private BalloonHitText GetBalloonInChildren()
    {
        foreach (Transform child in transform)
        {
            BalloonHitText balloon = child.GetComponent<BalloonHitText>();

            if (balloon != null)
            {
                return balloon;
            }
        }

        return null;
    }

    private int GetBalloonChildCount()
    {
        int count = 0;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<BalloonHitText>() != null)
            {
                count++;
            }
        }

        return count;
    }

    public int GetBalloonValue()
    {
        if (currentBalloon == null || currentBalloon.transform.parent != transform)
        {
            currentBalloon = GetBalloonInChildren();
        }

        return currentBalloon != null ? currentBalloon.Value : 0;
    }

    public bool HasNumber()
    {
        if (currentBalloon != null && currentBalloon.transform.parent == transform)
        {
            return true;
        }

        currentBalloon = GetBalloonInChildren();
        return currentBalloon != null;
    }

    public void OnBalloonRemoved()
    {
        currentBalloon = null;
        RefreshSum();
    }

    public void SetBalloon(BalloonHitText balloon)
    {
        currentBalloon = balloon;

        if (balloon != null)
        {
            balloon.transform.SetParent(transform);
            balloon.transform.localPosition = Vector3.zero;

            BalloonDragHandler dragHandler = balloon.GetComponent<BalloonDragHandler>();

            if (dragHandler != null)
            {
                dragHandler.originalSlot = transform;
            }
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
            gameManager.UpdateStepDisplay();
        }
    }
}