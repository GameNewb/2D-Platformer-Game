using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public PlayerPosition startingPosition;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool climbing = false;

    private float tapSpeed = 0.35f;
    private float lastTapTime = 0;
    private bool doubleTap = false;
    private KeyCode currentKey;
    private KeyCode previousKey;
    /*
    private void Start()
    {
        transform.position = startingPosition.initialValue;
    }*/

    void Update()
    {
        checkDoubleTap();

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetButtonDown("Climb"))
        {
            climbing = true;
        }

        
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool IsCrouching)
    {
        animator.SetBool("IsCrouching", IsCrouching);
    }
    
    public void OnClimb(bool IsClimbing, bool OnGround)
    {
        if (IsClimbing)
        {
            // Set animator speed depending if player is moving ON the ladder or staying still
            if (!OnGround)
            {
                animator.SetBool("Climbing", true);
                animator.speed = 0f;
            }
            else
            {
                animator.SetBool("Climbing", OnGround);
                animator.speed = 1f;
            }
        }
        else
        {
            climbing = IsClimbing;
            animator.SetBool("Climbing", IsClimbing);
            animator.speed = 1f;
        }
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, climbing);
        jump = false;
    }

    // Function to allow player to dash when keys are simultaneously pressed
    private void checkDoubleTap()
    {
        previousKey = currentKey;

        // Reset double tap when timer resets
        if ((Time.time - lastTapTime) > tapSpeed)
        {
            animator.SetBool("Idle", true);
            animator.SetBool("IsDashing", false);
            doubleTap = false;
        }

        // Check whether left or right keys are being pressed
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentKey = KeyCode.RightArrow;

            if ((Time.time - lastTapTime) < tapSpeed)
            {
                animator.SetBool("IsDashing", true);
                doubleTap = true;
            }

            lastTapTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentKey = KeyCode.LeftArrow;

            if ((Time.time - lastTapTime) < tapSpeed)
            {

                animator.SetBool("IsDashing", true);
                doubleTap = true;
            }

            lastTapTime = Time.time;
        }

        // Reset double tap when player quickly taps left->right or right->left keys
        if (currentKey != previousKey)
        {
            animator.SetBool("IsDashing", false);
            doubleTap = false;
        }

        // Add basic movement to player X axis
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        // Dash towards direction
        if (doubleTap)
        {
            horizontalMove += Input.GetAxis("Horizontal") * (runSpeed / 1.2f);
        }
    }
}
