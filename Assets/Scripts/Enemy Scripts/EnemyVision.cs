using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private EnemyController enemyObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Target player
        if (collision.tag == "Player")
        {
            enemyObject.chaseObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            enemyObject.chaseObject = null;
        }
    }
}
