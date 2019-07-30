using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
    [SerializeField] private EnemyController enemyObject;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground" && enemyObject.leapingEnemy && enemyObject.stateName == "Patrol")
        {
            enemyObject.AddJumpForce();
        }
    }
}
