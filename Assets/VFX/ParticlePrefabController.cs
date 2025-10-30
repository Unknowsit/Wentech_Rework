using System.Collections;
using UnityEngine;

public class ParticlePrefabController : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private ParticleSystem oneShotPrefab;   // �Ϳ࿡��Ẻǧ/���Դ (����������)
    [SerializeField] private ParticleSystem trailPrefab;     // �Ϳ࿡��Ẻ�ҡ/�蹤�ҧ (����ҧ�����ҡ)

    [Header("Input (Mouse Buttons)")]
    [Tooltip("0=����, 1=���, 2=��ҧ")]
    [SerializeField] private int oneShotMouseButton = 0;
    [Tooltip("0=����, 1=���, 2=��ҧ")]
    [SerializeField] private int dragMouseButton = 1;

    [Header("Trail Emit (�ҡ/��ҧ)")]
    [Tooltip("�ӹǹ͹��Ҥ������зҧ 1 ˹��� (world space)")]
    [SerializeField] private float emitPerUnit = 20f;
    [Tooltip("�ѹ���: ��ͧ��Ѻ���ҧ������ҹ���͹�л����")]
    [SerializeField] private float minDistance = 0.01f;
    [Tooltip("��ǻ���µ�����˹�����������ҧ�ҡ")]
    [SerializeField] private bool followMouse = true;

    [Header("Auto-Config (��ͧ�ѹ��ҧ/��ش�ͧ)")]
    [Tooltip("maxParticles ����Ѻ�Ϳ࿡���ҡ �ҡ㹾��Ὼ����������Թ�")]
    [SerializeField] private int desiredMaxParticlesForTrail = 8000;
    [Tooltip("��� > 0 �кѧ�ѺŴ startLifetime Ẻ constant �������Թ��ҹ�� (����Ŵ�������)")]
    [SerializeField] private float trailLifetimeClamp = 1.25f;

    [Header("Parent (optional)")]
    [SerializeField] private Transform runtimeParent;

    private Camera cam;
    private ParticleSystem activeTrail;
    private Vector3 lastPos;
    private bool dragging;

    void Awake()
    {
        cam = Camera.main;
        if (!cam)
        {
            cam = FindAnyObjectByType<Camera>();
        }
    }

    void Update()
    {
        // ==================== One-Shot ====================
        if (oneShotPrefab && Input.GetMouseButtonDown(oneShotMouseButton))
        {
            Vector3 pos = MouseWorldAtPrefabZ(oneShotPrefab);
            var ps = Instantiate(oneShotPrefab, pos, oneShotPrefab.transform.rotation, runtimeParent);

            // ��駤��: ����ٻ + ��ش���� Destroy �ѵ��ѵ�
            ConfigureOneShotAutoDestroy(ps);

            ps.Clear(true);
            ps.Play(true);

            // ���͡ó� stopAction ���ӧҹ (sub-emitter �š �)
            StartCoroutine(DestroyWhenDone(ps));
        }

        // ==================== Begin Drag (Hold) ====================
        if (trailPrefab && Input.GetMouseButtonDown(dragMouseButton))
        {
            dragging = true;

            Vector3 pos = MouseWorldAtPrefabZ(trailPrefab);
            activeTrail = Instantiate(trailPrefab, pos, trailPrefab.transform.rotation, runtimeParent);

            // ��駤��: �Դ�ٻ, stopAction=None, ���� maxParticles, �ҨŴ lifetime
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
                // �Դ��û���� �������͹��Ҥ�������ͤ��� � �Ѻ
                var em = activeTrail.emission;
                em.enabled = false;

                // ���е�� loop=true + stopAction=None ����� StopEmitting ���ͻ����������ͧ
                activeTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                // ���������ʹѺ��� (safety)
                StartCoroutine(DestroyWhenDone(activeTrail));
                activeTrail = null;
            }
        }
    }

    // ---------- Config ����Ѻ One-Shot ----------
    void ConfigureOneShotAutoDestroy(ParticleSystem root)
    {
        if (!root) return;
        foreach (var ps in root.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.Destroy; // ��ش���Ƿ����
        }
    }

    // ---------- Config ����Ѻ Trail/Hold ----------
    void ConfigureTrailHold(ParticleSystem root, int desiredMaxParticles = 5000, float lifetimeClamp = -1f)
    {
        if (!root) return;

        foreach (var ps in root.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;

            // �Ӥѭ: ����к��������ʹ�͹�ҡ (�����������ͧ)
            main.loop = true;
            main.stopAction = ParticleSystemStopAction.None;

            // ������͹��Ҥ���������骹 MaxParticles ������ش�ͧ
            if (main.maxParticles < desiredMaxParticles)
                main.maxParticles = desiredMaxParticles;

            // Ŵ lifetime (੾�Сó� constant) �������������������
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
        if (!cam)
            return Vector3.zero;

        Vector3 m = Input.mousePosition;
        float z = Mathf.Abs(cam.transform.position.z - prefabOrInstance.transform.position.z);
        m.z = z;

        Vector3 world = cam.ScreenToWorldPoint(m);
        world.z = prefabOrInstance.transform.position.z; // ��͡ Z ���ç prefab
        return world;
    }

    // ---------- Safety Destroy ----------
    IEnumerator DestroyWhenDone(ParticleSystem ps)
    {
        if (!ps) yield break;

        // �Դ timeout ����� � �ҡ duration + �����٧�ش�ͧ�١ � + buffer
        float duration = ps.main.duration;
        float maxLife = 0f;

        foreach (var p in ps.GetComponentsInChildren<ParticleSystem>(true))
        {
            var m = p.main;
            float life;

            switch (m.startLifetime.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    life = m.startLifetime.constant;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    life = Mathf.Max(m.startLifetime.constantMin, m.startLifetime.constantMax);
                    break;
                default:
                    life = 5f; // ������Ͷ���� curve
                    break;
            }

            maxLife = Mathf.Max(maxLife, life);
        }

        float timeout = Time.time + duration + maxLife + 2f;

        while (ps && ps.IsAlive(true) && Time.time < timeout)
            yield return null;

        if (ps) Destroy(ps.gameObject);
    }
}