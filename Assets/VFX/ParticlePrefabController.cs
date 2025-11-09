using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParticlePrefabController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ParticleSystem oneShotPrefab;   // เอฟเฟกต์แบบวง/ระเบิด (กดครั้งเดียว)
    [SerializeField] private ParticleSystem trailPrefab;     // เอฟเฟกต์แบบลาก/พ่นค้าง (กดค้างแล้วลาก)

    // ให้ UI เรียกเปลี่ยนพรีแฟบได้ตอนรัน
    public void SetOneShotPrefab(ParticleSystem prefab) => oneShotPrefab = prefab;
    public void SetTrailPrefab(ParticleSystem prefab) => trailPrefab = prefab;

    [Header("Input (Mouse Buttons)")]
    [Tooltip("0=ซ้าย, 1=ขวา, 2=กลาง")]
    [SerializeField] private int oneShotMouseButton = 0;
    [Tooltip("0=ซ้าย, 1=ขวา, 2=กลาง")]
    [SerializeField] private int dragMouseButton = 1;

    [Header("Trail Emit (ลาก/ค้าง)")]
    [Tooltip("จำนวนอนุภาคต่อระยะทาง 1 หน่วย (world space)")]
    [SerializeField] private float emitPerUnit = 20f;
    [Tooltip("กันสั่น: ต้องขยับอย่างน้อยเท่านี้ก่อนจะปล่อย")]
    [SerializeField] private float minDistance = 0.01f;
    [Tooltip("หัวปล่อยตามตำแหน่งเมาส์ระหว่างลาก")]
    [SerializeField] private bool followMouse = true;

    [Header("Auto-Config (ป้องกันค้าง/หยุดเอง)")]
    [Tooltip("maxParticles สำหรับเอฟเฟกต์ลาก หากในพรีแฟบตั้งไว้น้อยเกินไป")]
    [SerializeField] private int desiredMaxParticlesForTrail = 8000;
    [Tooltip("ถ้า > 0 จะบังคับลด startLifetime แบบ constant ให้ไม่เกินค่านี้ (ช่วยลดการสะสม)")]
    [SerializeField] private float trailLifetimeClamp = 1.25f;

    [Header("Parent (optional)")]
    [SerializeField] private Transform runtimeParent;

    [Header("Camera (optional)")]
    [Tooltip("ถ้ากำหนดไว้ จะใช้กล้องนี้แทน Camera.main (แก้เคสหลายกล้อง/ไม่มี Main tag)")]
    [SerializeField] private Camera camOverride;

    private Camera camCached;           // กล้องที่ใช้จริง (รีเฟรชเมื่อเปลี่ยนซีน)
    private ParticleSystem activeTrail;
    private Vector3 lastPos;
    private bool dragging;

    public static ParticlePrefabController instance;

    // ==================== Singleton + Survive Scenes ====================
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene prev, Scene next)
    {
        // ซีนใหม่ → เคลียร์แคชกล้อง ให้หาใหม่รอบต่อไป
        camCached = null;
    }

    // ดึงกล้องที่ถูกต้องทุกครั้ง (มี override มาก่อน, ตามด้วย main, สุดท้ายหาอะไรก็ได้)
    Camera GetCam()
    {
        if (camOverride) return camOverride;
        if (camCached && camCached.isActiveAndEnabled) return camCached;

        camCached = Camera.main;
        if (!camCached) camCached = Object.FindFirstObjectByType<Camera>();
        return camCached;
    }

    // ==================== Update ====================
    void Update()
    {
        // ==================== One-Shot ====================
        if (oneShotPrefab && Input.GetMouseButtonDown(oneShotMouseButton))
        {
            Vector3 pos = MouseWorldAtPrefabZ(oneShotPrefab);
            var ps = Instantiate(oneShotPrefab, pos, oneShotPrefab.transform.rotation, runtimeParent);

            // ตั้งค่า: ไม่ลูป + หยุดแล้ว Destroy อัตโนมัติ
            ConfigureOneShotAutoDestroy(ps);

            ps.Clear(true);
            ps.Play(true);

            // เผื่อกรณี stopAction ไม่ทำงาน (sub-emitter แปลก ๆ)
            StartCoroutine(DestroyWhenDone(ps));
        }

        // ==================== Begin Drag (Hold) ====================
        if (trailPrefab && Input.GetMouseButtonDown(dragMouseButton))
        {
            dragging = true;

            Vector3 pos = MouseWorldAtPrefabZ(trailPrefab);
            activeTrail = Instantiate(trailPrefab, pos, trailPrefab.transform.rotation, runtimeParent);

            // ตั้งค่า: เปิดลูป, stopAction=None, เพิ่ม maxParticles, อาจลด lifetime
            ConfigureTrailHold(activeTrail, desiredMaxParticlesForTrail, trailLifetimeClamp);

            var em = activeTrail.emission;
            em.enabled = true;

            activeTrail.Play(true);
            lastPos = pos;
        }

        // ==================== Dragging ====================
        if (dragging && Input.GetMouseButton(dragMouseButton) && activeTrail)
        {
            Vector3 current = MouseWorldAtPrefabZ(activeTrail);

            if (followMouse)
                activeTrail.transform.position = current;

            float dist = Vector3.Distance(current, lastPos);
            if (dist >= minDistance)
            {
                int count = Mathf.FloorToInt(dist * emitPerUnit);
                if (count > 0)
                {
                    var ep = new ParticleSystem.EmitParams();
                    activeTrail.Emit(ep, count);
                    lastPos = current;
                }
            }
        }

        // ==================== End Drag ====================
        if (dragging && Input.GetMouseButtonUp(dragMouseButton))
        {
            dragging = false;

            if (activeTrail)
            {
                // ปิดการปล่อย แล้วให้อนุภาคที่เหลือค่อย ๆ ดับ
                var em = activeTrail.emission;
                em.enabled = false;

                // ตั้ง loop=true + stopAction=None → ใช้ StopEmitting
                activeTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                // ทำลายเมื่อดับหมด (safety)
                StartCoroutine(DestroyWhenDone(activeTrail));
                activeTrail = null;
            }
        }
    }

    // ---------- Config สำหรับ One-Shot ----------
    void ConfigureOneShotAutoDestroy(ParticleSystem root)
    {
        if (!root) return;
        foreach (var ps in root.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Destroy; // หยุดแล้วทำลาย
        }
    }

    // ---------- Config สำหรับ Trail/Hold ----------
    void ConfigureTrailHold(ParticleSystem root, int desiredMaxParticles = 5000, float lifetimeClamp = -1f)
    {
        if (!root) return;

        foreach (var ps in root.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;

            // สำคัญ: ให้ระบบอยู่ได้ตลอดตอนลาก (ไม่หมดอายุเอง)
            main.loop = true;
            main.stopAction = ParticleSystemStopAction.None;

            // เพิ่มงบอนุภาคเพื่อไม่ให้ชน MaxParticles แล้วหยุดเอง
            if (main.maxParticles < desiredMaxParticles)
                main.maxParticles = desiredMaxParticles;

            // ลด lifetime (เฉพาะกรณี constant) เพื่อไม่ให้สะสมจนเต็ม
            if (lifetimeClamp > 0f && main.startLifetime.mode == ParticleSystemCurveMode.Constant)
            {
                if (main.startLifetime.constant > lifetimeClamp)
                    main.startLifetime = lifetimeClamp;
            }
        }
    }

    // ---------- Mouse to World ----------
    Vector3 MouseWorldAtPrefabZ(ParticleSystem prefabOrInstance)
    {
        var cam = GetCam();
        if (!cam) return Vector3.zero; // กันพังชั่วคราวถ้าไม่มีกล้อง

        Vector3 m = Input.mousePosition;
        float z = Mathf.Abs(cam.transform.position.z - prefabOrInstance.transform.position.z);
        m.z = z;

        Vector3 world = cam.ScreenToWorldPoint(m);
        world.z = prefabOrInstance.transform.position.z; // ล็อก Z ให้ตรง prefab/อินสแตนซ์
        return world;
    }

    // ---------- Safety Destroy ----------
    IEnumerator DestroyWhenDone(ParticleSystem ps)
    {
        if (!ps) yield break;

        // คิด timeout คร่าว ๆ จาก duration + อายุสูงสุดของลูก ๆ + buffer
        float duration = ps.main.duration;
        float maxLife = 0f;

        foreach (var p in ps.GetComponentsInChildren<ParticleSystem>(true))
        {
            var m = p.main;
            float life;
            switch (m.startLifetime.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    life = m.startLifetime.constant; break;
                case ParticleSystemCurveMode.TwoConstants:
                    life = Mathf.Max(m.startLifetime.constantMin, m.startLifetime.constantMax); break;
                default:
                    life = 5f; break; // ค่าเผื่อถ้าเป็น curve
            }
            maxLife = Mathf.Max(maxLife, life);
        }

        float timeout = Time.time + duration + maxLife + 2f;

        while (ps && ps.IsAlive(true) && Time.time < timeout)
            yield return null;

        if (ps) Destroy(ps.gameObject);
    }
}
