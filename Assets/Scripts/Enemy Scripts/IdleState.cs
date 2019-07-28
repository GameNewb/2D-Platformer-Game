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
        //Debug.Log(enemy.animator);
        //enemy.animator.SetBool("IsMoving", true);
        //enemy.animator.SetFloat("Speed", 0);

        idleTimer += Time.fixedDeltaTime;

        if (idleTimer >= idleDuration)
        {
            enemyObject.ChangeState(new PatrolState());
        }
    }
}
