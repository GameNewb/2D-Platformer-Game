using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float animatorTime;
    private float timeBetweenAttacks;
    private string enemyTag;
    public float startTimeBetweenAttacks;

    public Transform attackPosition;
    public LayerMask whatIsEnemy;
    public Rigidbody2D rigidBody2D;
    public Animator playerAnimator;
    public float attackRange;
    public int damage;

    private void Start()
    {
        animatorTime = GetComponent<AnimationManager>().GetAnimationLength("Ninja Attack 1");
    }

    private void FixedUpdate()
    {
        if (timeBetweenAttacks <= 0)
        {
            // Reset animator bool
            playerAnimator.SetBool("IsAttacking", false);

            // X to do Attack 1
            if (Input.GetKeyDown(KeyCode.X)) {
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
