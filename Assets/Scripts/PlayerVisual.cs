using UnityEngine;

/// <summary>
/// Handles visual representation of the player character,
/// including animation states and sprite flipping based on movement.
/// </summary>
public class PlayerVisual : MonoBehaviour
{
	[Header("Component References")]
	private Animator animator;                // Reference to the Animator component
	private SpriteRenderer spriteRenderer;    // Reference to the SpriteRenderer component

	[Header("Animation Parameters")]
	private const string IS_WALKING = "IsWalking";  // Animator boolean parameter name

	private void Awake()
	{
		// Cache required components
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		// Early exit if PlayerController instance isn't available
		if (PlayerController.Instance == null) return;

		UpdateWalkingAnimation();
		UpdateSpriteFacing();
	}

	/// <summary>
	/// Updates the walking animation state based on player movement
	/// </summary>
	private void UpdateWalkingAnimation()
	{
		bool isWalking = PlayerController.Instance.IsWalking();
		animator.SetBool(IS_WALKING, isWalking);
	}

	/// <summary>
	/// Flips the sprite horizontally based on movement direction
	/// </summary>
	private void UpdateSpriteFacing()
	{
		Vector2 moveDirection = PlayerController.Instance.GetMoveDirection();

		// Only flip when there's significant horizontal movement
		if (Mathf.Abs(moveDirection.x) > Mathf.Epsilon)
		{
			spriteRenderer.flipX = moveDirection.x < 0;
		}
	}
}