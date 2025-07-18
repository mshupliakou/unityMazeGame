using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles returning to the main menu while preserving the player's position
/// for when they return to the game scene. Includes sound effect playback.
/// </summary>
public class ReturnToMenu : MonoBehaviour
{
	[Header("Scene Settings")]
	[SerializeField] private string menuSceneName = "MainMenu";  // Name of the menu scene to load

	[Header("Player Position Saving")]
	private static Vector3 savedPlayerPosition;  // Stores player's position between scenes
	private static bool hasSavedPosition = false;  // Flag indicating if position was saved

	private AudioManager audioManager;  // Reference to audio system

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

	/// <summary>
	/// Transitions to the main menu scene while saving the player's current position.
	/// Resets timescale and plays UI sound effect.
	/// </summary>
	public void ReturnToMainMenu()
	{
		StartCoroutine(PlayButtonSoundAndLoad());

		// Save player position before leaving
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			savedPlayerPosition = player.transform.position;
			hasSavedPosition = true;
		}

		// Ensure normal timescale before scene change
		Time.timeScale = 1f;
	}

	/// <summary>
	/// Plays button click sound with slight delay before scene transition
	/// </summary>
	private IEnumerator PlayButtonSoundAndLoad()
	{
		if (audioManager != null)
		{
			audioManager.PlaySFX(audioManager.buttonClicked);
		}

		// Brief delay to allow sound to play
		yield return new WaitForSecondsRealtime(0.1f);

		SceneManager.LoadScene(menuSceneName);
	}

	/// <summary>
	/// Restores the player's saved position when returning to the game scene.
	/// Should be called when the game scene loads.
	/// </summary>
	public static void RestorePlayerPosition()
	{
		if (hasSavedPosition)
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				player.transform.position = savedPlayerPosition;
			}
			hasSavedPosition = false;  // Reset after restoring
		}
	}
}