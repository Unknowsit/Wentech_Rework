using UnityEngine;

public class CannonAim : MonoBehaviour
{
    [SerializeField] private float min = 0f;
    [SerializeField] private float max = 90f;

    private Camera cam;
    private Vector2 mousePos
    {
        get
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            return pos;
        }
    }

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector2 dir = mousePos - (Vector2)transform.position;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        float clampedAngle = Mathf.Clamp(angle, min, max);
        transform.eulerAngles = new Vector3(0f, 0f, clampedAngle);
    }

    private void OnDrawGizmos()
    {
        if (cam == null) cam = Camera.main;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, mousePos);
    }
}