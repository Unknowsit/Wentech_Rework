using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class VFXSettingsUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] ParticlePrefabController controller; // ปล่อยว่างได้ จะหาให้อัตโนมัติ
    [SerializeField] TMP_Dropdown oneShotDropdown;
    [SerializeField] TMP_Dropdown trailDropdown;

    [Header("Choices")]
    [SerializeField] List<ParticleSystem> oneShotOptions;
    [SerializeField] List<ParticleSystem> trailOptions;

    private AudioManager audioManager;

    const string KEY_ONESHOT = "vfx.oneshot.index";
    const string KEY_TRAIL = "vfx.trail.index";

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene a, Scene b)
    {
        // ซีนเปลี่ยน → หา controller ใหม่
        ResolveController();
    }

    void Awake()
    {
        audioManager = AudioManager.instance;
        ResolveController();
    }

    IEnumerator Start()
    {
        // รอ 1 เฟรมให้ของ DontDestroy สร้างตัวเองก่อน
        yield return new WaitForEndOfFrame();   
        InitDropdowns();
    }

    void ResolveController()
    {
        if (controller) return;
        controller = ParticlePrefabController.instance
                 ? ParticlePrefabController.instance
                 : FindFirstObjectByType<ParticlePrefabController>();
    }

    void InitDropdowns()
    {
        if (!oneShotDropdown || !trailDropdown) return;

        oneShotDropdown.ClearOptions();
        oneShotDropdown.AddOptions(oneShotOptions.ConvertAll(p => p ? p.name : "(null)"));

        trailDropdown.ClearOptions();
        trailDropdown.AddOptions(trailOptions.ConvertAll(p => p ? p.name : "(null)"));

        int idxOne = Mathf.Clamp(PlayerPrefs.GetInt(KEY_ONESHOT, 0), 0, Mathf.Max(0, oneShotOptions.Count - 1));
        int idxTrl = Mathf.Clamp(PlayerPrefs.GetInt(KEY_TRAIL, 0), 0, Mathf.Max(0, trailOptions.Count - 1));

        oneShotDropdown.SetValueWithoutNotify(idxOne);
        trailDropdown.SetValueWithoutNotify(idxTrl);

        // เซ็ตให้คอนโทรลเลอร์ (ถ้ายังหาไม่ได้ ให้ข้ามไป ไม่ต้อง NRE)
        ApplyOneShot(idxOne);
        ApplyTrail(idxTrl);

        oneShotDropdown.onValueChanged.AddListener(ApplyOneShot);
        trailDropdown.onValueChanged.AddListener(ApplyTrail);
    }

    public void ApplyOneShot(int index)
    {
        // กัน index/ตัวเลือก/คอนโทรลเลอร์ว่าง
        if (index < 0 || index >= oneShotOptions.Count) return;
        var prefab = oneShotOptions[index];
        if (controller) controller.SetOneShotPrefab(prefab);
        PlayerPrefs.SetInt(KEY_ONESHOT, index);
        //audioManager.PlaySFX("SFX04");
    }

    public void ApplyTrail(int index)
    {
        if (index < 0 || index >= trailOptions.Count) return;
        var prefab = trailOptions[index];
        if (controller) controller.SetTrailPrefab(prefab);
        PlayerPrefs.SetInt(KEY_TRAIL, index);
        //audioManager.PlaySFX("SFX04");
    }
}