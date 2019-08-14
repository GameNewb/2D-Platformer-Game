using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBetweenAttacks;
    public float startTimeBetweenAttacks;

    public Transform attackPosition;
    public LayerMask whatIsEnemy;
    public Rigidbody2D rigidBody2D;
    public Animator playerAnimator;
    public float attackRange;
    public int damage;
    
    private void Update()
    {
        if (timeBetweenAttacks <= 0)
        {
            playerAnimator.SetBool("IsAttacking", false);

            if (Input.GetKey(KeyCode.X)) {
                
                playerAnimator.SetBool("IsAttacking", true);

                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsEnemy);

                for (int i = 0; i < enemiesToDamage.Length; i++)
                {
                    if (enemiesToDamage[i].GetComponent<EnemyController>() != null)
                    {
                        enemiesToDamage[i].GetComponent<EnemyController>().health -= damage;
                    }
                }

                timeBetweenAttacks = startTimeBetweenAttacks;
            }

        } 
        else
        {
            // Reduce timer
            timeBetweenAttacks -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}
