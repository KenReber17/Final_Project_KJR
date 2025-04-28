using UnityEngine;
using TMPro;
using System.Collections;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelCompleteText; // Reference to TextMeshProUGUI for "LEVEL COMPLETE"
    [SerializeField] private AudioSource completeAudioSource; // AudioSource for level complete sound
    [SerializeField] private float pauseDelay = 1f; // Delay before pausing the game (in seconds)
    private bool isPlayerInTrigger = false; // Tracks if player is in the trigger zone
    private bool isLevelCompleted = false; // Prevents multiple triggers

    void Start()
    {
        // Ensure the TextMeshProUGUI is hidden initially
        if (levelCompleteText != null)
        {
            levelCompleteText.gameObject.SetActive(false);
            Debug.Log("LevelCompleteText initialized and hidden on " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("LevelCompleteText is not assigned on " + gameObject.name);
        }

        // Validate AudioSource
        if (completeAudioSource == null)
        {
            Debug.LogWarning("CompleteAudioSource is not assigned on " + gameObject.name);
        }
        else if (completeAudioSource.clip == null)
        {
            Debug.LogWarning("CompleteAudioSource has no AudioClip assigned on " + gameObject.name);
        }
        else
        {
            Debug.Log("CompleteAudioSource initialized with clip: " + completeAudioSource.clip.name);
        }

        // Validate pauseDelay
        if (pauseDelay < 0f)
        {
            pauseDelay = 0f;
            Debug.LogWarning("PauseDelay was negative; set to 0 on " + gameObject.name);
        }
    }

    void Update()
    {
        // Check for "E" key press when player is in trigger zone and level not yet completed
        if (isPlayerInTrigger && !isLevelCompleted && Input.GetKeyDown(KeyCode.E))
        {
            isLevelCompleted = true; // Prevent re-triggering
            // Show "LEVEL COMPLETE" text
            if (levelCompleteText != null)
            {
                levelCompleteText.text = "LEVEL COMPLETE";
                levelCompleteText.gameObject.SetActive(true);
                Debug.Log("Level complete! Displaying text on " + gameObject.name);
            }
            else
            {
                Debug.LogWarning("Cannot display level complete text: TextMeshProUGUI is not assigned");
            }

            // Play level complete sound
            if (completeAudioSource != null && completeAudioSource.clip != null)
            {
                completeAudioSource.PlayOneShot(completeAudioSource.clip);
                Debug.Log("Playing level complete sound: " + completeAudioSource.clip.name);
            }
            else
            {
                Debug.LogWarning("Cannot play level complete sound: AudioSource or AudioClip is missing");
            }

            // Start coroutine to pause after delay
            StartCoroutine(PauseAfterDelay());
        }
    }

    // Coroutine to pause the game after pauseDelay
    private IEnumerator PauseAfterDelay()
    {
        yield return new WaitForSeconds(pauseDelay);
        Time.timeScale = 0f;
        Debug.Log($"Game paused after {pauseDelay}s delay on " + gameObject.name);
    }

    // Detect when player enters the trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            Debug.Log("Player entered level complete trigger zone on " + gameObject.name);
        }
    }

    // Detect when player exits the trigger zone
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("Player left level complete trigger zone on " + gameObject.name);
        }
    }
}