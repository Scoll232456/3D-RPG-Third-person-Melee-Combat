using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatAfterAttackState : State<EnemyController>
{
    public float backwardWalkSpeed = 1.5f;
    public float distanceToRetreat = 4f;

    EnemyController enemy;
    Vector3 tartgetPos;
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        tartgetPos = enemy.Target.transform.position;
    }

    public override void Execute()
    {
        if (Vector3.Distance(enemy.transform.position , tartgetPos) >= distanceToRetreat) 
        {
            enemy.ChangeState(EnemyState.CombatMovement);
            return;
        }

        var vecToTarget = enemy.Target.transform.position - enemy.transform.position;
        enemy.NavMeshAgent.Move(-vecToTarget.normalized * backwardWalkSpeed * Time.deltaTime);

        vecToTarget.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(vecToTarget),500 * Time.deltaTime);
    }

    public override void Exit()
    {

    }
}
