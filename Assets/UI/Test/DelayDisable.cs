using UnityEngine;
using System.Collections;

public class DelayDisable : MonoBehaviour
{
    public GameObject Object_1;
    public GameObject Object_2;
    public GameObject Object_3;
    public GameObject Object_4;
    public GameObject Object_5;
    public GameObject Object_6;

    public void Delay1Right()
    {
        StartCoroutine(DelayObject_Right());
    }

    IEnumerator DelayObject_Right()
    {
        // ปิด Object_1 หลัง 1.1 วิ
        StartCoroutine(DelayAction(Object_1, false, 1.1f));

        // เปิด Object_2 หลัง 1 วิ
        StartCoroutine(DelayAction(Object_2, true, 1f));

        // (ถ้าต้องการ) ปรับ order ของ Object_2 เป็น 0 ทันที คงไว้ 2 วิ
        // StartCoroutine(DelayOrderAction(Object_2, 0, 2f, 0f));

        yield break;
    }

    public void Delay2Left()
    {
        StartCoroutine(DelayObject_Left());
    }

    IEnumerator DelayObject_Left()
    {
        // ปิด Object_2 หลัง 1.1 วิ
        StartCoroutine(DelayAction(Object_2, false, 1.1f));

        // เปิด Object_1 หลัง 1 วิ
        StartCoroutine(DelayAction(Object_1, true, 1f));

        // ✅ ให้ Object 4,5,6 หน่วง 1 วิ ก่อน แล้วค่อยเปลี่ยน order เป็น 0 คงไว้ 2 วิ
        StartCoroutine(DelayOrderAction(Object_4, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_5, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_6, 0, 2f, 1f));

        yield break;
    }

    IEnumerator DelayAction(GameObject obj, bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) obj.SetActive(state);
    }

    // เพิ่ม startDelay เพื่อรอก่อนค่อยเปลี่ยน order
    IEnumerator DelayOrderAction(GameObject obj, int tempOrder, float duration, float startDelay)
    {
        if (obj == null) yield break;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            int originalOrder = sr.sortingOrder;

            // รอก่อนตามเวลาที่กำหนด
            if (startDelay > 0f)
                yield return new WaitForSeconds(startDelay);

            // เปลี่ยนเป็นค่าใหม่
            sr.sortingOrder = tempOrder;

            // คงค่าไว้ตามเวลาที่กำหนด
            yield return new WaitForSeconds(duration);

            // กลับเป็นค่าเดิม
            sr.sortingOrder = originalOrder;
        }
    }
}
