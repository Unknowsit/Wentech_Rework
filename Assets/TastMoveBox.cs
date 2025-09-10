using UnityEngine;

public class TastMoveBox : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform startPoint; // �ش�������
    public Transform endPoint;   // �ش����ش
    public float speed = 2f;

    private Transform target; // �ش�����ѧ�����

    private void Start()
    {   // �ҧ���ش������鹵͹�������
        transform.position = startPoint.position;
    }

    private void Update()
    {
        // ����͹���仢�ҧ˹��
        transform.position = Vector2.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);

        // ���件֧ endPoint  ���컡�Ѻ� startPoint
        if (Vector2.Distance(transform.position, endPoint.position) < 0.01f)
        {
            transform.position = startPoint.position;
        }
    }
}
