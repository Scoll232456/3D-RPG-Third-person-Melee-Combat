using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmenyManager : MonoBehaviour
{
    public Vector2 timeRangeBetweenAttacks = new Vector2(1,4);

    private List<EnemyController> enemiesInRange = new List<EnemyController>();
    private float notAttackingTimer = 2f;
    public static EmenyManager i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    private void Update()
    {
        if (enemiesInRange.Count == 0) { return; }
        if (!enemiesInRange.Any(e => e.IsInState(EnemyState.Attack)))
        {
            if (notAttackingTimer > 0)
            {
                notAttackingTimer -= Time.deltaTime;
            }
            else
            {
                // 攻击间隔时间已到，选择一个敌人来攻击玩家
                var attackingEnemy = SelectEnemyToAttack();
                attackingEnemy.ChangeState(EnemyState.Attack);
                notAttackingTimer = UnityEngine.Random.Range(timeRangeBetweenAttacks.x,
                    timeRangeBetweenAttacks.y);
            }
        }

    }

    public void AddEnemyInRange(EnemyController enemy)
    {
        if(!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    public void RemoveEnemyInRange(EnemyController enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    private EnemyController SelectEnemyToAttack()
    {
        return enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();
    }
}
