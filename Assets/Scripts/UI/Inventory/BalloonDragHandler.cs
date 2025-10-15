using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BalloonDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalSlot;
    public Image balloonImage;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalSlot = transform.parent;

        BalloonSlot slot = originalSlot.GetComponent<BalloonSlot>();
        if (slot != null)
        {
            slot.OnBalloonRemoved();
        }

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        balloonImage.raycastTarget = false;
        audioManager.PlaySFX("SFX02");
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.parent == transform.root)
        {
            transform.SetParent(originalSlot);

            BalloonSlot slot = originalSlot.GetComponent<BalloonSlot>();
            if (slot != null)
            {
                slot.SetBalloon(GetComponent<BalloonHitText>());
            }
        }

        balloonImage.raycastTarget = true;
        audioManager.PlaySFX("SFX03");
    }
}