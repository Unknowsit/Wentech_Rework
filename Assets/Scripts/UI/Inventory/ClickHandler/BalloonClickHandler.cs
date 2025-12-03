using UnityEngine;
using UnityEngine.EventSystems;

public class BalloonClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Selection Settings")]
    [SerializeField] private float selectedScale = 1.15f;

    private bool isSelected = false;
    private static BalloonClickHandler currentlySelected;

    private BalloonHitText balloonHitText;
    private AudioManager audioManager;

    private void Awake()
    {
        balloonHitText = GetComponent<BalloonHitText>();
        audioManager = AudioManager.instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool isInSlot = IsInBalloonSlot();

        if (isInSlot && currentlySelected != null && currentlySelected != this)
        {
            Transform myParent = transform.parent;

            if (myParent.GetComponent<BalloonSlot>() != null)
            {
                currentlySelected.MoveToBalloonSlot(myParent.GetComponent<BalloonSlot>());
            }
            else if (myParent.GetComponent<NumberSlot>() != null)
            {
                currentlySelected.MoveToNumberSlot(myParent.GetComponent<NumberSlot>());
            }

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

    private bool IsInBalloonSlot()
    {
        if (transform.parent == null) return false;

        return transform.parent.GetComponent<BalloonSlot>() != null || transform.parent.GetComponent<NumberSlot>() != null;
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

    public void MoveToBalloonSlot(BalloonSlot targetSlot)
    {
        if (targetSlot == null || balloonHitText == null)
        {
            return;
        }

        Transform currentParent = transform.parent;
        BalloonSlot currentBalloonSlot = currentParent?.GetComponent<BalloonSlot>();
        NumberSlot currentNumberSlot = currentParent?.GetComponent<NumberSlot>();

        BalloonHitText existingBalloon = GetBalloonInSlot(targetSlot.transform);

        if (existingBalloon != null && existingBalloon == balloonHitText)
        {
            Deselect();
            return;
        }

        if (existingBalloon != null && (currentBalloonSlot != null || currentNumberSlot != null))
        {
            if (currentBalloonSlot != null)
            {
                currentBalloonSlot.OnBalloonRemoved();
            }
            else if (currentNumberSlot != null)
            {
                currentNumberSlot.OnBalloonRemoved();
            }

            targetSlot.OnBalloonRemoved();

            existingBalloon.transform.SetParent(currentParent);
            existingBalloon.transform.localPosition = Vector3.zero;
            existingBalloon.transform.localScale = Vector3.one;

            if (currentBalloonSlot != null)
            {
                currentBalloonSlot.SetBalloon(existingBalloon);
            }
            else if (currentNumberSlot != null)
            {
                currentNumberSlot.SetBalloon(existingBalloon);
            }

            transform.SetParent(targetSlot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            targetSlot.SetBalloon(balloonHitText);
        }
        else
        {
            if (currentBalloonSlot != null)
            {
                currentBalloonSlot.OnBalloonRemoved();
            }
            else if (currentNumberSlot != null)
            {
                currentNumberSlot.OnBalloonRemoved();
            }

            if (existingBalloon != null)
            {
                targetSlot.OnBalloonRemoved();
                Destroy(existingBalloon.gameObject);
            }

            transform.SetParent(targetSlot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            targetSlot.SetBalloon(balloonHitText);
        }

        Deselect();
        audioManager.PlaySFX("SFX03");
    }

    public void MoveToNumberSlot(NumberSlot targetSlot)
    {
        if (targetSlot == null || balloonHitText == null)
        {
            return;
        }

        Transform currentParent = transform.parent;
        NumberSlot currentNumberSlot = currentParent?.GetComponent<NumberSlot>();
        BalloonSlot currentBalloonSlot = currentParent?.GetComponent<BalloonSlot>();

        BalloonHitText existingBalloon = GetBalloonInSlot(targetSlot.transform);

        if (existingBalloon != null && existingBalloon == balloonHitText)
        {
            Deselect();
            return;
        }

        if (existingBalloon != null && (currentNumberSlot != null || currentBalloonSlot != null))
        {
            if (currentNumberSlot != null)
            {
                currentNumberSlot.OnBalloonRemoved();
            }
            else if (currentBalloonSlot != null)
            {
                currentBalloonSlot.OnBalloonRemoved();
            }

            targetSlot.OnBalloonRemoved();

            existingBalloon.transform.SetParent(currentParent);
            existingBalloon.transform.localPosition = Vector3.zero;
            existingBalloon.transform.localScale = Vector3.one;

            if (currentNumberSlot != null)
            {
                currentNumberSlot.SetBalloon(existingBalloon);
            }
            else if (currentBalloonSlot != null)
            {
                currentBalloonSlot.SetBalloon(existingBalloon);
            }

            transform.SetParent(targetSlot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            targetSlot.SetBalloon(balloonHitText);
        }
        else
        {
            if (currentNumberSlot != null)
            {
                currentNumberSlot.OnBalloonRemoved();
            }
            else if (currentBalloonSlot != null)
            {
                currentBalloonSlot.OnBalloonRemoved();
            }

            if (existingBalloon != null)
            {
                targetSlot.OnBalloonRemoved();
                Destroy(existingBalloon.gameObject);
            }

            transform.SetParent(targetSlot.transform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            targetSlot.SetBalloon(balloonHitText);
        }

        Deselect();
        audioManager.PlaySFX("SFX03");
    }

    private BalloonHitText GetBalloonInSlot(Transform slotTransform)
    {
        if (slotTransform == null || slotTransform.childCount == 0)
            return null;

        foreach (Transform child in slotTransform)
        {
            BalloonHitText balloon = child.GetComponent<BalloonHitText>();

            if (balloon != null)
                return balloon;
        }

        return null;
    }

    public static BalloonClickHandler GetCurrentlySelected()
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