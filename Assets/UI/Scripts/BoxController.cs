using UnityEngine;

public class BoxController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform startPoint; // จุดเริ่มต้น
    public Transform endPoint;   // จุดสิ้นสุด
    public float speed = 2f;

    private void Start()
    {
        transform.position = startPoint.position;
    }
    // Call this method once per frame
    private void Update()
    {
        // Move the box towards the end point
        transform.position = Vector2.MoveTowards(transform.position, endPoint.position, speed * Time.deltaTime);

        // If the box reaches the end point, teleport it back to the start point
        if (Vector2.Distance(transform.position, endPoint.position) < 0.01f)
        {
            transform.position = startPoint.position;
        }
    }

    // This method is called when this object's 2D collider enters a trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has an Animator
        Animator anim = other.GetComponent<Animator>();

        // If it does and the "Box" tag is correct, trigger the animation.
        // This assumes the script is on the Box and detects collision with the letter.
        if (anim != null)
        {
            anim.SetTrigger("HitBox");
        }
    }
}