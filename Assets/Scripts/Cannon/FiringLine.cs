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

                // ����� Debug: �Ҵ segment ��ԧ (origin -> hitPoint)
                Debug.DrawLine(origin, hitPoint, Color.green);

                // ����� Debug: �Ҵ normal ���ش�� (�����硹���)
                Debug.DrawRay(hitPoint, hitInfo.normal * 0.5f, Color.red);

                if (hitInfo.collider.CompareTag("Box"))
                {
                    // �ӹǳ��ȷҧ�з�͹
                    direction = Vector2.Reflect(direction, hitInfo.normal).normalized;

                    // ����� Debug: �Ҵ��ȷҧ����з�͹�ҡ�ش�� (�����硹���)
                    Debug.DrawRay(hitPoint, direction * 1.0f, Color.cyan);

                    // ��� origin ��硹��������ش�ҡ collider ������ա����§��ê���� (epsilon shift)
                    origin = hitPoint + direction * 0.01f;
                }
                else
                {
                    // ���������Ш�����ش
                    break;
                }
            }
            else
            {
                // ���������ⴹ: �Ҵ���ش����
                Vector2 endPoint = origin + direction * MaxRayDistance;
                lineOfSight.SetPosition(lineOfSight.positionCount - 1, endPoint);

                // ����� Debug: �Ҵ segment �ش����
                Debug.DrawLine(origin, endPoint, Color.red);
                break;
            }
        }
    }
}
