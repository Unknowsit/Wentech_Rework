using UnityEngine;
using UnityEngine.EventSystems;

public class OperatorBalloonClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Selection Settings")]
    [SerializeField] private float selectedScale = 1.15f;

    private bool isSelected = false;
    private static OperatorBalloonClickHandler currentlySelected;

    private OperatorBalloon operatorBalloon;
    private AudioManager audioManager;
    private GameManager gameManager;

    private void Awake()
    {
        operatorBalloon = GetComponent<OperatorBalloon>();
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool isInSlot = IsInOperatorSlot();

        if (isInSlot && currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.MoveToSlot(transform.parent.GetComponent<OperatorSlot>());
            return;
        }

        if (isInSlot && isSelected)
        {
            Deselect();
            audioManager.PlaySFX("SFX02");
            return;
        }

        if (!isInSlot)
        {
            if (currentlySelected != null && currentlySelected != this)
            {
                currentlySelected.Deselect();
                Select();
                return;
            }

            if (isSelected)
            {
                Deselect();
                audioManager.PlaySFX("SFX02");
            }
            else
            {
                Select();
            }
        }
        else
        {
            if (currentlySelected != null && currentlySelected != this)
            {
                currentlySelected.Deselect();
            }

            Select();
        }
    }

    private bool IsInOperatorSlot()
    {
        return transform.parent != null && transform.parent.GetComponent<OperatorSlot>() != null;
    }

    private void Select()
    {
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.Deselect();
        }

        isSelected = true;
        currentlySelected = this;
        transform.localScale = Vector3.one * selectedScale;
        audioManager.PlaySFX("SFX02");
    }

    public void Deselect()
    {
        if (!isSelected) return;

        isSelected = false;

        if (currentlySelected == this)
        {
            currentlySelected = null;
        }

        transform.localScale = Vector3.one;
    }

    public void MoveToSlot(OperatorSlot targetSlot)
    {
        if (targetSlot == null || operatorBalloon == null)
        {
            return;
        }

        Transform currentParent = transform.parent;
        OperatorSlot currentSlot = currentParent?.GetComponent<OperatorSlot>();
        bool wasFromSource = (currentSlot == null);
        int siblingIndex = transform.GetSiblingIndex();

        OperatorBalloon existingOperator = GetOperatorInSlot(targetSlot);

        if (existingOperator != null && existingOperator == operatorBalloon)
        {
            Deselect();
            return;
        }

        if (existingOperator != null && currentSlot != null)
        {
            currentSlot.OnOperatorRemoved();
            targetSlot.OnOperatorRemoved();

            existingOperator.transform.SetParent(currentSlot.transform);
            existingOperator.transform.localPosition = Vector3.zero;
            existingOperator.transform.localScale = Vector3.one;
            currentSlot.SetOperator(existingOperator);

            transform.SetParent(targetSlot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            targetSlot.SetOperator(operatorBalloon);
        }
        else
        {
            if (currentSlot != null)
            {
                currentSlot.OnOperatorRemoved();
            }

            if (existingOperator != null)
            {
                targetSlot.OnOperatorRemoved();
                Destroy(existingOperator.gameObject);
            }

            transform.SetParent(targetSlot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            targetSlot.SetOperator(operatorBalloon);

            if (wasFromSource)
            {
                gameManager.RespawnOperatorBalloon(operatorBalloon.OperatorMode, siblingIndex);
            }
        }

        Deselect();
        audioManager.PlaySFX("SFX03");
    }

    private OperatorBalloon GetOperatorInSlot(OperatorSlot slot)
    {
        if (slot == null || slot.transform.childCount == 0)
            return null;

        foreach (Transform child in slot.transform)
        {
            OperatorBalloon op = child.GetComponent<OperatorBalloon>();

            if (op != null)
                return op;
        }

        return null;
    }

    public static OperatorBalloonClickHandler GetCurrentlySelected()
    {
        return currentlySelected;
    }

    public static void DeselectCurrent()
    {
        if (currentlySelected != null)
        {
            currentlySelected.Deselect();
        }
    }

    private void OnDestroy()
    {
        if (currentlySelected == this)
        {
            currentlySelected = null;
        }
    }
}