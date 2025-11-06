using UnityEngine;
using UnityEngine.UI;

public class SwipeMenuVertical : MonoBehaviour
{
    public GameObject scrollbar; // ลิงก์ Scrollbar จาก Inspector
    private float scroll_pos = 0;
    private float[] pos;

    void Update()
    {
        // เก็บตำแหน่งของแต่ละ child (ระยะห่างเท่ากัน)
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);

        for (int i = 0; i < pos.Length; i++)
            pos[i] = distance * i;

        // รองรับทั้งเมาส์และมือถือ (แตะหน้าจอ)
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            // จัดตำแหน่งให้ล็อกอยู่กับไอเท็มใกล้สุดเมื่อปล่อยนิ้ว
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollbar.GetComponent<Scrollbar>().value =
                        Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }

        // ปรับขนาดให้ไอเท็มตรงกลางขยาย
        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                transform.GetChild(i).localScale =
                    Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);

                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        transform.GetChild(a).localScale =
                            Vector2.Lerp(transform.GetChild(a).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }
    }
}
