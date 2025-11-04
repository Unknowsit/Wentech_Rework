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

        if (transform.childCount == 0 || currentBalloon == null)
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

            if (existingBalloon == null)
            {
                Debug.LogError("Existing balloon doesn't have BalloonDragHandler!");
                return;
            }

            Transform previousSlot = draggedBalloon.originalSlot;

            existingBalloon.originalSlot = previousSlot;
            draggedBalloon.originalSlot = transform;

            existingBalloonTransform.SetParent(previousSlot);
            existingBalloonTransform.localPosition = Vector3.zero;

            droppedObject.transform.SetParent(transform);
            droppedObject.transform.localPosition = Vector3.zero;

            if (previousSlot != null)
            {
                NumberSlot prevSlot = previousSlot.GetComponent<NumberSlot>();

                if (prevSlot != null)
                {
                    prevSlot.SetBalloon(existingBalloonTransform.GetComponent<BalloonHitText>());
                }
            }

            currentBalloon = droppedNumber;
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