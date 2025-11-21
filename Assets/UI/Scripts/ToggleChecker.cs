using UnityEngine;
using UnityEngine.UI;

public class ToggleChecker : MonoBehaviour
{
    public Toggle[] toggles;        // จำนวน toggle ที่ใช้เช็ค
    public GameObject objectA;      // เปิดจากปุ่ม
    public GameObject objectB;      // อัปเดต realtime

    // เรียกจาก Button
    public void ToggleObjectA()
    {
        objectA.SetActive(!objectA.activeSelf);
    }

    public void Delects()
    {
        Destroy(objectB);
    }

    void Update()
    {
        int count = 0;

        foreach (var t in toggles)
            if (t.isOn) count++;

        // objectB อัปเดตแบบ realtime
        objectB.SetActive(count > 1);
    }
}
