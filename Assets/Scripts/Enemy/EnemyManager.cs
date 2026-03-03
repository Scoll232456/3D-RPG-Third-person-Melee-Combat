using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Vector2 timeRangeBetweenAttacks = new Vector2(1,4);
    public CombatController player;

    private CameraController cam;
    private List<EnemyController> enemiesInRange = new List<EnemyController>();
    private float notAttackingTimer = 2f;
    public static EnemyManager i { get; private set; }

    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraController>();
        i = this;
    }

    float timer = 0; // 给一个时间管理器，避免按键重复时某个功能一直调用
    EnemyController PreTargetEnemy = null;
    EnemyController LockedTargetEnemy = null;
    bool IsLockEnemy = false;

    GameObject enemyObject;
    public GameObject enemyHealthBar { get; set; }

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

                if (attackingEnemy != null) 
                {
                    attackingEnemy.ChangeState(EnemyState.Attack);
                    notAttackingTimer = UnityEngine.Random.Range(timeRangeBetweenAttacks.x,
                        timeRangeBetweenAttacks.y); 
                }
            }
        }

        if (timer > 0.1f) 
        {
            timer = 0;
            var CloseEnemy = GetCloseToPlayerEnemyDirection(player.GetTargetingDir());
            if(CloseEnemy != null && CloseEnemy != player.TargetEnemy )
            {
                PreTargetEnemy = player.TargetEnemy;

                player.TargetEnemy = CloseEnemy;

                 player?.TargetEnemy?.MeshHighlighter.HighlightMesh(true);
                 PreTargetEnemy?.MeshHighlighter.HighlightMesh(false);
            }
        }
        timer += Time.deltaTime;
        //if (player.TargetEnemy != null && Input.GetKeyDown(KeyCode.Q))
        //{
        //    HideEnemyHealthbar();
        //    LockedTargetEnemy?.MeshHighlighter.HighlightMesh(false);

        //    LockedTargetEnemy = player.TargetEnemy;
        //    DisplayEnemyHealthbar();
        //    LockedTargetEnemy?.MeshHighlighter.HighlightMesh(true);

        //    PreTargetEnemy?.MeshHighlighter.HighlightMesh(false);
        //}

        if(enemyHealthBar != null && enemyHealthBar.activeSelf )
        {
            Vector3 SliderToCameraDir = cam.transform.position - enemyHealthBar.transform.position;
            enemyHealthBar.transform.rotation = Quaternion.LookRotation(SliderToCameraDir);
        }

        // 角色死亡，血条消失
        if(player.TargetEnemy != null && player.TargetEnemy.IsInState(EnemyState.Dead))
        {
            enemyHealthBar.SetActive(false);
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
        if(enemy == player.TargetEnemy)
        {
            enemy.MeshHighlighter?.HighlightMesh(false);
            player.TargetEnemy = GetCloseToPlayerEnemyDirection(player.GetTargetingDir());
            player.TargetEnemy?.MeshHighlighter?.HighlightMesh(true);
        }
    }

    private EnemyController SelectEnemyToAttack()
    {
        return enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault(e =>e.Target != null);
    }

    public EnemyController GetAttackingEnemy()
    {
        return enemiesInRange.FirstOrDefault(e => e.IsInState(EnemyState.Attack));
    }

    public EnemyController GetCloseToPlayerEnemyDirection(Vector3 direction)
    {
        // var targetingDir = player.GetTargetingDir();

        float MinAngle = Mathf.Infinity;
        // float MinDistance = Mathf.Infinity;
        EnemyController closeEnemy = null;
        
        foreach(var enemy in enemiesInRange)
        {
            var VectorCameraToEnemy = enemy.transform.position - player.cameraController.transform.position;
            var angle = Vector3.Angle(direction, VectorCameraToEnemy); 
            // var SinDistance = VectorCameraToEnemy.magnitude * Mathf.Sin(angle * Mathf.Rad2Deg);

            if(angle <= MinAngle)
            {
                MinAngle = angle;
                closeEnemy = enemy;
            }
        }
        return closeEnemy;
    }

    private void DisplayEnemyHealthbar()
    {
        enemyObject = LockedTargetEnemy?.transform.gameObject;
        enemyHealthBar = enemyObject?.transform.Find("HealthBarCanvas").gameObject;
        enemyHealthBar?.SetActive(true);
    }

    private void HideEnemyHealthbar()
    {
        enemyObject = LockedTargetEnemy?.transform.gameObject;
        enemyHealthBar = enemyObject?.transform.Find("HealthBarCanvas").gameObject;
        enemyHealthBar?.SetActive(false);
    }
}

