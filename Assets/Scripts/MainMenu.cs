using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles main menu functionality including:
/// - Volume control via sliders
/// - Scene transitions
/// - Game exit
/// - Audio feedback for UI interactions
/// </summary>
public class MainMenu : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private Slider musicSlider;    // Controls music volume
	[SerializeField] private Slider sfxSlider;     // Controls SFX volume

	private AudioManager audioManager;             // Reference to audio system

	private void Awake()
	{
		InitializeAudioManager();
		ValidateSliders();
		InitializeVolumeSliders();
	}

	/// <summary>
	/// Safely gets reference to AudioManager with error checking
	/// </summary>
	private void InitializeAudioManager()
	{
		GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
		if (audioObject == null)
		{
			Debug.LogError("No GameObject with 'Audio' tag found!");
			return;
		}

		audioManager = audioObject.GetComponent<AudioManager>();
		if (audioManager == null)
		{
			Debug.LogError("AudioManager component missing on Audio object!");
		}
	}

	/// <summary>
	/// Validates that required sliders are assigned
	/// </summary>
	private void ValidateSliders()
	{
		if (musicSlider == null || sfxSlider == null)
		{
			Debug.LogError("Volume sliders not assigned in inspector!");
		}
	}

	/// <summary>
	/// Initializes volume sliders with current values and sets up callbacks
	/// </summary>
	private void InitializeVolumeSliders()
	{
		if (audioManager == null) return;

		// Set initial values
		musicSlider.value = audioManager.GetMusicVolume();
		sfxSlider.value = audioManager.GetSFXVolume();

		// Add listeners
		musicSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
		sfxSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
	}

	/// <summary>
	/// Starts the game with sound feedback
	/// </summary>
	public void PlayGame()
	{
		StartCoroutine(PlaySoundAndLoadScene("SampleScene"));
	}

	/// <summary>
	/// Quits the game with sound feedback
	/// </summary>
	public void QuitGame()
	{
		StartCoroutine(PlaySoundAndQuit());
	}

	/// <summary>
	/// Coroutine that plays button sound and loads scene asynchronously
	/// </summary>
	private IEnumerator PlaySoundAndLoadScene(string sceneName)
	{
		if (audioManager != null)
		{
			audioManager.PlaySFX(audioManager.buttonClicked);
		}

		// Brief delay for sound playback
		yield return new WaitForSecondsRealtime(0.1f);

		// Async scene loading with delay
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;

		// Additional delay for visual feedback
		yield return new WaitForSecondsRealtime(0.2f);
		asyncLoad.allowSceneActivation = true;
	}

	/// <summary>
	/// Coroutine that plays button sound and quits application
	/// </summary>
	private IEnumerator PlaySoundAndQuit()
	{
		if (audioManager != null)
		{
			audioManager.PlaySFX(audioManager.buttonClicked);
		}

		yield return new WaitForSecondsRealtime(0.1f);

		Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}