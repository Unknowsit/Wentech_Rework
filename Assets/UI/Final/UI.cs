using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{
    public void PlusCardButtonOnTheRight()
    {
        StartCoroutine(PlusButtonOnTheRight());
    }

    IEnumerator PlusButtonOnTheRight()
    {
        StartCoroutine(DelayAction("PlusCard", false, 1.2f));
        StartCoroutine(DelayAction("MinusCard", true, 1f));
        StartCoroutine(DelayOrderAction("MinusCard", 0, 2f, 0f));
        yield break;
    }

    public void MinusCardButtonOnTheLeft()
    {
        StartCoroutine(MinusButtonOnTheLeft());
    }

    IEnumerator MinusButtonOnTheLeft()
    {
        StartCoroutine(DelayAction("MinusCard", false, 1.2f));
        StartCoroutine(DelayAction("PlusCard", true, 1f));

        StartCoroutine(DelayOrderAction("MinusCardInTheMiddle", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheRight", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheLeft", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MinusHiddenCardInTheRight", 0, 2f, 1f));
        yield break;
    }

    public void MinusCardButtonOnTheRight()
    {
        StartCoroutine(MinusButtonOnTheRight());
    }

    IEnumerator MinusButtonOnTheRight()
    {
        StartCoroutine(DelayAction("MinusCard", false, 1.2f));
        StartCoroutine(DelayAction("MultiplyCard", true, 1f));

        StartCoroutine(DelayOrderAction("MinusCardInTheMiddle", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheRight", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MinusCardInTheLeft", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MinusHiddenCardInTheRight", 0, 2f, 1f));
        yield break;
    }

    public void MultiplyCardButtonOnTheLeft()
    {
        StartCoroutine(MultiplyButtonOnTheLeft());
    }

    IEnumerator MultiplyButtonOnTheLeft()
    {
        StartCoroutine(DelayAction("MultiplyCard", false, 1.2f));
        StartCoroutine(DelayAction("MinusCard", true, 1f));

        StartCoroutine(DelayOrderAction("MultiplyCardInTheMiddle", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheRight", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheLeft", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyHiddenCardInTheLeft", 0, 2f, 1f));
        yield break;
    }

    public void MultiplyCardButtonOnTheRight()
    {
        StartCoroutine(MultiplyButtonOnTheRight());
    }

    IEnumerator MultiplyButtonOnTheRight()
    {
        StartCoroutine(DelayAction("MultiplyCard", false, 1.2f));
        StartCoroutine(DelayAction("DivideCard", true, 1f));

        StartCoroutine(DelayOrderAction("MultiplyCardInTheMiddle", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheRight", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyCardInTheLeft", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("MultiplyHiddenCardInTheLeft", 0, 2f, 1f));
        yield break;
    }

    public void DivideCardButtonOnTheLeft()
    {
        StartCoroutine(DivideButtonOnTheLeft());
    }

    IEnumerator DivideButtonOnTheLeft()
    {
        StartCoroutine(DelayAction("DivideCard", false, 1.2f));
        StartCoroutine(DelayAction("MultiplyCard", true, 1f));

        StartCoroutine(DelayOrderAction("DivideCardInTheMiddle", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("DivideCardInTheLeft", 0, 2f, 1f));
        StartCoroutine(DelayOrderAction("DivideHiddenCardInTheLeft", 0, 2f, 1f));
        yield break;
    }

    // 🔹 ใช้หา object แม้จะ inactive
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
