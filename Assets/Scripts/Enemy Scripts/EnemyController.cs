using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    // Cached components
    [SerializeField] private float moveSpeed = 1f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] public GameObject chaseObject;
    [SerializeField] private GameObject enemyObject;
    [SerializeField] public float idleTimer;
    [SerializeField] public bool facingRight = false;
    [SerializeField] public bool idleOnly = false;
    [SerializeField] public bool patrolOnly = false;

    public Animator animator;

    private IEnemyState currentState;
    private Rigidbody2D enemyRigidBody;
    
    
    private void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        chaseObject = null;

        if (patrolOnly)
        {
            ChangeState(new PatrolState());
        } 
        else
        {
            ChangeState(new IdleState()); 
        }
        
        // GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>()
    }

    public void Move()
    {
        //animator.SetFloat("Speed", moveSpeed);
        if (enemyObject.name == "Opossum" || enemyObject.name == "Frog Master")
        {
            // Increase speed towards player
            if (chaseObject != null)
            {
                transform.Translate(GetDirection() * (moveSpeed * Time.fixedDeltaTime * 2.5f));
            }
            else
            {
                // Move only
                transform.Translate(GetDirection() * (moveSpeed * Time.fixedDeltaTime));
            }
            
        }
    }

    private void FixedUpdate()
    {
        currentState.Execute();
    }

    public void ChangeState(IEnemyState newState)
    {
        // Stop current state
        if (currentState != null)
        {
            currentState.Exit();
        }

        // Only enter new state when it's different from previous state, prevent multiple calls of the same function
        if (currentState != newState)
        {
            currentState = newState;
            currentState.Enter(this);
        }
    }

    public bool IsFacingRight()
    {
        // Positive localScale = true, else false
        return transform.localScale.x > 0;
    }

    public Vector2 GetDirection()
    {
        facingRight = IsFacingRight();
        return facingRight ? Vector2.right : Vector2.left;
    }
    
    // Flip enemy sprite when it reaches the ledge
    public void ChangeDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector2(-(transform.localScale.x), 1f);
    }
    
}
