using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // For TextMeshPro

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;     // Maximum health of the player
    public int currentHealth;       // Current health of the player
    public Slider healthSlider;     // UI Slider to represent health visually
    public Image damageImage;       // Image that flashes when taking damage
    public float flashSpeed = 5f;   // Speed of the flash effect
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f); // Color to flash when taking damage
    public AudioClip diamondPickupSound; // Audio clip for diamond pickup sound
    public AudioClip gameOverSound;      // Audio clip for game over sound
    public TextMeshProUGUI gameOverText; // Reference to the Game Over TMP text

    // Private reference to the Animator component for death animation
    private Animator anim;
    // Private reference to the AudioSource component
    private AudioSource audioSource;
    // Boolean to check if the player is dead
    private bool isDead;

    void Awake()
    {
        // Get the Animator component
        anim = GetComponent<Animator>();
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        // Initialize current health to max health
        currentHealth = maxHealth;
        // Set the health slider to full
        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
            healthSlider.maxValue = maxHealth;
        }
        // Ensure Game Over text is hidden initially
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        isDead = false;
    }

    // Public method to apply damage to the player
    public void TakeDamage(int amount)
    {
        if (isDead) return; // Don't apply damage if already dead

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
        }

        if (damageImage != null) 
        {
            damageImage.color = flashColour;
        }

        if (currentHealth <= 0 && !isDead)
        {
            StartCoroutine(DeathSequence());
        }
    }

    // Public method to heal the player
    public void Heal(int amount)
    {
        if (isDead) return; // Don't heal if dead

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
        }
    }

    // Method to fully regenerate health
    public void RegenerateFullHealth()
    {
        if (isDead) return; // Don't regenerate if dead

        currentHealth = maxHealth;

        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
        }
    }

    // Detect collision with diamond (using trigger)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Diamond"))
        {
            RegenerateFullHealth();
            Destroy(other.gameObject); // Remove the diamond from the scene
            Debug.Log("Diamond collected! Health fully restored.");
            
            if (audioSource != null && diamondPickupSound != null)
            {
                audioSource.PlayOneShot(diamondPickupSound);
            }
        }
    }

    // Coroutine for handling the death sequence
    IEnumerator DeathSequence()
    {
        isDead = true;
        if (anim != null) 
        {
            anim.SetTrigger("Die");
        }

        // Show Game Over text
        if (gameOverText != null)
        {
            gameOverText.text = "GAME OVER";
            gameOverText.gameObject.SetActive(true);
        }

        // Play Game Over sound
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Wait for the animation to finish or for a certain time
        yield return new WaitForSeconds(2f);

        // Pause the game
        Time.timeScale = 0f;

        // Game over logic here, e.g., load game over screen
        Debug.Log("Player has died!");
        //SceneManager.LoadScene("GameOver");
    }

    void Update()
    {
        // Gradually fade out the damage image if it's not fully transparent
        if (damageImage != null)
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }
}