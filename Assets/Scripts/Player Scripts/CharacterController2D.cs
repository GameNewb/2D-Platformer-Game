using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    // Cached components
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private float climbSpeed = 8f;
    [SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    [SerializeField] private Vector2 knockback = new Vector2(0.5f, 0.5f);

    const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
    private CircleCollider2D bodyCollider2D;
    private CapsuleCollider2D feetCollider2D;
	private Vector3 m_Velocity = Vector3.zero;
    private float gravityScaleAtStart;
    private bool tookDamage = false;

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

    // State
    bool isAlive = true;

    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        bodyCollider2D = GetComponent<CircleCollider2D>();
        feetCollider2D = GetComponent<CapsuleCollider2D>();

        gravityScaleAtStart = m_Rigidbody2D.gravityScale;

        if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
        
        if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
        
        if (OnClimbEvent == null)
            OnClimbEvent = new BoolArrayEvent();

    }

    private void FixedUpdate()
	{
        bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
                m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

        TakeDamage();
 
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
                m_Rigidbody2D.gravityScale = 0f;
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);

                bool isMovingVertically = (Input.GetAxis("Vertical") != 0f);
                bool onTheGround = feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));

                if (isMovingVertically)
                {
                    // Disable crouching when on the ladder
                    crouch = false;

                    m_Rigidbody2D.velocity = new Vector2(0f, Input.GetAxis("Vertical") * climbSpeed);
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
                    m_Grounded = false;
                    m_Rigidbody2D.gravityScale = gravityScaleAtStart;
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce - (climbSpeed * m_Rigidbody2D.velocity.y)));
                    OnClimbEvent.Invoke(false, onTheGround);
                }

            }
            else
            {
                m_Rigidbody2D.gravityScale = gravityScaleAtStart;
                OnClimbEvent.Invoke(false, false);
            }

        }

        // If crouching, check to see if the character can stand up
        if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
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
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
            else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector2 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            FlipSprite(move);
        } 

		// If the player should jump...
		if (m_Grounded && jump)
		{
            // Add a vertical force to the player.
            //m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }

        if (!m_Grounded)
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

                    m_Rigidbody2D.velocity = new Vector2(0f, Input.GetAxis("Vertical") * climbSpeed);
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

    private void TakeDamage()
    {
        bool touchingEnemy = bodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy")) || feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy"));

        if (touchingEnemy && !tookDamage)
        {
            m_Rigidbody2D.velocity = -(knockback);

            if (FindObjectOfType<GameObject>())
            {
                FindObjectOfType<GameSession>().ProcessPlayerDeath();
            }

            tookDamage = true;
        }
    }

}
