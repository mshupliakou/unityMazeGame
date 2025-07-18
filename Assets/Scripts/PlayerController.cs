using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement, input processing, and audio feedback.
/// Implements singleton pattern for global access.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
	[Header("Movement Settings")]
	[Tooltip("Base movement speed in units per second")]
	[SerializeField] private float speed = 5f;
	[Tooltip("Minimum input magnitude to be considered moving")]
	[SerializeField] private float minMovingSpeed = 0.1f;

	[Header("Component References")]
	private Rigidbody2D rb;
	private BoxCollider2D playerCollider;
	private AudioManager audioManager;

	// Movement state
	private Vector2 moveInput;
	private bool isWalking = false;
	private bool wasWalking = false;

	// Singleton instance
	public static PlayerController Instance { get; private set; }

	#region Initialization
	private void Awake()
	{
		InitializeSingleton();
		InitializeComponents();
		ReturnToMenu.RestorePlayerPosition();
	}

	private void InitializeSingleton()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	private void InitializeComponents()
	{
		rb = GetComponent<Rigidbody2D>();
		playerCollider = GetComponent<BoxCollider2D>();

		// Configure physics
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb.interpolation = RigidbodyInterpolation2D.Interpolate;
		rb.freezeRotation = true;

		// Get audio manager
		audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
		if (audioManager == null)
		{
			Debug.LogWarning("AudioManager not found in scene");
		}
	}

	private void Start()
	{
		ReturnToMenu.RestorePlayerPosition();
		SetupInputCallbacks();
	}
	#endregion

	#region Input Handling
	private void SetupInputCallbacks()
	{
		var playerInput = GetComponent<PlayerInput>();
		if (playerInput != null)
		{
			playerInput.actions["Move"].performed += OnMove;
			playerInput.actions["Move"].canceled += OnMove;
		}
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
	}

	private void OnDestroy()
	{
		var playerInput = GetComponent<PlayerInput>();
		if (playerInput != null)
		{
			playerInput.actions["Move"].performed -= OnMove;
			playerInput.actions["Move"].canceled -= OnMove;
		}
	}
	#endregion

	#region Movement
	private void FixedUpdate()
	{
		HandleMovement();
		UpdateWalkingState();
	}

	private void HandleMovement()
	{
		Vector2 targetVelocity = moveInput * speed;
		rb.linearVelocity = targetVelocity;
	}

	private void UpdateWalkingState()
	{
		bool nowWalking = moveInput.magnitude > minMovingSpeed;

		// Handle walking sound effects
		if (nowWalking && !wasWalking)
		{
			audioManager?.PlaySFX(audioManager.walkingNoise);
		}
		else if (!nowWalking && wasWalking)
		{
			audioManager?.StopSFX();
		}

		// Update state
		isWalking = nowWalking;
		wasWalking = nowWalking;
	}
	#endregion

	#region Public Interface
	/// <summary>
	/// Returns whether the player is currently moving
	/// </summary>
	public bool IsWalking()
	{
		return isWalking;
	}

	/// <summary>
	/// Returns the current movement input vector
	/// </summary>
	public Vector2 GetMoveDirection()
	{
		return moveInput;
	}
	#endregion
}