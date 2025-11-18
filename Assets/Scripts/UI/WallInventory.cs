using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WallInventory : MonoBehaviour, IPointerDownHandler
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxWalls = 3;
    private int currentWallCount = 3;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0.3f);

    [Header("Wall Prefab")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private Transform wallParent;

    [Header("Ghost Preview")]
    [SerializeField] private SpriteRenderer ghostSprite;
    [SerializeField] private Color ghostColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] private Color validColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1, 0, 0, 0.5f);

    [Header("Placement Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private Vector2 placementAreaMin = new Vector2(-5, -3);
    [SerializeField] private Vector2 placementAreaMax = new Vector2(5, 3);
    [SerializeField] private LayerMask obstacleLayer;

    private bool isDragging = false;

    private Camera cam;
    private GameObject currentGhost;
    private AudioManager audioManager;

    private List<GameObject> placedWalls = new List<GameObject>();

    private void Start()
    {
        cam = Camera.main;
        audioManager = AudioManager.instance;
        UpdateUI();
    }

    private void Update()
    {
        if (isDragging)
        {
            UpdateGhostPosition();

#if UNITY_STANDALONE
            if (Input.GetMouseButtonUp(0))
            {
                TryPlaceWall();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
#elif UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended)
                {
                    TryPlaceWall();
                }
            }
#endif
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentWallCount > 0 && !isDragging)
        {
            StartDragging();
        }
        else if (currentWallCount <= 0)
        {
            audioManager.PlaySFX("SFX_Error");
        }
    }

    private void StartDragging()
    {
        isDragging = true;
        currentGhost = Instantiate(wallPrefab);
        SetupGhost(currentGhost);
        audioManager.PlaySFX("SFX05");
    }

    private void SetupGhost(GameObject ghost)
    {
        foreach (var col in ghost.GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        SpriteRenderer[] sprites = ghost.GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in sprites)
        {
            Color color = sr.color;
            color.a = ghostColor.a;
            sr.color = color;
            ghostSprite = sr;
        }
    }

    private void UpdateGhostPosition()
    {
        if (currentGhost == null) return;

#if UNITY_STANDALONE
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID
        if (Input.touchCount == 0) return;
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
#else
        Vector3 mousePos = Vector3.zero;
#endif

        mousePos.z = 0;
        Vector3 snappedPos = new Vector3(Mathf.Round(mousePos.x / gridSize) * gridSize, Mathf.Round(mousePos.y / gridSize) * gridSize, 0);
        currentGhost.transform.position = snappedPos;

        bool isValid = IsValidPlacement(snappedPos);
        UpdateGhostColor(isValid);
    }

    private bool IsValidPlacement(Vector3 position)
    {
        if (position.x < placementAreaMin.x || position.x > placementAreaMax.x || position.y < placementAreaMin.y || position.y > placementAreaMax.y)
        {
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.4f, obstacleLayer);
        return colliders.Length == 0;
    }

    private void UpdateGhostColor(bool isValid)
    {
        Color color = isValid ? validColor : invalidColor;
        ghostSprite.color = color;
    }

    private void TryPlaceWall()
    {
        if (currentGhost == null)
        {
            isDragging = false;
            return;
        }

        Vector3 position = currentGhost.transform.position;

        if (IsValidPlacement(position))
        {
            GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity, wallParent);
            placedWalls.Add(wall);
            currentWallCount--;
            UpdateUI();
            audioManager.PlaySFX("SFX04");
        }
        else
        {
            audioManager.PlaySFX("SFX_Error");
        }

        Destroy(currentGhost);
        isDragging = false;
    }

    private void CancelPlacement()
    {
        Destroy(currentGhost);
        isDragging = false;
        audioManager.PlaySFX("SFX_Cancel");
    }

    private void UpdateUI()
    {
        countText.text = $"{currentWallCount}/{maxWalls}";
        iconImage.color = currentWallCount > 0 ? normalColor : emptyColor;
    }

    public void ResetWalls()
    {
        DestroyAllPlacedWalls();
        currentWallCount = maxWalls;
        UpdateUI();
    }

    private void DestroyAllPlacedWalls()
    {
        foreach (GameObject wall in placedWalls)
        {
            if (wall != null)
            {
                Destroy(wall);
            }
        }

        placedWalls.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 min = new Vector3(placementAreaMin.x, placementAreaMin.y, 0);
        Vector3 max = new Vector3(placementAreaMax.x, placementAreaMax.y, 0);
        Vector3 size = max - min;
        Vector3 center = (min + max) / 2;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}