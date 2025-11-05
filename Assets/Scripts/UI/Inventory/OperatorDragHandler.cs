using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OperatorDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform originalSlot;
    public Image balloonImage;

    private RectTransform rect;
    private Canvas canvas;

    private AudioManager audioManager;
    private GameManager gameManager;
    private OperatorBalloon operatorBalloon;
    private CanvasGroup canvasGroup;

    private bool isDraggingFromSource = false;

    private void Awake()
    {
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;

        operatorBalloon = GetComponent<OperatorBalloon>();
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag: {gameObject.name}");

        originalSlot = transform.parent;

        OperatorSlot operatorSlot = originalSlot.GetComponent<OperatorSlot>();
        isDraggingFromSource = (operatorSlot == null);

        int siblingIndex = transform.GetSiblingIndex();

        Debug.Log($"isDraggingFromSource: {isDraggingFromSource}, Parent: {originalSlot.name}");

        if (isDraggingFromSource && operatorBalloon != null)
        {
            gameManager.RespawnOperatorBalloon(operatorBalloon.OperatorMode, siblingIndex);
            Debug.Log($"Respawned {operatorBalloon.OperatorMode} operator");
        }
        else
        {
            if (operatorSlot != null)
            {
                operatorSlot.OnOperatorRemoved();
                Debug.Log("Removed from OperatorSlot");
            }
        }

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        if (balloonImage != null)
            balloonImage.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;

        if (audioManager != null)
        {
            audioManager.PlaySFX("SFX02");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = Input.mousePosition;
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"OnEndDrag: {gameObject.name}, Parent: {transform.parent.name}");

        if (transform.parent == transform.root)
        {
            if (!isDraggingFromSource)
            {
                transform.SetParent(originalSlot);
                OperatorSlot slot = originalSlot.GetComponent<OperatorSlot>();
                if (slot != null)
                {
                    slot.SetOperator(operatorBalloon);
                }
                Debug.Log("Returned to original slot");
            }
            else
            {
                Debug.Log("Destroying unused operator");
                Destroy(gameObject);
            }
        }

        balloonImage.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;

        audioManager.PlaySFX("SFX03");
    }
}