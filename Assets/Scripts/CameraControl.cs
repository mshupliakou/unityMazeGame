using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Camera controller that follows the player with look-ahead prediction
/// and smooth movement. Uses Unity's Input System for input handling.
/// </summary>
public class CameraControl : MonoBehaviour
{
	[Header("Target Settings")]
	[SerializeField] private Transform player;          // Player transform to follow
	[SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // Camera offset from player

	[Header("Movement Settings")]
	[Tooltip("How far the camera looks ahead of the player")]
	[SerializeField] private float lookAheadAmount = 2f;
	[Tooltip("Camera follow smoothing speed")]
	[SerializeField] private float smoothSpeed = 5f;
	[Tooltip("Minimum distance before camera starts moving")]
	[SerializeField] private float deadZoneRadius = 0.5f;

	private Vector3 _targetPosition;                   // Calculated target position
	private float _lookDirectionX;                     // Current look direction (-1 left, 1 right)
	private PlayerInput _playerInput;                  // Reference to player's input component

	private void Start()
	{
		if (player == null)
		{
			Debug.LogError("Player transform not assigned to CameraControl!");
			enabled = false;
			return;
		}

		_playerInput = player.GetComponent<PlayerInput>();
		if (_playerInput == null)
		{
			Debug.LogError("PlayerInput component missing on player!");
			enabled = false;
			return;
		}

		InitializeCameraPosition();
	}

	/// <summary>
	/// Sets initial camera position without smoothing
	/// </summary>
	private void InitializeCameraPosition()
	{
		_lookDirectionX = 1f; // Default to right direction
		_targetPosition = CalculateTargetPosition();
		transform.position = _targetPosition; // Snap to position
	}

	private void Update()
	{
		if (_playerInput == null) return;

		UpdateLookDirection();
		_targetPosition = CalculateTargetPosition();
	}

	/// <summary>
	/// Updates camera's look direction based on player input
	/// </summary>
	private void UpdateLookDirection()
	{
		Vector2 moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

		// Only update direction when there's significant horizontal input
		if (Mathf.Abs(moveInput.x) > 0.1f)
		{
			_lookDirectionX = Mathf.Sign(moveInput.x);
		}
	}

	/// <summary>
	/// Calculates target position with look-ahead offset
	/// </summary>
	private Vector3 CalculateTargetPosition()
	{
		return player.position + offset +
			   new Vector3(_lookDirectionX * lookAheadAmount, 0, 0);
	}

	private void LateUpdate()
	{
		// Only move if outside dead zone
		if (Vector3.Distance(transform.position, _targetPosition) > deadZoneRadius)
		{
			transform.position = Vector3.Lerp(
				transform.position,
				_targetPosition,
				smoothSpeed * Time.deltaTime
			);
		}
	}

#if UNITY_EDITOR
    /// <summary>
    /// Draw debug gizmos in editor
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Draw look-ahead direction
        Gizmos.color = Color.blue;
        Vector3 lookAheadPos = player.position + 
                              new Vector3(_lookDirectionX * lookAheadAmount, 0, 0);
        Gizmos.DrawLine(player.position, lookAheadPos);
        Gizmos.DrawWireSphere(lookAheadPos, 0.2f);

        // Draw dead zone
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, deadZoneRadius);
    }
#endif
}