using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
    // ¹¥»÷¾àÀë
    public float attackDistance;

    EnemyController enemy;

    private bool isAttacking = false;

    public override void Enter(EnemyController owner)
    {
        enemy = owner;

        enemy.navMeshAgent.stoppingDistance = attackDistance;
    }
    public override void Execute()
    {
        if (isAttacking || enemy.Target == null) { return; }

        enemy.navMeshAgent.SetDestination(enemy.Target.transform.position);

        if (Vector3.Distance(enemy.Target.transform.position, 
            enemy.transform.position) <= attackDistance + 0.03f) 
        {
            StartCoroutine(Attack(Random.Range(0, enemy.Fighter.attacks.Count + 1)));
        }
    }
    public override void Exit()
    {
        enemy.navMeshAgent.ResetPath();
    }

    IEnumerator Attack(int comboCount = 1)
    {
        isAttacking = true;
        enemy.animator.applyRootMotion = true;

        enemy.Fighter.TryToAttack();

        for (int i = 1;i< comboCount; i++ )
        {
            yield return new WaitUntil(() => enemy.Fighter.AttackState == MeeleFighterAttackState.CoolDown);
            enemy.Fighter.TryToAttack();
        }

        yield return new WaitUntil(()=>enemy.Fighter.AttackState == MeeleFighterAttackState.Idle);

        enemy.animator.applyRootMotion = false;
        isAttacking = false;

        enemy.ChangeState(EnemyState.RetreatAfterAttack);
    }
}
