using UnityEngine;
using UnityEngine.EventSystems;

public class BalloonSlot : MonoBehaviour, IDropHandler
{
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

            draggedBalloon.originalSlot = transform;
        }
    }
}