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

    public void InitializeBalloon()
    {
        if (transform.childCount > 0 && currentBalloon == null)
        {
            BalloonHitText childBalloon = transform.GetChild(0).GetComponent<BalloonHitText>();
            if (childBalloon != null)
            {
                currentBalloon = childBalloon;
                BalloonDragHandler dragHandler = childBalloon.GetComponent<BalloonDragHandler>();

                if (dragHandler != null)
                {
                    dragHandler.originalSlot = transform;
                }
            }
        }
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

        if (transform.childCount == 0)
        {
            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;
            draggedBalloon.originalSlot = transform;
            currentBalloon = droppedNumber;
        }
        else
        {
            Transform existingBalloonTransform = transform.GetChild(0);
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

    public int GetBalloonValue()
    {
        return currentBalloon != null ? currentBalloon.Value : 0;
    }

    public bool HasNumber()
    {
        return currentBalloon != null && transform.childCount > 0;
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
        }
    }
}