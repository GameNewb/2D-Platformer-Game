using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeCheck : MonoBehaviour
{
    [SerializeField] private EnemyController enemyObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        enemyObject.ChangeDirection();
    }
    
}
