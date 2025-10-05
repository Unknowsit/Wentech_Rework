using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BalloonDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalSlot;
    public Image balloonImage;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalSlot = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        balloonImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalSlot);
        balloonImage.raycastTarget = true;
    }
}