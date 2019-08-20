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
    public int damage;

    [Header("Collider Properties")]
    [Space(2)]
    private Vector3 attackRangeV3;
    private Vector3 boxColliderPosition;
    private Vector2 boxVector2D;
    public float circleAttackRange;
    public float boxAttackRange;
    [Tooltip("Increases the collider distance from the player")]
    public float increaseColliderDistanceX = 0.5f;
    public float increaseColliderDistanceY = 0.2f;
    
    private bool attack1 = false;
    private bool attack2 = false;
    private bool attack3 = false;

    public GameObject hitEffectPrefab1;
    public GameObject hitEffectPrefab2;
    public GameObject hitEffectPrefab3;

    private float parentTransformXScale;

    private void Start()
    {
        animatorTime = GetComponent<AnimationManager>().GetAnimationLength("Ninja Attack 1");
    }

    private void Update()
    {
        // Only let player attack when it's idle
        if (playerAnimator.GetFloat("Speed") == 0f)
        {
            if (timeBetweenAttacks <= 0)
            {
                // Reset animator bool
                playerAnimator.SetBool("IsAttacking1", false);
                playerAnimator.SetBool("IsAttacking2", false);
                playerAnimator.SetBool("IsAttacking3", false);

                attack1 = false;
                attack2 = false;
                attack3 = false;

                // X to do Attack 1
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    attack1 = true;
                    playerAnimator.SetBool("IsAttacking1", true);
                    timeBetweenAttacks = startTimeBetweenAttacks;

                    CollideWithObject();
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    attack2 = true;
                    playerAnimator.SetBool("IsAttacking2", true);
                    timeBetweenAttacks = startTimeBetweenAttacks;

                    // Retrieve parent's X scale
                    parentTransformXScale = gameObject.GetComponentInParent<Transform>().localScale.x;

                    CollideWithObject();
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    attack3 = true;
                    playerAnimator.SetBool("IsAttacking3", true);
                    timeBetweenAttacks = startTimeBetweenAttacks;

                    CollideWithObject();
                }

            }
            else
            {
                // Reduce timer
                timeBetweenAttacks -= Time.deltaTime;
            }
        }
        else
        {
            // Reset animator for attacking when player moves immediately after attacking
            playerAnimator.SetBool("IsAttacking1", false);
            playerAnimator.SetBool("IsAttacking2", false);
            playerAnimator.SetBool("IsAttacking3", false);
        }
        
    }

    private void CollideWithObject()
    {
        Collider2D[] enemiesToDamage;
        boxVector2D = new Vector2(boxAttackRange, boxAttackRange);
        attackRangeV3 = new Vector3(boxAttackRange, boxAttackRange, 0);
        
        if (attack1)
        {
            enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, circleAttackRange, whatIsEnemy);
        }
        else if (attack2)
        {
            // If right side, add collider position
            // If left side, subtract
            if (parentTransformXScale > 0)
            {
                boxColliderPosition = new Vector3(attackPosition.position.x + increaseColliderDistanceX, attackPosition.position.y + increaseColliderDistanceY);
            }
            else
            {
                boxColliderPosition = new Vector3(attackPosition.position.x - increaseColliderDistanceX, attackPosition.position.y + increaseColliderDistanceY);
            }

            enemiesToDamage = Physics2D.OverlapBoxAll(boxColliderPosition, boxVector2D, whatIsEnemy);
        } 
        else
        {
            enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, circleAttackRange, whatIsEnemy);
        }

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

            DisplayHitAnimation(enemiesToDamage[i]);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (attack1)
        {
            Gizmos.DrawWireSphere(attackPosition.position, circleAttackRange);
        }
        else if (attack2)
        {
            Gizmos.DrawWireCube(boxColliderPosition, boxVector2D);
        }
        else if (attack3)
        {
            Gizmos.DrawWireSphere(attackPosition.position, circleAttackRange * 1.3f);
        }
    }

    private void DisplayHitAnimation(Collider2D enemy)
    {
        if (attack1)
        {
            // Apply hit effects on objects with the effect manager
            if (enemy.GetComponent<HitEffectManager>() != null)
            {
                enemy.GetComponent<HitEffectManager>().ShowHitEffects(hitEffectPrefab1);
            }
        }
        else if (attack2)
        {
            // Apply hit effects on objects with the effect manager
            if (enemy.GetComponent<HitEffectManager>() != null)
            {
                enemy.GetComponent<HitEffectManager>().ShowHitEffects(hitEffectPrefab2);
            }
        }
        else if (attack3)
        {
            // Apply hit effects on objects with the effect manager
            if (enemy.GetComponent<HitEffectManager>() != null)
            {
                enemy.GetComponent<HitEffectManager>().ShowHitEffects(hitEffectPrefab3);
            }
        }
    }

}
