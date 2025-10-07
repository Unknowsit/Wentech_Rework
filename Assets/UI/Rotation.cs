using UnityEngine;

public class Rotation : MonoBehaviour
{
    public AudioManager audioManager;
    public float rotationAngle = 90f;  // มุมหมุน
    public float rotationSpeed = 200f; // ความเร็วหมุน (องศาต่อวินาที)

    private bool isRotating = false;
    private float targetAngle;


    private void Awake()
    {
         audioManager = AudioManager.instance;
    }
    // เรียกเมื่อกดปุ่มหมุน +90
    public void RotatePlus90()
    {
        if (!isRotating)
        {
            targetAngle = transform.eulerAngles.z + rotationAngle;
            StartCoroutine(RotateTo(targetAngle));
            audioManager.PlaySFX("SFX05");
        }
    }

    // เรียกเมื่อกดปุ่มหมุน -90
    public void RotateMinus90()
    {
        if (!isRotating)
        {
            targetAngle = transform.eulerAngles.z - rotationAngle;
            StartCoroutine(RotateTo(targetAngle));
            audioManager.PlaySFX("SFX05");
        }
    }

    System.Collections.IEnumerator RotateTo(float target)
    {
        isRotating = true;

        float startAngle = transform.eulerAngles.z;
        float currentAngle = startAngle;

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, target)) > 0.1f)
        {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, target, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, target);
        isRotating = false;
    }



}
