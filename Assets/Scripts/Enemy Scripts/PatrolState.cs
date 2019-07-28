using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private EnemyController enemyObject;
    private float patrolTimer;
    private float patrolDuration = 8f;

    public void Enter(EnemyController enemy)
    {
        enemyObject = enemy;
    }

    public void Execute()
    {
        Patrol();

        enemyObject.Move();
    }

    public void Exit()
    {
    }

    public void OnTriggerEnter(Collider2D other)
    {
    }
    
    private void Patrol()
    {
        patrolTimer += Time.fixedDeltaTime;

        if (patrolTimer >= patrolDuration)
        {
            enemyObject.ChangeState(new IdleState());
        }
    }
}
