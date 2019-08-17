using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float animatorTime;
    private float timeBetweenAttacks;
    private string enemyTag;
    private bool isAttacking = false;
    public float startTimeBetweenAttacks;

    public Transform attackPosition;
    public LayerMask whatIsEnemy;
    public Rigidbody2D rigidBody2D;
    public Animator playerAnimator;
    public float attackRange;
    public int damage;

    private void Start()
    {
        // Obtain animator controller to determine the length of an animation
        RuntimeAnimatorController ac = playerAnimator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == "Ninja Attack 1") //If it has the same name as your clip
            {
                animatorTime = ac.animationClips[i].length;
            }
        }
    }

    private void FixedUpdate()
    {
        if (timeBetweenAttacks <= 0)
        {
            // Reset animator bool
            playerAnimator.SetBool("IsAttacking", false);

            // X to do Attack 1
            if (Input.GetKeyDown(KeyCode.X)) {
                isAttacking = true;
                playerAnimator.SetBool("IsAttacking", true);
                timeBetweenAttacks = startTimeBetweenAttacks;

                // Take in enemy layer and damage them
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsEnemy);

                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    enemyTag = enemiesToDamage[i].gameObject.tag;

                    // Reduce HP of dummy targets
                    if (enemyTag.Equals("Dummy Targets"))
                    {
                        enemiesToDamage[i].GetComponent<Interactables>().health -= damage;
                    }
                    
                    // Reduce HP of enemy objects
                    if (enemiesToDamage[i].GetComponent<EnemyController>() != null)
                    {
                        enemiesToDamage[i].GetComponent<EnemyController>().health -= damage;
                    }
                }
            }

        } 
        else
        {
            // Reduce timer
            timeBetweenAttacks -= Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
    
}
