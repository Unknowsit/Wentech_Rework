using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Clock : MonoBehaviour
{
    [Header("Lock UI position")]
    public RectTransform child;        // รูปแท่งเสียง / กลุ่ม UI ที่ต้องการล็อก
    public GameObject targetObject;    // อันที่อยากให้โผล่หลังดีเลย์ (ถ้ามี)

    [Header("Link with volume slider")]
    public Slider volumeSlider;        // Slider ด้านบน
    public Image volumeImage;          // รูปรวมแท่งเสียง (อันเดียว)

    Vector2 fixedPos;
    bool canLock = false;

    void Awake()
    {
        // เก็บตำแหน่ง UI ตอนออกแบบ (ใช้ anchoredPosition แทน position)
        if (child != null)
            fixedPos = child.anchoredPosition;
    }

    void Start()
    {
        // ผูกกับ Slider ให้เวลาเลื่อนแล้วอัปเดตรูป
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
            OnSliderChanged(volumeSlider.value); // sync กับค่าที่โหลดมาจาก PlayerPrefs ตอนเริ่ม
        }
    }

    void LateUpdate()
    {
        // ถ้าต้องการล็อกตำแหน่ง
        if (canLock && child != null)
        {
            child.anchoredPosition = fixedPos;   // ใช้ anchoredPosition สำหรับ UI
        }
    }
    // ===========================
    // ปุ่ม 1: Lock position (มีดีเล 1 วิ)
    // ===========================
    public void OnButtonPressed()
    {
        StartCoroutine(DelayLock());
    }

    IEnumerator DelayLock()
    {
        yield return new WaitForSeconds(0.5f); // ดีเล 1 วิ
        canLock = true;                      // เริ่มล็อกตำแหน่ง

        // ถ้ายังต้องการดีเลก่อน Active ก็ทำแบบนี้
        yield return new WaitForSeconds(0.2f);
        targetObject.SetActive(true);
    }

    // ===========================
    // ปุ่ม 2: Unlock position
    // ===========================
    public void UnlockPosition()
    {
        canLock = false;         // ปลดล็อกตำแหน่ง
    }

    // ===========================
    // ให้รูปหาย/โผล่ตามค่า Slider
    // ===========================
    void OnSliderChanged(float value)
    {
        if (volumeImage != null)
        {
            // ถ้า slider ใช้ค่า 0–1 อยู่แล้วก็ใส่ตรง ๆ ได้เลย
            volumeImage.fillAmount = Mathf.Clamp01(value);

            // ถ้า slider ใช้ 0–100 ต้อง normalize ก่อน (ตัวอย่าง)
            // float normalized = Mathf.InverseLerp(0f, 100f, value);
            // volumeImage.fillAmount = normalized;
        }
    }
}
