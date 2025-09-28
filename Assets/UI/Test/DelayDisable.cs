using UnityEngine;
using System.Collections;

public class DelayDisable : MonoBehaviour
{
    public GameObject Object_1;
    public GameObject Object_2;
    public GameObject Object_3;
    public GameObject Object_32;
    public GameObject Object_4;
    public GameObject Object_5;
    public GameObject Object_6;
    public GameObject Object_7;
    public GameObject Object_8;
    public GameObject Object_9;
    public GameObject Object_10;
    public GameObject Object_11;

    public GameObject Object_12;
    public GameObject Object_13;
    public GameObject Object_14;
    public GameObject Object_15;

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
        StartCoroutine(DelayOrderAction(Object_2, 0, 2f, 0f));

        yield break;
    }


    public void Delay4left()
    {
        StartCoroutine(DelayObject4_Left());
    }

    IEnumerator DelayObject4_Left()
    {
        // ปิด Object_1 หลัง 1.1 วิ
        StartCoroutine(DelayAction(Object_32, false, 1.1f));

        // เปิด Object_1 หลัง 1 วิ
        StartCoroutine(DelayAction(Object_3, true, 1f));

        // ✅ ให้ Object 4,5,6 หน่วง 1 วิ ก่อน แล้วค่อยเปลี่ยน order เป็น 0 คงไว้ 2 วิ
        StartCoroutine(DelayOrderAction(Object_12, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_13, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_14, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_15, 0, 2f, 1f));

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
        StartCoroutine(DelayOrderAction(Object_7, 0, 2f, 1f));

        yield break;
    }

    public void Delay3Left()
    {
        StartCoroutine(DelayObject3_Left());
    }

    IEnumerator DelayObject3_Left()
    {
        // ปิด Object_2 หลัง 1.1 วิ
        StartCoroutine(DelayAction(Object_3, false, 1.1f));

        // เปิด Object_1 หลัง 1 วิ
        StartCoroutine(DelayAction(Object_2, true, 1f));

        // ✅ ให้ Object 4,5,6 หน่วง 1 วิ ก่อน แล้วค่อยเปลี่ยน order เป็น 0 คงไว้ 2 วิ
        StartCoroutine(DelayOrderAction(Object_8, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_9, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_10, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_11, 0, 2f, 1f));

        yield break;
    }

    public void Delay2Right()
    {
        StartCoroutine(DelayObject2_Right());
    }

    IEnumerator DelayObject2_Right()
    {
        // ปิด Object_2 หลัง 1.1 วิ
        StartCoroutine(DelayAction(Object_2, false, 1.1f));

        // เปิด Object_1 หลัง 1 วิ
        StartCoroutine(DelayAction(Object_3, true, 1f));

        // ✅ ให้ Object 4,5,6 หน่วง 1 วิ ก่อน แล้วค่อยเปลี่ยน order เป็น 0 คงไว้ 2 วิ
        StartCoroutine(DelayOrderAction(Object_4, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_5, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_6, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_7, 0, 2f, 1f));

        yield break;
    }

    public void Delay3Right()
    {
        StartCoroutine(DelayObject3_Right());
    }

    IEnumerator DelayObject3_Right()
    {
        // ปิด Object_2 หลัง 1.1 วิ
        StartCoroutine(DelayAction(Object_3, false, 1.1f));

        // เปิด Object_1 หลัง 1 วิ
        StartCoroutine(DelayAction(Object_32, true, 1f));

        // ✅ ให้ Object 4,5,6 หน่วง 1 วิ ก่อน แล้วค่อยเปลี่ยน order เป็น 0 คงไว้ 2 วิ
        StartCoroutine(DelayOrderAction(Object_8, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_9, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_10, 0, 2f, 1f));
        StartCoroutine(DelayOrderAction(Object_11, 0, 2f, 1f));

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
