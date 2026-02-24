using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 在当前状态下，会有不同的反应
public enum AICombatState
{
    Idle,
    Chase,
    Circling
}

public class CombatMovementState : State<EnemyController>
{
    public float CirclingSpeed = 20f;
    public Vector2 IdleTimeRange = new Vector2(2,5);
    public Vector2 CirclingTimeRange = new Vector2(3,6);
    float timer = 0f; // 使用一个计时器来度量状态的时长
    int circlDir = 1;

    AICombatState _State;
    public float _AdjustDistance = 1f; // 增加一个微小量，避免玩家轻微移动就开始追击
    public float stopdistance = 4f;
    EnemyController enemy;

    // 进入范围之后，进行初始化
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.navMeshAgent.stoppingDistance = stopdistance;
        enemy.CombatMovementTimer = 0f;
    }
    public override void Execute()
    {
        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) > stopdistance + _AdjustDistance)
        StartChase();

        if (_State == AICombatState.Idle)
        {
            if (timer <= 0)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    StartIdle();
                }
                else
                { 
                    StartCircling(); 
                }
            }
        }
        else if (_State == AICombatState.Chase)
        {
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= stopdistance + 0.05f)
            {
                StartIdle();
                return; 
            }
            enemy.navMeshAgent.SetDestination(enemy.Target.transform.position);
        }
        else if (_State == AICombatState.Circling)
        {
            if (timer <= 0) 
            {
                StartIdle();
                return;
            }
            // transform.RotateAround(enemy.Target.transform.position, Vector3.up,CirclingSpeed * circlDir * Time.deltaTime);
            var vecToVector = enemy.transform.position - enemy.Target.transform.position;
            var rotatedPos = Quaternion.Euler(0, CirclingSpeed * circlDir * Time.deltaTime,0) * vecToVector;
            enemy.navMeshAgent.Move(rotatedPos - vecToVector);
            enemy.transform.rotation=Quaternion.Euler( - vecToVector);
        }
        
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        enemy.CombatMovementTimer += Time.deltaTime;
    }

    public override void Exit()
    {
        Debug.Log("离开追逐状态");
    }

    private void StartChase()
    {
        _State = AICombatState.Chase;
        enemy.animator.SetBool("CombatMode", false);
    }
    private void StartIdle()
    {
        _State = AICombatState.Idle;
        timer = UnityEngine.Random.Range(IdleTimeRange.x, IdleTimeRange.y);

        enemy.animator.SetBool("CombatMode", true);
    }
    private void StartCircling()
    {
        _State = AICombatState.Circling;

        enemy.navMeshAgent.ResetPath();
        timer = UnityEngine.Random.Range(CirclingTimeRange.x, CirclingTimeRange.y);

        circlDir = UnityEngine.Random.Range(0,2) == 0 ? 1 : -1;
        // enemy.animator.SetBool("CombatMode", false);
    }

}
