using UnityEngine;
using UnityEngine.UI;

public class ToggleCheckerButton : MonoBehaviour
{
    public Toggle[] toggles;        // ใส่ Toggle ที่ต้องตรวจ
    public GameObject targetObject; // Object ที่จะเปิด/ปิด

    // เรียกฟังก์ชันนี้จาก Button OnClick()
    public void CheckToggles()
    {
        int count = 0;

        foreach (var t in toggles)
        {
            if (t.isOn) count++;
        }

        if (count >= 2)
        {
            targetObject.SetActive(true);
        }
    }
}
