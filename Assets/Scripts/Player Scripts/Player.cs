using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    // Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(0.5f, 0.5f);
    
    // State
    bool isAlive = true;

    // Cached component references
    Rigidbody2D rigidBody;
    RigidbodyConstraints2D playerConstraints;
    Animator animator;
    CapsuleCollider2D bodyCollider2D;
    BoxCollider2D feetCollider2D;
    float gravityScaleAtStart;
    string[] collideables;
 
    enum State
    {
        Idle,
        RunningLeft,
        RunningRight,
        JumpingUp,
        JumpingDown,
        Landing
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider2D = GetComponent<CapsuleCollider2D>();
        feetCollider2D = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = rigidBody.gravityScale;
        playerConstraints = rigidBody.constraints;

        collideables = new string[] { "Enemy", "Hazards" };
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        Run();
        Jump();
        Climb();
        //Death();
        FlipSprite();
    }

    private void Run()
    {
        // X axis, value is between -1 to +1
        // Get axis
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");

        // X movement 
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, rigidBody.velocity.y); // Current y velocity
        rigidBody.velocity = playerVelocity;

        // True if we're moving
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon;

        // 'Running' bool in Animation Controller
        animator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void Jump()
    {
        // Jump Key
        bool controlJump = CrossPlatformInputManager.GetButtonDown("Jump");
        bool isTouchingLayers = feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));

        // True if we're jumping
        bool playerHasVerticalSpeed = Mathf.Abs(rigidBody.velocity.y) > Mathf.Epsilon;
        
        // If not touching ground, set animation sprite
        if (!isTouchingLayers)
        {
            return;
        }
        else
        {
            animator.SetBool("Jumping", false);
            animator.speed = 1f;
        }

        if (controlJump)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            rigidBody.velocity += jumpVelocityToAdd;

            // Pause animation at frame 0
            animator.SetBool("Jumping", true);
            animator.speed = 0f;
        }
    }

    private void Climb()
    {
        bool isTouchingLadder = feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"));

        // If player is touching ladder, remove gravity and display correct sprite animation
        // Else bring back to default state
        if (isTouchingLadder)
        {
            rigidBody.gravityScale = 0f;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            OnPlayerVerticalInput();
        }
        else
        {
            animator.SetBool("Climbing", false);
            rigidBody.gravityScale = gravityScaleAtStart;
            animator.speed = 1f;
        }

        /*
        if (!isTouchingLadder)
        {
            // Set animator back to idle
            // Not on the ladder, reset gravity
            myAnimator.SetBool("Climbing", false);
            myRigidBody.constraints = playerConstraints;
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        // Move player up the ladder, locking in the X movement
        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
        
        if (playerHasVerticalSpeed)
        {
            myRigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
        } else
        {
            myRigidBody.constraints = playerConstraints;
        }*/
    }

    private void OnPlayerVerticalInput()
    {
        bool isMovingVertically = (CrossPlatformInputManager.GetAxis("Vertical") != 0f);
        bool onTheGround = feetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (isMovingVertically)
        {
            animator.SetBool("Climbing", true);
            rigidBody.velocity = new Vector2(0f, CrossPlatformInputManager.GetAxis("Vertical") * climbSpeed);
            animator.speed = 1f;
        }
        else if (!onTheGround)
        {
            animator.SetBool("Climbing", true);
            animator.speed = 0f;
        }
        else
        {
            animator.SetBool("Climbing", false);
            animator.speed = 1f;
        }

        /*
        animator.SetBool("Climbing", isMovingVertically);
        animator.speed = isMovingVertically ? 1f : 0f;

        float xMovement = isMovingVertically ? 0f : rigidBody.velocity.x;

        rigidBody.velocity = new Vector2(xMovement,
                    CrossPlatformInputManager.GetAxis("Vertical") * climbSpeed);*/
    }
    /*
    private void Death()
    {
        bool touchingEnemy = bodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards"));
        //bool touchingEnemy = IsCollidingWithEnemy();

        if (touchingEnemy)
        {
            animator.SetTrigger("Dying");
            GetComponent<Rigidbody2D>().velocity = -(deathKick);
            isAlive = false;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }*/

    private void FlipSprite()
    {
        // Movement is > 0
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            // Flip sprite, either +1 or -1 for scale
            transform.localScale = new Vector2(Mathf.Sign(rigidBody.velocity.x), 1);
        }
    }

    private bool IsCollidingWithEnemy()
    {
        // For every collideable enemy/object specified
        for (int i = 0; i < collideables.Length; i++)
        {
            // Check if player is colliding, if they are then deal damage
            if (bodyCollider2D.IsTouchingLayers(LayerMask.GetMask(collideables[i])))
            {
                return true;
            }
        }

        return false;
    }
}
