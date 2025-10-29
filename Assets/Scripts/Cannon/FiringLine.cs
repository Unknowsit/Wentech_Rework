using UnityEngine;

public class FiringLine : MonoBehaviour
{

    public LineRenderer lineOfSight;

    public int reflections;
    public float MaxRayDistance;
    public LayerMask LayerDetection;

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }

    private void Update()
    {
        lineOfSight.positionCount = 1;
        lineOfSight.SetPosition(0, transform.position);

        Vector2 direction = transform.right;
        Vector2 origin = transform.position;

        for (int i = 0; i < reflections; i++)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, MaxRayDistance, LayerDetection);
            lineOfSight.positionCount += 1;

            if (hitInfo.collider != null)
            {
                Vector2 hitPoint = hitInfo.point;
                lineOfSight.SetPosition(lineOfSight.positionCount - 1, hitPoint);

                // ————— Debug: วาด segment จริง (origin -> hitPoint)
                Debug.DrawLine(origin, hitPoint, Color.green);

                // ————— Debug: วาด normal ที่จุดชน (ยาวเล็กน้อย)
                Debug.DrawRay(hitPoint, hitInfo.normal * 0.5f, Color.red);

                if (hitInfo.collider.CompareTag("Box"))
                {
                    // คำนวณทิศทางสะท้อน
                    direction = Vector2.Reflect(direction, hitInfo.normal).normalized;

                    // ————— Debug: วาดทิศทางที่สะท้อนจากจุดชน (ยาวเล็กน้อย)
                    Debug.DrawRay(hitPoint, direction * 1.0f, Color.cyan);

                    // ตั้ง origin เล็กน้อยให้หลุดจาก collider เพื่อหลีกเลี่ยงการชนซ้ำ (epsilon shift)
                    origin = hitPoint + direction * 0.01f;
                }
                else
                {
                    // ถ้าไม่ใช่กระจกก็หยุด
                    break;
                }
            }
            else
            {
                // ไม่มีอะไรโดน: วาดจนสุดระยะ
                Vector2 endPoint = origin + direction * MaxRayDistance;
                lineOfSight.SetPosition(lineOfSight.positionCount - 1, endPoint);

                // ————— Debug: วาด segment สุดท้าย
                Debug.DrawLine(origin, endPoint, Color.red);
                break;
            }
        }
    }
}
