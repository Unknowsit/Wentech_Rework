using UnityEngine;
using UnityEngine.EventSystems;

public class BalloonSlotClickReceiver : MonoBehaviour, IPointerClickHandler
{
    private BalloonSlot balloonSlot;

    private void Awake()
    {
        balloonSlot = GetComponent<BalloonSlot>();

        if (balloonSlot == null)
        {
            enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (balloonSlot == null) return;

        BalloonClickHandler selected = BalloonClickHandler.GetCurrentlySelected();

        if (selected == null) return;

        selected.MoveToBalloonSlot(balloonSlot);

        eventData.Use();
    }
}