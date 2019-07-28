using UnityEngine.Events;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public EnemyController2D controller;
    public Animator animator;
    [SerializeField] float moveSpeed = 40f;

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
        animator.SetBool("IsMoving", true);
    }

    public void OnLanding()
    {
        animator.SetBool("IsMoving", false);
    }

    void FixedUpdate()
    {
        controller.Move(moveSpeed);
    }
}
