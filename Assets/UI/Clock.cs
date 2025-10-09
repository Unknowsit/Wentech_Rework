using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    public float rotationSpeed = 100f; // ความเร็วในการหมุน (องศาต่อวินาที)

    void Update()
    {
        // หมุนไปตามเข็มนาฬิกา
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}
