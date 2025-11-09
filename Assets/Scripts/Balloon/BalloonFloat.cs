using UnityEngine;

public class BalloonFloat : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatAmplitude = 0.3f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private bool randomizeStart = true;

    [Header("Horizontal Sway (Optional)")]
    [SerializeField] private bool enableSway = true;
    [SerializeField] private float swayAmplitude = 0.1f;
    [SerializeField] private float swaySpeed = 0.7f;

    [Header("Rotation (Optional)")]
    [SerializeField] private bool enableRotation = true;
    [SerializeField] private float rotationAmount = 5f;
    [SerializeField] private float rotationSpeed = 0.5f;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = transform.position;

        if (randomizeStart)
        {
            timeOffset = Random.Range(0f, 2f * Mathf.PI);
        }
    }

    private void Update()
    {
        float time = Time.time * floatSpeed + timeOffset;

        float yOffset = Mathf.Sin(time) * floatAmplitude;
        float xOffset = enableSway ? Mathf.Sin(time * swaySpeed) * swayAmplitude : 0f;

        Vector3 newPosition = startPosition + new Vector3(xOffset, yOffset, 0);
        transform.position = newPosition;

        if (enableRotation)
        {
            float rotation = Mathf.Sin(time * rotationSpeed) * rotationAmount;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }
}