using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class UI : MonoBehaviour
{
    public void PlusCardButtonOnTheRight()
    {
        StartCoroutine(PlusButtonOnTheRight());
    }

    IEnumerator PlusButtonOnTheRight()
    {
        StartCoroutine(DelayAction("PlusCard", false, 1.4f));
        StartCoroutine(DelayAction("MinusCard", true, 1f));

        StartCoroutine(DelayOrderAction("PlusCardInTheMiddle", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("PlusCardInTheRight", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("PlusHiddenCardInTheRight", 0, 1.4f, 1f));

        StartCoroutine(DelayActionToggle("MinusBarrier", 1.4f, 0f));
        yield break;
    }

    public void MinusCardButtonOnTheLeft()
    {
        StartCoroutine(MinusButtonOnTheLeft());
    }

    IEnumerator MinusButtonOnTheLeft()
    {
        StartCoroutine(DelayAction("MinusCard", false, 1.4f));
        StartCoroutine(DelayAction("PlusCard", true, 1f));

        StartCoroutine(DelayOrderAction("MinusCardInTheMiddle", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheRight", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheLeft", -1, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MinusHiddenCardInTheRight", 0, 1.4f, 1f));

        StartCoroutine(DelayOrderAction("MinusBG", 0, 1f, 1f));

        StartCoroutine(DelayActionToggle("PlusBarrier", 1.4f, 0f));
        yield break;
    }

    public void MinusCardButtonOnTheRight()
    {
        StartCoroutine(MinusButtonOnTheRight());
    }

    IEnumerator MinusButtonOnTheRight()
    {
        StartCoroutine(DelayAction("MinusCard", false, 1.4f));
        StartCoroutine(DelayAction("MultiplyCard", true, 1f));

        StartCoroutine(DelayOrderAction("MinusCardInTheMiddle", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheRight", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheLeft", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MinusHiddenCardInTheRight", 0, 1.4f, 1f));

        StartCoroutine(DelayActionToggle("MultiplyBarrier", 1.4f, 0f));
        yield break;
    }

    public void MultiplyCardButtonOnTheLeft()
    {
        StartCoroutine(MultiplyButtonOnTheLeft());
    }

    IEnumerator MultiplyButtonOnTheLeft()
    {
        StartCoroutine(DelayAction("MultiplyCard", false, 1.4f));
        StartCoroutine(DelayAction("MinusCard", true, 1f));

        StartCoroutine(DelayOrderAction("MultiplyCardInTheMiddle", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheRight", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheLeft", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyHiddenCardInTheLeft", 0, 1.4f, 1f));

        StartCoroutine(DelayOrderAction("MultiplyBG", 0, 1f, 1f));

        StartCoroutine(DelayActionToggle("MinusBarrier", 1.4f, 0f));
        yield break;
    }

    public void MultiplyCardButtonOnTheRight()
    {
        StartCoroutine(MultiplyButtonOnTheRight());
    }

    IEnumerator MultiplyButtonOnTheRight()
    {
        StartCoroutine(DelayAction("MultiplyCard", false, 1.4f));
        StartCoroutine(DelayAction("DivideCard", true, 1f));

        StartCoroutine(DelayOrderAction("MultiplyCardInTheMiddle", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheRight", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheLeft", 0, 1.4f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyHiddenCardInTheLeft", 0, 1.4f, 1f));

        StartCoroutine(DelayActionToggle("DivideBarrier", 1.4f, 0f));
        yield break;
    }

    public void DivideCardButtonOnTheLeft()
    {
        StartCoroutine(DivideButtonOnTheLeft());
    }

    IEnumerator DivideButtonOnTheLeft()
    {
        StartCoroutine(DelayAction("DivideCard", false, 1.4f));
        StartCoroutine(DelayAction("MultiplyCard", true, 1f));

        StartCoroutine(DelayOrderAction("DivideCardInTheMiddle", 0, 1f, 1f));
        StartCoroutine(DelayOrderAction("DivideCardInTheLeft", 0, 1f, 1f));
        StartCoroutine(DelayOrderAction("DivideHiddenCardInTheLeft", 0, 1f, 1f));

        StartCoroutine(DelayOrderAction("DivideBG", 0, 1f, 1f));

        StartCoroutine(DelayActionToggle("MultiplyBarrier", 1.4f, 0f));
        yield break;
    }

    GameObject FindEvenIfInactive(string name)
    {
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }

    IEnumerator DelayAction(string objName, bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject obj = FindEvenIfInactive(objName);
        if (obj != null) obj.SetActive(state);
    }

    IEnumerator DelayActionToggle(string objName, float activeTime, float delayBeforeStart = 0f)
    {
        if (delayBeforeStart > 0f)
            yield return new WaitForSeconds(delayBeforeStart);

        GameObject obj = FindEvenIfInactive(objName);
        if (obj != null)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(activeTime);
            obj.SetActive(false);
        }
    }

    IEnumerator DelayOrderAction(string objName, int tempOrder, float duration, float startDelay)
    {
        GameObject obj = FindEvenIfInactive(objName);
        if (obj == null) yield break;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            int originalOrder = sr.sortingOrder;

            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            sr.sortingOrder = tempOrder;
            yield return new WaitForSeconds(duration);
            sr.sortingOrder = originalOrder;
        }
    }
}
