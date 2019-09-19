using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeCheck : MonoBehaviour
{
    [SerializeField] private EnemyController enemyObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Change direction when player is hit and keep moving
        if (collision.tag == "Player" && !enemyObject.leapingEnemy)
        {
            enemyObject.ChangeDirection();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enemyObject.ChangeDirection();
    }

}
