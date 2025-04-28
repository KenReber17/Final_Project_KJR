using UnityEngine;

public class SlidingPlatform : MonoBehaviour
{
    [SerializeField] private float slideDistance = 2f; // Distance to slide left and right from center
    [SerializeField] private float slideSpeed = 2f; // Speed of sliding (controls frequency of movement)
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // Easing for smooth slowdown at ends

    private Vector3 centerPosition; // Center position of the platform (initial position)
    private float timeCounter = 0f; // Tracks time for sine wave oscillation

    void Start()
    {
        // Store the initial position as the center
        centerPosition = transform.position;
        // Ensure Z remains constant (e.g., Z = 6)
        Debug.Log($"SlidingPlatform initialized at center: {centerPosition}");
    }

    void Update()
    {
        // Increment time counter for continuous movement
        timeCounter += Time.deltaTime * slideSpeed;

        // Use sine wave for oscillation (-1 to 1)
        float sineValue = Mathf.Sin(timeCounter);

        // Apply easing curve to slow down near ends
        float easedValue = easingCurve.Evaluate(Mathf.Abs(sineValue)) * Mathf.Sign(sineValue);

        // Calculate new X position, keeping Y and Z constant
        Vector3 newPosition = centerPosition + new Vector3(easedValue * slideDistance, 0f, 0f);
        transform.position = newPosition;
    }

    // Optional: Ensure player moves with the platform (for parenting)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Parent the player to the platform to move with it
            collision.transform.SetParent(transform);
            Debug.Log("Player landed on platform");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Unparent the player when leaving the platform
            collision.transform.SetParent(null);
            Debug.Log("Player left platform");
        }
    }

    // Draw gizmos to visualize slide range in the Scene view
    void OnDrawGizmos()
    {
        // Draw center position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        // Draw left and right endpoints
        Gizmos.color = Color.blue;
        Vector3 leftPos = transform.position - new Vector3(slideDistance, 0f, 0f);
        Vector3 rightPos = transform.position + new Vector3(slideDistance, 0f, 0f);
        Gizmos.DrawWireSphere(leftPos, 0.2f);
        Gizmos.DrawWireSphere(rightPos, 0.2f);
    }
}