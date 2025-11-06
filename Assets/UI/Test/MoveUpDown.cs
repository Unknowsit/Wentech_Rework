using UnityEngine;

public class FloatMovement : MonoBehaviour
{
    public float amplitude = 1f;     // ระยะทางที่ขึ้น-ลง
    public float speed = 2f;         // ความเร็วในการแกว่ง
    private float offset;            // ค่าเลื่อนเวลาแต่ละ object
    private float startY;            // ตำแหน่งเริ่มต้น

    void Start()
    {
        startY = transform.position.y;
        offset = Random.Range(0f, Mathf.PI * 2f); // สุ่ม phase offset ไม่ให้แกว่งพร้อมกัน
    }

    void Update()
    {
        float newY = startY + Mathf.Sin(Time.time * speed + offset) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
