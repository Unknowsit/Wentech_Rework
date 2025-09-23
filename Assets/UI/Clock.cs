using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    void Update()
    {
        DateTime time = DateTime.Now;
        float seconds = time.Second + time.Millisecond / 1000f;
        float angle = -seconds * 6f; 
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
