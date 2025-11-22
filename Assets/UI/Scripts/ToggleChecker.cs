using UnityEngine;
using UnityEngine.UI;

public class ToggleChecker : MonoBehaviour
{
    public Toggle[] toggles;        // จำนวน toggle ที่ใช้เช็ค
    public GameObject objectA;      // เปิด/ปิดจากปุ่ม
    public GameObject objectB;      // อัปเดต realtime (count > 1)
    public GameObject objectC;      // เปิดเมื่อ count == 1

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
    public void DeleteObjectC()
    {
        if (objectC != null)
        {
            Destroy(objectC);
            objectC = null;  // กัน error ใน Update
        }
    }

    void Update()
    {
        int count = 0;
        foreach (var t in toggles)
        {
            if (t != null && t.isOn)
                count++;
        }

        // --------------------------
        // ObjectB: เปิดเมื่อ count > 1
        // --------------------------
        if (objectB != null)
            objectB.SetActive(count > 1);

        // --------------------------
        // ObjectC: เปิดเมื่อ count == 1
        // ปิดเมื่อ count != 1 (รวม 0 และ 2)
        // --------------------------
        if (objectC != null)
            objectC.SetActive(count == 1);
    }
}
