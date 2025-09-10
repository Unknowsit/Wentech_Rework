using UnityEngine;

public class CannonShooter : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform bulletSpawnPos;

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
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.instance.PlaySFX("SFX01");
            Destroy(this);
            Vector2 direction = mousePos - (Vector2)bulletSpawnPos.position;
            Bullet bullet = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
            bullet.Shoot(direction.normalized);
        }
    }
}