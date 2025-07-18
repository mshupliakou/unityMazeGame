using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controls the behavior of a girl character that reacts when the player approaches,
/// including jumping animation, victory effects, and sound playback.
/// </summary>
public class GirlBehavior : MonoBehaviour
{
	[Header("Movement Settings")]
	public float jumpHeight = 1.5f;          // Maximum height of the jump
	public float jumpSpeed = 1f;            // Speed of the jump animation
	public float detectionRadius = 3f;      // Distance at which player is detected
	public float minYPosition = 0f;         // Minimum Y position to prevent falling through ground

	[Header("Component References")]
	public Transform player;                // Reference to the player's transform
	public Animator animator;               // Animator controller for character animations
	public TextMeshProUGUI winText;         // UI text element for victory message
	public AudioClip victorySound;          // Sound played when player wins

	private bool isJumping = false;         // Flag for jump state
	private bool playerFound = false;      // Flag for player detection
	private Vector3 startPosition;          // Initial position before jumping
	private float jumpProgress = 0f;        // Progress through jump animation (0-PI)
	private AudioSource audioSource;        // For playing victory sound
	private AudioManager audioManager;      // Reference to global AudioManager

	private void Awake()
	{
		// Find and cache the AudioManager in the scene
		if (audioManager == null)
		{
			GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
			if (audioObject != null)
			{
				audioManager = audioObject.GetComponent<AudioManager>();
			}
		}
	}

	void Start()
	{
		// Initialize positions and components
		startPosition = transform.position;
		minYPosition = startPosition.y;

		// Ensure AudioSource component exists
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}

		// Hide win text initially
		if (winText != null)
		{
			winText.gameObject.SetActive(false);
		}
	}

	void Update()
	{
		// Check for player detection if not already found
		if (!playerFound && player != null)
		{
			float distance = Vector3.Distance(transform.position, player.position);

			if (distance <= detectionRadius)
			{
				playerFound = true;
				audioManager.StopMusic();
				PlayVictorySound();
				ShowWinText();
				StartJumping();
			}
		}
		// Handle jumping animation
		else if (isJumping)
		{
			jumpProgress += Time.deltaTime * jumpSpeed;
			float jumpValue = Mathf.Sin(jumpProgress * Mathf.PI); // Sine wave for smooth jump
			float newY = startPosition.y + jumpValue * jumpHeight;
			newY = Mathf.Max(newY, minYPosition); // Clamp to minimum Y position

			transform.position = new Vector3(
				transform.position.x,
				newY,
				transform.position.z
			);

			// Reset jump cycle
			if (jumpProgress >= Mathf.PI)
			{
				jumpProgress = 0f;
			}
		}
	}

	/// <summary>
	/// Plays the victory sound effect if configured
	/// </summary>
	void PlayVictorySound()
	{
		if (victorySound != null && audioSource != null)
		{
			audioSource.PlayOneShot(victorySound);
		}
		else
		{
			Debug.LogWarning("Victory sound or AudioSource not assigned!");
		}
	}

	/// <summary>
	/// Initiates the jumping animation and triggers happy animation state
	/// </summary>
	void StartJumping()
	{
		isJumping = true;
		jumpProgress = 0f;

		if (animator != null)
		{
			// Force play the Happy animation from the beginning
			animator.Play("Happy", -1, 0f); // -1 = base layer, 0f = normalized start time
			animator.SetBool("IsJumping", true);
		}
	}

	/// <summary>
	/// Displays and animates the victory text
	/// </summary>
	void ShowWinText()
	{
		if (winText != null)
		{
			winText.gameObject.SetActive(true);
			winText.text = "VICTORY!";
			StartCoroutine(ScaleTextAnimation());
		}
		else
		{
			Debug.LogWarning("Win text element not assigned!");
		}
	}

	/// <summary>
	/// Coroutine that animates the victory text with a scaling effect
	/// </summary>
	IEnumerator ScaleTextAnimation()
	{
		float duration = 0.5f;
		float timer = 0f;
		while (timer < duration)
		{
			timer += Time.deltaTime;
			// Ease-out effect using sine curve
			float progress = Mathf.Sin(timer / duration * Mathf.PI * 0.5f);
			winText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
			yield return null;
		}
		winText.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Draws debug gizmos in the Unity editor
	/// </summary>
	void OnDrawGizmosSelected()
	{
		// Draw detection radius
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRadius);

		// Draw minimum Y position indicator
		Gizmos.color = Color.red;
		Vector3 minPos = new Vector3(
			transform.position.x,
			minYPosition,
			transform.position.z
		);
		Gizmos.DrawLine(minPos - Vector3.right * 2f, minPos + Vector3.right * 2f);
	}
}