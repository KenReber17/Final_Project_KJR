using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour
{
    public enum SlideDirection { Left, Right }
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Right; // Direction to slide (Left or Right)
    [SerializeField] private float slideDistance = 2f; // Distance to slide when opening
    [SerializeField] private float slideSpeed = 2f; // Speed of sliding animation
    [SerializeField] private AudioSource doorAudioSource; // AudioSource for door sound
    [SerializeField] private bool closeOnStart = true; // Checkbox to close door on start after 0.5s delay

    private Vector3 closedPosition; // Door's closed position at Z = 6
    private Vector3 openPosition; // Door's open position at Z = 6
    private bool isOpen = true; // Start with door open
    private bool isPlayerInTrigger = false; // Tracks if player is in the trigger zone

    void Start()
    {
        // Set initial state to open
        openPosition = transform.position; // Current position is open
        // Calculate closed position based on slide direction and distance
        float direction = slideDirection == SlideDirection.Right ? 1f : -1f;
        closedPosition = openPosition - new Vector3(slideDistance * direction, 0f, 0f);
        // Validate AudioSource
        if (doorAudioSource == null)
        {
            Debug.LogWarning("DoorAudioSource is not assigned on " + gameObject.name);
        }
        else if (doorAudioSource.clip == null)
        {
            Debug.LogWarning("DoorAudioSource has no AudioClip assigned on " + gameObject.name);
        }
        else
        {
            Debug.Log("DoorAudioSource initialized with clip: " + doorAudioSource.clip.name);
        }

        // If closeOnStart is checked, close the door after 0.5s delay
        if (closeOnStart)
        {
            StartCoroutine(CloseOnStart());
        }
    }

    // Coroutine to close the door after 0.5s delay
    private IEnumerator CloseOnStart()
    {
        yield return new WaitForSeconds(0.5f);
        ToggleDoor(); // Close the door
        Debug.Log("Door automatically closed after 0.5s delay on " + gameObject.name);
    }

    void Update()
    {
        // Check for "E" key press when player is in trigger zone
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            ToggleDoor();
        }

        // Smoothly move the door to its target position, preserving Z = 6
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, slideSpeed * Time.deltaTime);
    }

    // Toggle the door between open and closed states
    private void ToggleDoor()
    {
        isOpen = !isOpen;
        // Play the door sound if AudioSource and clip are assigned
        if (doorAudioSource != null && doorAudioSource.clip != null)
        {
            doorAudioSource.Play();
            Debug.Log("Playing door sound: " + doorAudioSource.clip.name);
        }
        else
        {
            Debug.LogWarning("Cannot play door sound: AudioSource or AudioClip is missing");
        }
        Debug.Log($"Door is now {(isOpen ? "open" : "closed")}");
    }

    // Detect when player enters the trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            Debug.Log("Player entered door trigger zone on " + gameObject.name);
        }
    }

    // Detect when player exits the trigger zone
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("Player left door trigger zone on " + gameObject.name);
        }
    }

    // Draw gizmos to visualize closed and open positions in the Scene view
    void OnDrawGizmos()
    {
        // Draw open position (initial position)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        // Draw closed position
        float direction = slideDirection == SlideDirection.Right ? 1f : -1f;
        Vector3 closedPos = transform.position - new Vector3(slideDistance * direction, 0f, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(closedPos, 0.2f);
    }
}