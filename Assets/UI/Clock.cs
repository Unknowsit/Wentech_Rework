using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Clock : MonoBehaviour
{
    public float rotationSpeed = 100f; // ความเร็วในการหมุน (องศาต่อวินาที)

    void Update()
    {
        // หมุนไปตามเข็มนาฬิกา
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }

    public void ChangeScene(string Mainmenu)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
