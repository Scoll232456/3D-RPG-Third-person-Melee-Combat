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
    public Vector2 IdleTimeRange = new Vector2(2,5); // 区间(2,5)之间随机取一个数为闲置(驾势)时间
    public Vector2 CirclingTimeRange = new Vector2(3,6); // 区间(3,6)之间随机取一个数为徘徊时间
    float timer = 0f; // 使用一个计时器来度量状态的时长
    int circlDir = 1;

    AICombatState _State;
    public float _AdjustDistance = 1f; // 增加一个微小量，避免玩家轻微移动就开始追击
    public float stopdistance = 4f;
    EnemyController enemy;

    // 玩家进入敌人范围之后，进行初始化
    public override void Enter(EnemyController owner)
    {
        enemy = owner;
        enemy.NavMeshAgent.stoppingDistance = stopdistance;
        enemy.CombatMovementTimer = 0f;

        enemy.animator.SetBool("CombatMode", true);
    }
    public override void Execute()
    {
        // 设置范围(a,b)，当玩家距离敌人a,b之间的时候开始追逐
        // 小于a说明距离很近了，准备发动攻击或者在一旁徘徊伺机而动
        // 大于b说明距离太远，脱离仇恨范围
        if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) > stopdistance + _AdjustDistance)
        { 
            StartChase(); 
        }

        if (_State == AICombatState.Idle) // 已经追上了玩家 随机进入闲置(架势)或徘徊状态
        {
            if (timer <= 0) // 闲置(架势)时间结束
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
        else if (_State == AICombatState.Chase) // 处于追逐玩家的过程之中  
        {
            // 判断距离大小 大则继续追逐，小则进入闲置(架势)状态
            if (Vector3.Distance(enemy.Target.transform.position, enemy.transform.position) <= stopdistance + 0.05f)
            {
                StartIdle();
                return; 
            }
            enemy.NavMeshAgent.SetDestination(enemy.Target.transform.position);
        }
        else if (_State == AICombatState.Circling) // 不仅追上了玩家，而且还绕着玩家进行左右徘徊
        {
            if (timer <= 0) // 徘徊时间结束
            {
                StartIdle();
                return;
            }
            // transform.RotateAround(enemy.Target.transform.position, Vector3.up,CirclingSpeed * circlDir * Time.deltaTime);
            var vecToVector = enemy.transform.position - enemy.Target.transform.position;
            var rotatedPos = Quaternion.Euler(0, CirclingSpeed * circlDir * Time.deltaTime,0) * vecToVector;
            enemy.NavMeshAgent.Move(rotatedPos - vecToVector);
            // enemy.transform.rotation = Quaternion.Euler( - vecToVector);
            enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);

        }

        if (timer > 0)
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
        // enemy.animator.SetBool("CombatMode", false);
    }
    private void StartIdle()
    {
        _State = AICombatState.Idle;
        timer = UnityEngine.Random.Range(IdleTimeRange.x, IdleTimeRange.y);

        // enemy.animator.SetBool("CombatMode", true);
    }
    private void StartCircling()
    {
        _State = AICombatState.Circling;

        enemy.NavMeshAgent.ResetPath();
        timer = UnityEngine.Random.Range(CirclingTimeRange.x, CirclingTimeRange.y);

        circlDir = UnityEngine.Random.Range(0,2) == 0 ? 1 : -1;
        // enemy.animator.SetBool("CombatMode", false);
    }
    private void StartReturnOriginPoint()
    {

    }
}
