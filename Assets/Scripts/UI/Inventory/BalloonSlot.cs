using UnityEngine;
using UnityEngine.EventSystems;

public class BalloonSlot : MonoBehaviour, IDropHandler
{
    private GameManager gameManager;
    private BalloonHitText currentBalloon;

    private void Awake()
    {
        gameManager = GameManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        BalloonDragHandler draggedBalloon = droppedObject.GetComponent<BalloonDragHandler>();

        if (transform.childCount == 0)
        {
            draggedBalloon.originalSlot = transform;
        }
        else
        {
            Transform existingBalloonTransform = transform.GetChild(0);
            BalloonDragHandler existingBalloon = existingBalloonTransform.GetComponent<BalloonDragHandler>();

            Transform previousSlot = draggedBalloon.originalSlot;

            existingBalloon.originalSlot = previousSlot;
            existingBalloonTransform.SetParent(previousSlot);

            if (previousSlot.TryGetComponent(out BalloonSlot prevSlot))
            {
                prevSlot.SetBalloon(existingBalloonTransform.GetComponent<BalloonHitText>());
            }

            draggedBalloon.originalSlot = transform;
        }

        currentBalloon = droppedObject.GetComponent<BalloonHitText>();
        gameManager.UpdateBalloonSum();
    }

    public int GetBalloonValue()
    {
        return currentBalloon != null ? currentBalloon.Value : 0;
    }

    public void OnBalloonRemoved()
    {
        currentBalloon = null;
        gameManager.UpdateBalloonSum();
    }

    public void SetBalloon(BalloonHitText balloon)
    {
        currentBalloon = balloon;
        gameManager.UpdateBalloonSum();
    }
}