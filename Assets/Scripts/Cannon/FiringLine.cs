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

        Vector2 mirrorHitPoint = Vector2.zero;
        Vector2 mirrorHitNormal = Vector2.zero;

        Vector2 direction = transform.right;
        Vector2 origin = transform.position;

        for (int i = 0; i < reflections; i++)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, MaxRayDistance, LayerDetection);

            lineOfSight.positionCount += 1;

            if (hitInfo.collider != null)
            {
                lineOfSight.SetPosition(lineOfSight.positionCount - 1, hitInfo.point);

                if (hitInfo.collider.CompareTag("Box"))
                {
                    //  ���ȷҧ����ԧ� Reflect �Ѻ normal ���ⴹ
                    direction = Vector2.Reflect(direction, hitInfo.normal);
                    origin = hitInfo.point;
                }
                else
                {
                    // ���������Ш�����ش
                    break;
                }
            }
            else
            {
                // ���������ⴹ �ԧ�ç�����ش����
                lineOfSight.SetPosition(lineOfSight.positionCount - 1, origin + direction * MaxRayDistance);
                break;
            }
        }
    }
}
