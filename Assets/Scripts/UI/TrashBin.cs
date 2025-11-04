using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IDropHandler
{
    private AudioManager audioManager;
    private GameManager gameManager;
    private UIManager uiManager;

    [Header("Visual Feedback")]
    [SerializeField] private Image binImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.red;

    private void Start()
    {
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;

        binImage = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
            return;

        var operatorBalloon = droppedObject.GetComponent<OperatorBalloon>();
        var numberBalloon = droppedObject.GetComponent<BalloonHitText>();

        if (operatorBalloon != null)
        {
            Debug.Log($"Deleting operator balloon: {operatorBalloon.OperatorMode}");
            OperatorDragHandler dragHandler = droppedObject.GetComponent<OperatorDragHandler>();

            if (dragHandler != null && dragHandler.originalSlot != null)
            {
                BalloonSlot slot = dragHandler.originalSlot.GetComponent<BalloonSlot>();

                if (slot != null)
                {
                    slot.OnBalloonRemoved();
                    Debug.Log("Removed operator from slot");
                }
            }

            Destroy(droppedObject);
            Debug.Log("Operator destroyed");

            audioManager.PlaySFX("SFX01");

            if (GameData.IsSingleMode())
            {
                gameManager.UpdateBalloonSum(uiManager.TotalText);
            }
            else
            {
                gameManager.UpdateBalloonSum(uiManager.MultiTotalText);
            }
        }
        else if (numberBalloon != null)
        {
            Debug.LogWarning("Cannot delete number balloons!");

            /*
            // If you want to the function delete NumberBalloon
            BalloonDragHandler dragHandler = droppedObject.GetComponent<BalloonDragHandler>();
            if (dragHandler != null && dragHandler.originalSlot != null)
            {
                BalloonSlot slot = dragHandler.originalSlot.GetComponent<BalloonSlot>();
                if (slot != null)
                {
                    slot.OnBalloonRemoved();
                }
            }
            
            Destroy(droppedObject);
            
            AudioManager.instance.PlaySFX("SFX01");
            
            if (GameData.IsSingleMode())
            {
                gameManager.UpdateBalloonSum(uiManager.TotalText);
            }
            else
            {
                gameManager.UpdateBalloonSum(uiManager.MultiTotalText);
            }
            */
        }
        else
        {
            Debug.LogWarning("Unknown balloon type!");
        }

        binImage.color = normalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        binImage.color = highlightColor;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        binImage.color = normalColor;
    }
}