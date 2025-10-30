using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    private bool isMouseOver = false;
    private bool isDragging = false;

    private Camera cam;
    private Vector3 offset;

    private GameManager gameManager;

    private void Start()
    {
        cam = Camera.main;
        gameManager = GameManager.instance;
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }

        if (isMouseOver)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.forward, gameManager.rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.forward, -gameManager.rotationSpeed * Time.deltaTime);
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