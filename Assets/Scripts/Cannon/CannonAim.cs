using UnityEngine;
using UnityEngine.EventSystems;

public class CannonAim : MonoBehaviour
{
    [SerializeField] private float min = 0f;
    [SerializeField] private float max = 90f;

    private Camera cam;
    private Vector2 inputPos;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
#if UNITY_STANDALONE
        inputPos = Input.mousePosition;
#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                inputPos = touch.position;

                Ray ray = cam.ScreenPointToRay(inputPos);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider != null && hit.collider.CompareTag("Box")) return;
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
#endif
        Vector2 worldPos = cam.ScreenToWorldPoint(inputPos);
        Vector2 localDir = transform.parent.InverseTransformPoint(worldPos) - transform.localPosition;

        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(angle, min, max);
        transform.localEulerAngles = new Vector3(0f, 0f, clampedAngle);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (cam == null) cam = Camera.main;
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, worldPos);
    }
#endif
}