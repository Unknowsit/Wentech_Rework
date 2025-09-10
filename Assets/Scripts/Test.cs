using UnityEngine;

public class Test : MonoBehaviour
{
    private bool isDragging = false;

    private Camera cam;
    private Vector3 offset;

    private void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Start()
    {
         cam = Camera.main;
    }

    private void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;

            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.forward, GameManager.instance.rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.forward, -GameManager.instance.rotationSpeed * Time.deltaTime);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = transform.position.z - cam.transform.position.z;
        return cam.ScreenToWorldPoint(mouseScreenPos);
    }
}