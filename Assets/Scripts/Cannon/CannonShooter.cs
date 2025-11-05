using UnityEngine;
using UnityEngine.EventSystems;

public class CannonShooter : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform bulletSpawnPos;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

#if UNITY_STANDALONE
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Shoot();
        }
    }
#endif

    public void Shoot()
    {
#if UNITY_ANDROID
        UIManager.instance.ShootButton.SetActive(false);
        Vector2 direction = (Vector2)transform.right;
#elif UNITY_STANDALONE || UNITY_EDITOR
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)bulletSpawnPos.position;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider != null && hit.collider.CompareTag("Box"))
            return;
#endif
        AudioManager.instance.PlaySFX("SFX01");
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
        bullet.Shoot(direction.normalized);
        enabled = false;
    }
}