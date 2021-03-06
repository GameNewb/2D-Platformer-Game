﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public string stateName;

    // Cached components
    [SerializeField] private float moveSpeed = 1f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] public GameObject chaseObject;
    [SerializeField] public int health = 3;
    [SerializeField] public float idleTimer = 5f;
    [SerializeField] public float jumpForce = 200f;
    [SerializeField] public bool groundEnemy = false;
    [SerializeField] public bool leapingEnemy = false;
    [SerializeField] public bool idleOnly = false;
    [SerializeField] public bool patrolOnly = false;
    

    private IEnemyState currentState;
    private Rigidbody2D enemyRigidBody;
    private bool facingRight = false;
    private bool isDestroyed = false;

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

    private void FixedUpdate()
    {
        if (health < 0)
        {
            StartCoroutine(PlayDeathAnimation());
            return;
        }

        currentState.Execute();

        if (stateName == "Patrol")
        {
            Move();
        }
        else
        {
            // Reset velocity for idle state
            enemyRigidBody.velocity = new Vector2(0f, 0f);
        }
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


    /*** MOVEMENT FUNCTIONS ***/
    public void Move()
    {
        float direction = GetXAxis();

        //animator.SetFloat("Speed", moveSpeed);
        if (groundEnemy || leapingEnemy)
        {
            IncreaseVelocity(direction);
        }
    }

    private void IncreaseVelocity(float direction)
    {
        // Increase speed towards player
        if (chaseObject != null)
        {
            enemyRigidBody.velocity = new Vector2(direction * (moveSpeed * 2.5f), enemyRigidBody.velocity.y);
            //transform.Translate(GetDirection() * (moveSpeed * Time.fixedDeltaTime * 2.5f));
        }
        else
        {
            // Move only
            enemyRigidBody.velocity = new Vector2(direction * moveSpeed, enemyRigidBody.velocity.y);
            //transform.Translate(GetDirection() * (moveSpeed * Time.fixedDeltaTime));
        }
    }

    public void AddJumpForce()
    {
        enemyRigidBody.AddForce(new Vector2(0f, jumpForce));
    }
    /*** END MOVEMENT FUNCTIONS ***/
    

    /*** DIRECTIONAL FUNCTIONS ***/
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

    public float GetXAxis()
    {
        facingRight = IsFacingRight();
        return facingRight ? 1f : -1f;
    }

    // Flip enemy sprite when it reaches the ledge
    public void ChangeDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector2(-(transform.localScale.x), 1f);
    }
    /*** END DIRECTIONAL FUNCTIONS ***/

    // Coroutine to play the death animation of Enemy objects
    IEnumerator PlayDeathAnimation()
    {
        isDestroyed = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), isDestroyed);

        // Obtain animator controller to determine the length of death animation
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        float time = 1f;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == "Enemy Death") //If it has the same name as your clip
            {
                time = ac.animationClips[i].length;
            }
        }

        // Reset velocity so that the enemy doesn't move during death
        enemyRigidBody.velocity = new Vector2(0f, 0f);

        // Set appropriate animation
        animator.SetBool("IsDestroyed", true);
        animator.SetBool("IsMoving", false);
        animator.SetFloat("Speed", 0f);

        yield return new WaitForSeconds(time); 
        Destroy(gameObject);
    }
  
}
