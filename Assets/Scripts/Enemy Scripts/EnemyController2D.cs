using UnityEngine;
using UnityEngine.Events;

public class EnemyController2D : MonoBehaviour
{
    // Cached components
    [SerializeField] private float m_JumpForce = 100f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform enemyGroundCheck;                           // A position marking where to check if the player is grounded.
   

    const float k_GroundedRadius = .05f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D enemyRigidBody;
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    private void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

    }

    public void Move(float move)
    {
        /*
        //only control the player if grounded or airControl is turned on
        if (m_Grounded)
        {
            // Move the character by finding the target velocity
            Vector2 targetVelocity = new Vector2(move * 10f, enemyRigidBody.velocity.y);
            // And then smoothing it out and applying it to the character
            enemyRigidBody.velocity = Vector3.SmoothDamp(enemyRigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            enemyRigidBody.AddForce(new Vector2(0f, m_JumpForce));
        }*/
        
        // If the player should jump...
        if (m_Grounded)
        {
            // X movement 
            //if (IsFacingRight())
           // {
               // enemyRigidBody.velocity = new Vector2(move * Time.fixedDeltaTime, 0f);
            //}
            //else
           // {
               enemyRigidBody.velocity = new Vector2(-move * Time.fixedDeltaTime, 0f);
            //}

            // Add a vertical force to the player.
            //m_Grounded = false;
            enemyRigidBody.AddForce(new Vector2(move, m_JumpForce));
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyGroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public bool IsFacingRight()
    {
        // Positive localScale = true, else false
        return transform.localScale.x > 0;
    }

    // Flip enemy when it reaches the ledge
    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(enemyRigidBody.velocity.x)), 1f);
    }
}
