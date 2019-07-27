using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    // Cached components
    [SerializeField] private float moveSpeed = 1f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private GameObject chaseObject;
    [SerializeField] private GameObject enemyObject;
    [SerializeField] private float minChaseDistance = 5f;
    [SerializeField] private float maxChaseDistance = 10f;

    private Rigidbody2D enemyRigidBody;

    private void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        // GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>()
    }

    public void Move()
    {
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
        }
    }

    private void FixedUpdate()
    {
        Move();
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
        // If target is close, chase
        if (Vector3.Distance(transform.position, chaseObject.transform.position) < minChaseDistance)
        {
            var targetPosition = new Vector2(chaseObject.transform.position.x, transform.position.y);
            
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

    // Flip enemy when it reaches the ledge
    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(enemyRigidBody.velocity.x)), 1f);
    }
    
}
