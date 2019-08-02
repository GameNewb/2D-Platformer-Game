using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBetweenAttacks;
    public float startTimeBetweenAttacks;

    public Transform attackPosition;
    public LayerMask whatIsEnemy;
    public Animator playerAnimator;
    public float attackRange;
    public int damage;

    public GameObject animationObject;
    GameObject tempObject;

    private void Update()
    {
        if (timeBetweenAttacks <= 0)
        {
            // Destroy attack object after animation
            if (tempObject != null)
            {
                Destroy(tempObject);
            }

            if (Input.GetKey(KeyCode.X)) {
                tempObject = Instantiate(animationObject, attackPosition.position, transform.rotation);
                tempObject.SetActive(true);

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
