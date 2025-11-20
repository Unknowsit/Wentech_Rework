using UnityEngine;
using UnityEngine.UI;

public class ToggleMaterialColor : MonoBehaviour
{
    public Toggle toggle;
    public Image targetImage;
    public Material neonMaterial;
    private Material originalMaterial;
    private Color originalColor;

    private bool lastState;

    void Start()
    {
        originalMaterial = targetImage.material;
        originalColor = targetImage.color;
        lastState = toggle.isOn;

        ApplyState(toggle.isOn);
    }

    void Update()
    {
        if (toggle.isOn != lastState)
        {
            lastState = toggle.isOn;
            ApplyState(toggle.isOn);
        }
    }

    void ApplyState(bool isOn)
    {
        targetImage.material = isOn ? neonMaterial : originalMaterial;
        targetImage.color = isOn
            ? new Color32(223, 255, 108, 255)
            : originalColor;
    }
}
