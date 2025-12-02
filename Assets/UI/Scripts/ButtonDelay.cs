using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ButtonDelay : MonoBehaviour
{
    public Toggle toggle;          // ตัว Toggle
    public GameObject targetObject; // Object ที่จะเปิด/ปิด

    void Start()
    {
        // ตั้งค่าการฟัง Event เวลา Toggle ถูกกด
        toggle.onValueChanged.AddListener(OnToggleChanged);

        // อัปเดตสถานะตั้งแต่เริ่ม
        OnToggleChanged(toggle.isOn);
    }

    void OnToggleChanged(bool isOn)
    {
        if (targetObject != null)
            targetObject.SetActive(isOn);  // true = เปิด, false = ปิด
    }
}
