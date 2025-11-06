using UnityEngine;

public class DraggableRotatable : MonoBehaviour
{
    private static DraggableRotatable currentlySelected = null;

    private bool isSelected = false;
    private Vector3 offset;

    private Camera cam;
    private GameManager gameManager;

    [Header("Input Settings")]
    [SerializeField] private float holdThreshold = 0.2f;

    private float mouseDownTime;
    private bool mouseHeld = false;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    private enum Mode { None, Rotate, Drag }
    private Mode currentMode = Mode.None;

    private void Start()
    {
        cam = Camera.main;
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        Vector3 mouseWorld = GetMouseWorldPosition();
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            if (Vector2.Distance(mousePos, transform.position) < 0.5f)
            {
                if (currentlySelected != null && currentlySelected != this)
                    return;

                if (!isSelected)
                    Select();

                mouseDownTime = Time.time;
                mouseHeld = true;
                offset = transform.position - mouseWorld;
            }
            else
            {
                if (isSelected)
                {
                    Deselect();
                }
            }
        }

        if (Input.GetMouseButton(0) && mouseHeld && isSelected)
        {
            float heldTime = Time.time - mouseDownTime;

            if (heldTime >= holdThreshold)
            {
                if (currentMode != Mode.Drag)
                {
                    EnterDragMode();
                }

                if (currentMode == Mode.Drag)
                {
                    transform.position = mouseWorld + offset;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && mouseHeld && isSelected)
        {
            float heldTime = Time.time - mouseDownTime;

            if (currentMode == Mode.Drag)
            {
                currentMode = Mode.None;
                Deselect();
            }
            else
            {
                if (heldTime < holdThreshold)
                {
                    if (currentMode != Mode.Rotate)
                    {
                        EnterRotateMode();
                    }
                }
            }

            mouseHeld = false;
        }

        if (currentMode == Mode.Rotate && isSelected)
        {
            RotateTowardMouse(mouseWorld);
        }
    }

    private void RotateTowardMouse(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void Select()
    {
        isSelected = true;
        currentlySelected = this;
        SetCannonActive(false);
        currentMode = Mode.None;
        UpdateSprite(true);
    }

    private void Deselect()
    {
        isSelected = false;
        currentlySelected = null;
        currentMode = Mode.None;
        SetCannonActive(true);
        UpdateSprite(false);
    }

    private void EnterRotateMode()
    {
        currentMode = Mode.Rotate;
    }

    private void EnterDragMode()
    {
        currentMode = Mode.Drag;
    }

    private void SetCannonActive(bool isActive)
    {
        gameManager.cannonAim.enabled = isActive;
        gameManager.cannonShooter.enabled = isActive;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = transform.position.z - cam.transform.position.z;
        return cam.ScreenToWorldPoint(mouseScreenPos);
    }

    private void UpdateSprite(bool selected)
    {
        if (selected)
            spriteRenderer.sprite = selectedSprite;
        else
            spriteRenderer.sprite = normalSprite;
    }
}