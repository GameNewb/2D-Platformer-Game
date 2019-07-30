using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyController enemyObject;
    private float idleTimer;
    private float idleDuration = 5f;

    public void Enter(EnemyController enemy)
    {
        enemyObject = enemy;
        enemyObject.stateName = "Idle";
    }

    public void Execute()
    {
        Idle();
    }

    public void Exit()
    {
    }

    public void OnTriggerEnter(Collider2D other)
    {
    }

    private void Idle()
    {
        enemyObject.animator.SetBool("IsMoving", false);
        enemyObject.animator.SetFloat("Speed", 0);
        if (enemyObject.idleOnly)
        {
            enemyObject.ChangeState(new IdleState());
        }
        else if (!enemyObject.patrolOnly)
        {
            idleTimer += Time.fixedDeltaTime;

            if (idleTimer >= idleDuration)
            {
                enemyObject.animator.SetBool("IsMoving", true);
                enemyObject.animator.SetFloat("Speed", 1f);
                enemyObject.ChangeState(new PatrolState());
            }
        }

    }
}
