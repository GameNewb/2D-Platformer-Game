using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool climbing = false;

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        } else if (Input.GetButtonUp("Crouch"))
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
}
