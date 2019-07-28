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
    [SerializeField] private float minChaseDistance = 5f;
    [SerializeField] private float maxChaseDistance = 10f;
    [SerializeField] public float idleTimer;

    Animator animator;

    private IEnemyState currentState;

    private Rigidbody2D enemyRigidBody;

    bool facingRight;
    
    private void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        facingRight = true;

        ChangeState(new IdleState());
        // GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>()
    }

    public void Move()
    {
        //animator.SetFloat("Speed", moveSpeed);

        transform.Translate(GetDirection() * (moveSpeed * Time.fixedDeltaTime));

        /*
        if (enemyObject.name == "Opossum")
        {
            if (chaseObject != null)
            {
                Chase();
            }
            else
            {
                Patrol(moveSpeed);
            }
        }*/
    }

    private void Update()
    {
        currentState.Execute();
        //Move();
    }

    public void ChangeState(IEnemyState newState)
    {
        // Stop current state
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    private void Patrol(float move)
    {
        // X movement 
        if (IsFacingRight())
        {
            enemyRigidBody.velocity = new Vector2(moveSpeed, 0f);
        }
        else
        {
            enemyRigidBody.velocity = new Vector2(-moveSpeed, 0f);
        }
    }

    private void Chase()
    {
        var targetPositionDefault = new Vector2(chaseObject.transform.position.x, 0f);

        // If target is close, chase
        if (Vector2.Distance(transform.position, chaseObject.transform.position) <= minChaseDistance)
        {
            var targetPosition = new Vector2(Mathf.Sign(chaseObject.transform.position.x), transform.position.y);
            
            // Chase object
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            Patrol(moveSpeed);
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
