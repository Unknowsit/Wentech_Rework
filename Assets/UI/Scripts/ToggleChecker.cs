using UnityEngine;
using UnityEngine.UI;

public class ToggleChecker : MonoBehaviour
{
    public Toggle[] toggles;        // จำนวน toggle ที่ใช้เช็ค
    public GameObject objectA;      // เปิด/ปิดจากปุ่ม
    public GameObject objectB;      // อัปเดต realtime

    // เรียกจาก Button
    public void ToggleObjectA()
    {
        if (objectA != null)
            objectA.SetActive(!objectA.activeSelf);
    }

    // ปุ่มลบ Object B
    public void DeleteObjectB()
    {
        if (objectB != null)
        {
            Destroy(objectB);
            objectB = null;  // กัน error ใน Update
        }
    }

    void Update()
    {
        if (objectB == null) return; // objectB ถูกลบแล้ว ไม่ต้องทำต่อ

        int count = 0;
        foreach (var t in toggles)
        {
            if (t != null && t.isOn)
                count++;
        }

        // objectB อัปเดตแบบ realtime
        objectB.SetActive(count > 1);
    }
}
