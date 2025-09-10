using UnityEngine;

public class TastMoveBox : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform startPoint; // จุดเริ่มต้น
    public Transform endPoint;   // จุดสิ้นสุด
    public float speed = 2f;

    private Transform target; // จุดที่กำลังจะไปหา

    private void Start()
    {   // วางที่จุดเริ่มต้นตอนเริ่มเกม
        transform.position = startPoint.position;
    }

    private void Update()
    {
        // เคลื่อนที่ไปข้างหน้า
        transform.position = Vector2.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);

        // ถ้าไปถึง endPoint  วาร์ปกลับไป startPoint
        if (Vector2.Distance(transform.position, endPoint.position) < 0.01f)
        {
            transform.position = startPoint.position;
        }
    }
}
