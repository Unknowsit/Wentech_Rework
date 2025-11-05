using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BalloonDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalSlot;
    RectTransform rect;
    Canvas canvas;
    public Image balloonImage;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.instance;
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
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
        //transform.position = Input.mousePosition;
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
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