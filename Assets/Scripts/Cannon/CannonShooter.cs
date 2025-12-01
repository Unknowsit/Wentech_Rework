using UnityEngine;
using System.Collections;

public class CannonShooter : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform bulletSpawnPos;

    [Header("VFX Feedback")]
    [SerializeField] private ParticleSystem shootVFXPrefab;
    [SerializeField] private Transform vfxSpawnPos;
    [SerializeField] private bool vfxAutoDestroy = true;

    [Header("Component Reference")]
    [SerializeField] private Timer timer;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

#if UNITY_STANDALONE
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && !EventSystem.current.IsPointerOverGameObject())
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
#endif

    public void Shoot()
    {
        timer.StopWarningMusic();
#if UNITY_ANDROID
        UIManager.instance.ShootButton.SetActive(false);
#elif UNITY_STANDALONE || UNITY_EDITOR
        //Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 direction = mousePos - (Vector2)bulletSpawnPos.position;

        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        //if (hit.collider != null && hit.collider.CompareTag("Box"))
        //    return;
#endif
        Vector2 direction = (Vector2)transform.right;

        AudioManager.instance.PlaySFX("SFX08");
        GameManager.instance.timer.isCounting = false;
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
        bullet.Shoot(direction.normalized);
        enabled = false;

        PlayShootVFX(direction.normalized);
    }

    private void PlayShootVFX(Vector2 dir)
    {
        if (!shootVFXPrefab) return;

        Transform anchor = vfxSpawnPos;

        float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(z, Vector3.forward);

        ParticleSystem ps = Instantiate(shootVFXPrefab, anchor.position, rot, anchor);

        ps.transform.SetPositionAndRotation(anchor.position, rot);

        var all = ps.GetComponentsInChildren<ParticleSystem>(true);

        foreach (var p in all)
        {
            var m = p.main; m.loop = false; m.stopAction = ParticleSystemStopAction.Destroy;
        }

        ps.Clear(true);
        ps.Play(true);

        if (vfxAutoDestroy)
            StartCoroutine(DestroyWhenDone(ps));
    }

    private IEnumerator DestroyWhenDone(ParticleSystem ps)
    {
        if (!ps) yield break;

        float duration = ps.main.duration;
        float maxLife = 0f;

        foreach (var p in ps.GetComponentsInChildren<ParticleSystem>(true))
        {
            var m = p.main;
            float life = m.startLifetime.mode switch
            {
                ParticleSystemCurveMode.Constant => m.startLifetime.constant,
                ParticleSystemCurveMode.TwoConstants => Mathf.Max(m.startLifetime.constantMin, m.startLifetime.constantMax),
                _ => 3f
            };
            maxLife = Mathf.Max(maxLife, life);
        }
        float timeout = Time.time + duration + maxLife + 1.5f;

        while (ps && ps.IsAlive(true) && Time.time < timeout)
            yield return null;

        if (ps) Destroy(ps.gameObject);
    }
}