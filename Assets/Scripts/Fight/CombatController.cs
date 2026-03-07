using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    
    MeeleFighter meeleFighter;

    Animator animator;
    public CameraController cameraController { private set; get; }

    private EnemyController targetEnemy;
    public EnemyController TargetEnemy 
    {
        get { return targetEnemy; }
        set
        {
            targetEnemy = value;

            if (targetEnemy == null)
            {
                comatMode = false;
            }
        }
    }

    // public bool CombatMode{ set; get; }
    private bool comatMode;
    public bool CombatMode 
    { 
        get { return comatMode; } 
        set 
        {
            comatMode = value;

            if(TargetEnemy == null)
            {
                comatMode = false;
            }

            animator.SetBool("CombatMode", comatMode); 
        } 
    }


    private void Awake()
    {
        meeleFighter = GetComponent<MeeleFighter>();
        animator = GetComponent<Animator>();
        cameraController = Camera.main.GetComponent<CameraController>();

    }
    private void Update()
    {
        if (Input.GetButtonDown("CommanAttack")) 
        {
            var enemy = EnemyManager.i.GetAttackingEnemy();
            if (enemy != null && enemy.Fighter.IsCounterable && !meeleFighter.InAction)
            {
                StartCoroutine(meeleFighter.PreformCombatAttack(enemy));
            }
            else 
            {
                var enemyToAttack = EnemyManager.i.GetCloseToPlayerEnemyDirection(PlayerController.i.InputDir);
                // Vector3? dirToAttack = null;
                //if (enemyToAttack != null) 
                //{ dirToAttack = enemyToAttack.transform.position - transform.position; }

                
                meeleFighter?.TryToAttack(enemyToAttack?.Fighter);
                
                CombatMode = true;
            }
        }

        if (Input.GetButtonDown("LockOn") || JoyStickHelper.i.GetAxisDown("LockOnTrigger"))
        {
            CombatMode = !CombatMode;
        }
    }
    private void OnAnimationMove()
    {
        transform.position += animator.deltaPosition;
        transform.rotation *= animator.deltaRotation;
    }

    // 1.玩家能够锁定的范围
    // 只有当目标处于摄像机正面朝向的方向时 才能被玩家锁定

    // 2.摄像机正面朝向的向量与敌人位置减去摄像机位置所成的向量的夹角
    public Vector3 GetTargetingDir()
    {
        if (!CombatMode) 
        {
            var VecFromCam = cameraController.TargetFollow.position + cameraController.MainCameraOffect - cameraController.transform.position;
            // VecFromCam.y = 0;
            return VecFromCam.normalized;
        }
        else 
        { 
            return transform.forward;
        }
    }
}
