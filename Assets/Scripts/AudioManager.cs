using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// A singleton-based AudioManager that handles background music, SFX playback, and volume control.
/// Supports mixer groups for advanced audio routing and persists across scenes.
/// </summary>
public class AudioManager : MonoBehaviour
{
	// Singleton pattern: ensures only one instance exists.
	public static AudioManager Instance { get; private set; }

	[Header("Audio Sources")]
	[SerializeField] private AudioSource musicSource;  // Handles background music.
	[SerializeField] private AudioSource sfxSource;   // Handles one-shot sound effects.

	[Header("Audio Clips")]
	public AudioClip background;       // Main background music clip.
	public AudioClip buttonClicked;    // SFX for UI buttons.
	public AudioClip walkingNoise;     // SFX for player movement.

	[Header("Mixer Groups")]
	[SerializeField] private AudioMixerGroup musicMixerGroup;  // Optional mixer group for music.
	[SerializeField] private AudioMixerGroup sfxMixerGroup;   // Optional mixer group for SFX.

	private void Awake()
	{
		// Singleton implementation: destroy duplicates and persist across scenes.
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			InitializeAudio();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Initializes audio settings, assigns mixer groups, and starts background music.
	/// </summary>
	private void InitializeAudio()
	{
		// Assign mixer groups if provided.
		if (musicMixerGroup != null)
			musicSource.outputAudioMixerGroup = musicMixerGroup;
		if (sfxMixerGroup != null)
			sfxSource.outputAudioMixerGroup = sfxMixerGroup;

		// Start playing background music (looped).
		musicSource.clip = background;
		musicSource.loop = true;
		musicSource.Play();

		// Load saved volume settings or use defaults (70% volume).
		SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.7f));
		SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.7f));
	}

	/// <summary>
	/// Returns the current music volume (0-1 range).
	/// </summary>
	public float GetMusicVolume()
	{
		return PlayerPrefs.GetFloat("MusicVolume", 0.7f);
	}

	/// <summary>
	/// Returns the current SFX volume (0-1 range).
	/// </summary>
	public float GetSFXVolume()
	{
		return PlayerPrefs.GetFloat("SFXVolume", 0.7f);
	}

	/// <summary>
	/// Plays a one-shot SFX clip.
	/// </summary>
	/// <param name="clip">The AudioClip to play. Ignored if null.</param>
	public void PlaySFX(AudioClip clip)
	{
		if (clip == null) return;
		sfxSource.PlayOneShot(clip);
	}

	/// <summary>
	/// Sets the music volume (clamped to 0.0001-1) and saves it to PlayerPrefs.
	/// Applies to the mixer group (if assigned) or directly to the AudioSource.
	/// </summary>
	/// <param name="volume">Linear volume value (0-1).</param>
	public void SetMusicVolume(float volume)
	{
		volume = Mathf.Clamp(volume, 0.0001f, 1f);
		PlayerPrefs.SetFloat("MusicVolume", volume);

		// Convert linear volume to dB for the mixer (logarithmic scale).
		if (musicMixerGroup != null && musicMixerGroup.audioMixer != null)
		{
			float dB = Mathf.Log10(volume) * 20;
			musicMixerGroup.audioMixer.SetFloat("MusicVolume", dB);
		}
		else
		{
			musicSource.volume = volume;  // Fallback: set directly on the AudioSource.
		}
	}

	/// <summary>
	/// Sets the SFX volume (clamped to 0.0001-1) and saves it to PlayerPrefs.
	/// Applies to the mixer group (if assigned) or directly to the AudioSource.
	/// </summary>
	/// <param name="volume">Linear volume value (0-1).</param>
	public void SetSFXVolume(float volume)
	{
		volume = Mathf.Clamp(volume, 0.0001f, 1f);
		PlayerPrefs.SetFloat("SFXVolume", volume);

		if (sfxMixerGroup != null && sfxMixerGroup.audioMixer != null)
		{
			float dB = Mathf.Log10(volume) * 20;
			sfxMixerGroup.audioMixer.SetFloat("SFXVolume", dB);
		}
		else
		{
			sfxSource.volume = volume;  // Fallback: set directly on the AudioSource.
		}
	}

	/// <summary>
	/// Stops all currently playing SFX.
	/// </summary>
	public void StopSFX()
	{
		sfxSource.Stop();
	}

	/// <summary>
	/// Stops background music playback.
	/// </summary>
	public void StopMusic()
	{
		musicSource.Stop();
	}
}