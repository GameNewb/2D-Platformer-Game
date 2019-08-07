using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    // Cached components
	[SerializeField] private float jumpForce;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private float climbSpeed = 8f;
    [SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask whatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform ceilingCheck;							// A position marking where to check for ceilings
    [SerializeField] private Collider2D crouchDisableCollider;                // A collider that will be disabled when crouching
    [SerializeField] private Vector2 knockback = new Vector2(0.5f, 0.5f);

    const float groundedRadius = .05f; // Radius of the overlap circle to determine if grounded
    const float ceilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private bool grounded;            // Whether or not the player is grounded.
    private Rigidbody2D rigidBody2D;
    private CircleCollider2D bodyCollider2D;
    private CapsuleCollider2D feetCollider2D;
    private HealthSystem playerHealth;
    private Vector3 m_Velocity = Vector3.zero;
    private float gravityScaleAtStart;
    private float jumpTimeCounter;

    public float jumpTime;

    [HideInInspector] public bool tookDamage = false;

    [Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

    [System.Serializable]
    public class BoolArrayEvent : UnityEvent<bool, bool> { }

    public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
    
    public BoolArrayEvent OnClimbEvent;

    private void Awake()
	{
		rigidBody2D = GetComponent<Rigidbody2D>();
        bodyCollider2D = GetComponent<CircleCollider2D>();
        feetCollider2D = GetComponent<CapsuleCollider2D>();
        playerHealth = GetComponent<HealthSystem>();

        gravityScaleAtStart = rigidBody2D.gravityScale;

        if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
        
        if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
        
        if (OnClimbEvent == null)
            OnClimbEvent = new BoolArrayEvent();

    }

    private void FixedUpdate()
	{
        bool wasGrounded = grounded;
		grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
                grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

        //TakeDamage();
 
    }

	public void Move(float move, bool crouch, bool jump, bool climb)
	{
        bool isTouchingGround = feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool isTouchingLadder = bodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"));

        // Make sure player doesn't "grab" or "latch" onto ladders until user presses UP key
        if (climb)
        {
            // If player's body is touching any climbing layers
            if (isTouchingLadder)
            {
                rigidBody2D.gravityScale = 0f;
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, 0);

                bool isMovingVertically = (Input.GetAxis("Vertical") != 0f);
                bool onTheGround = feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));

                if (isMovingVertically)
                {
                    // Disable crouching when on the ladder
                    crouch = false;

                    rigidBody2D.velocity = new Vector2(0f, Input.GetAxis("Vertical") * climbSpeed);
                    OnClimbEvent.Invoke(true, true);
                }
                else if (!onTheGround)
                {
                    OnClimbEvent.Invoke(true, false);
                }

                // If jump key is pressed while climbing, climb = false
                if (jump)
                {
                    // Reset back gravity and add jump velocity
                    // Add jumpforce to Y velocity, but ignore the player Y velocity when going up a ladder
                    grounded = false;
                    rigidBody2D.gravityScale = gravityScaleAtStart;
                    rigidBody2D.AddForce(new Vector2(0f, jumpForce - (climbSpeed * rigidBody2D.velocity.y)));
                    OnClimbEvent.Invoke(false, onTheGround);
                }

            }
            else
            {
                rigidBody2D.gravityScale = gravityScaleAtStart;
                OnClimbEvent.Invoke(false, false);
            }

        }

        // If crouching, check to see if the character can stand up
        if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (grounded || m_AirControl)
		{
			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= crouchSpeed;

				// Disable one of the colliders when crouching
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = false;
			}
            else
			{
				// Enable the collider when not crouching
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector2 targetVelocity = new Vector2(move * 10f, rigidBody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			rigidBody2D.velocity = Vector3.SmoothDamp(rigidBody2D.velocity, targetVelocity, ref m_Velocity, movementSmoothing);

            FlipSprite(move);
        } 

		// If the player should jump...
		if (grounded && jump)
		{
            // Add a vertical force to the player.
            //m_Grounded = false;
            rigidBody2D.AddForce(new Vector2(0f, jumpForce));

            /*
            jumpTimeCounter = jumpTime;
            rigidBody2D.velocity = Vector2.up * jumpForce;*/
        }
        
        /* Higher jump when Space is held
        if (Input.GetKey(KeyCode.Space) && !grounded)
        {
            if (jumpTimeCounter > 0)
            {
                Debug.Log("jumpTimeCounter > 0");
                rigidBody2D.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
        }*/

        if (!grounded)
        {
            isTouchingLadder = bodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"));

            if (climb && isTouchingLadder)
            {
                bool isMovingVertically = (Input.GetAxis("Vertical") != 0f);
                bool onTheGround = feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));

                if (isMovingVertically)
                {
                    // Stop jumping animation after player latches onto ladder
                    OnLandEvent.Invoke();

                    rigidBody2D.velocity = new Vector2(0f, Input.GetAxis("Vertical") * climbSpeed);
                    OnClimbEvent.Invoke(true, true);
                }
                else if (!onTheGround)
                {
                    OnClimbEvent.Invoke(true, false);
                }
            }
            else
            {
                OnClimbEvent.Invoke(false, false);
            }

        }
    }

    private void FlipSprite(float xVelocity)
    {
        // Movement is > 0
        bool playerHasHorizontalSpeed = Mathf.Abs(xVelocity) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            // Flip sprite, either +1 or -1 for scale
            transform.localScale = new Vector2(Mathf.Sign(xVelocity), 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isTouchingEnemy = bodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy")) || feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy"));

        // 12 - Enemy layer
        if ((collision.gameObject.layer.Equals(12) || isTouchingEnemy) && playerHealth.currentHealth > 0 && !tookDamage) 
        {
            rigidBody2D.velocity = -(knockback);

            if (FindObjectOfType<GameObject>())
            {
                FindObjectOfType<GameSession>().ProcessPlayerDeath();
            }
        }
    }
}
