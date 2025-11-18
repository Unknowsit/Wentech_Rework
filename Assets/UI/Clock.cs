using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour
{
    public Transform child;
    public GameObject targetObject;

    Vector3 fixedPos;
    bool canLock = false;

    void Start()
    {
        fixedPos = child.position; // เก็บตำแหน่งเริ่มต้น
    }

    void LateUpdate()
    {
        if (canLock)
        {
            child.position = fixedPos;  // ล็อกตำแหน่ง
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
}
