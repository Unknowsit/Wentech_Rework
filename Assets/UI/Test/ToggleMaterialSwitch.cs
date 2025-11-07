using UnityEngine;
using UnityEngine.UI;

public class ToggleMaterialColor : MonoBehaviour
{
    public Toggle toggle;         // ตัว Toggle
    public Image targetImage;     // Image ที่จะเปลี่ยน Material และสี
    public Material neonMaterial; // Material Neon
    private Material originalMaterial;
    private Color originalColor;  // เก็บสีเดิมไว้

    void Start()
    {
        originalMaterial = targetImage.material; // เก็บ Material เดิมไว้
        originalColor = targetImage.color;       // เก็บสีเดิมไว้
        toggle.onValueChanged.AddListener(OnToggleChanged);
        OnToggleChanged(toggle.isOn); // อัปเดตตอนเริ่ม
    }

    void OnToggleChanged(bool isOn)
    {
        targetImage.material = isOn ? neonMaterial : originalMaterial;
        targetImage.color = isOn ? new Color32(0xAE, 0xF5, 0xFF, 0xFF) : originalColor;
    }
}