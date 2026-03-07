using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

// 近战单位每段攻击都有的节点
public enum MeeleFighterAttackState
{
    Idle,
    WindUp,
    Impact,
    CoolDown
}

public class MeeleFighter : MonoBehaviour
{
    public List<AttackData> attacks;
    public List<AttackData> longRangeAttacks;
    public float longRangeAttackThreshold = 1.5f;

    public GameObject WeaponLargeSword;
    BoxCollider WeaponLargeSwordCollider;
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;
    public bool InAction { private set; get; } = false;

    public event Action OnGotHit;
    public event Action OnHitComplete;

    Animator animator;
    public MeeleFighterAttackState AttackState { get; private set; }
    public bool InCounter { set; get; } = false;

    public float rotationSpeed = 500f;

    // 连招，是否触发连招，连招数量
    private bool DoCombo;
    private int ComboCount = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (WeaponLargeSword != null)
        {
            WeaponLargeSwordCollider = WeaponLargeSword.GetComponent<BoxCollider>();

            leftHandCollider = animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
            rightHandCollider = animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
            leftFootCollider = animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
            rightFootCollider = animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();

            DisableAllCollider();
        }
    }

    public void TryToAttack(MeeleFighter target = null)
    {
        if (!InAction)
        {
            StartCoroutine(Attack(target));
        }
        else if (AttackState == MeeleFighterAttackState.Impact || AttackState == MeeleFighterAttackState.CoolDown)
        {
            DoCombo = true;
        }
    }

    IEnumerator Attack(MeeleFighter target = null)
    {
        InAction = true; // 避免反复触发攻击动作
        AttackState = MeeleFighterAttackState.WindUp; // 攻击状态由初始状态进入抬手状态

        var attack = attacks[ComboCount];

        var attackDir = transform.forward;
        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.zero;
        if (target != null) 
        {
            var vecTarget = target.transform.position - transform.position;
            vecTarget.y = 0;

            attackDir = vecTarget.normalized;
            float distance = vecTarget.magnitude;

           
            if (distance > longRangeAttackThreshold) 
            {
                attack = longRangeAttacks[0];
            }

            if (attack.MoveToTarget)
            {
                if (distance < attack.MaxMoveDistance)
                {
                    targetPos = target.transform.position
                        - attackDir * attack.DistanceFromTarget;
                }
                else 
                { 
                    targetPos = startPos + attackDir * attack.MaxMoveDistance; 
                }
            }
        }

        // CrossFadeInFixedTime是异步的，放在前面
       // animator.CrossFadeInFixedTime(attack.AnimName, 0.2f);
        animator.CrossFade(attack.AnimName, 0.2f);
        yield return null;

        var AnimatorState = animator.GetNextAnimatorStateInfo(1); // 切换Layer至 OverrideLayer

        // 声明一个变量来衡量动画的进度
        float timer = 0f;
        while (timer <= AnimatorState.length)
        {
            timer += Time.deltaTime;
            float AnimationProgressRatio = timer / AnimatorState.length;

            // 向目标移动并攻击
            if (target != null && attack.MoveToTarget) 
            {
                transform.position = Vector3.Lerp(startPos,targetPos, AnimationProgressRatio);
            }

            if (attackDir != null) 
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir),
                    rotationSpeed * Time.deltaTime); 
            }

            if (AttackState == MeeleFighterAttackState.WindUp)
            {
                if (InCounter) { break; }
                if (AnimationProgressRatio > attack.ImpactStartTime)
                {
                    AttackState = MeeleFighterAttackState.Impact; // 进入碰撞状态

                    EnableHitBox(attack);
                }
            }
            else if (AttackState == MeeleFighterAttackState.Impact)
            {
                if (AnimationProgressRatio > attack.ImpactEndTime)
                {
                    AttackState = MeeleFighterAttackState.CoolDown; // 进入冷却状态

                    DisableAllCollider(); // 禁用所有碰撞器
                }
            }
            else if (AttackState == MeeleFighterAttackState.CoolDown)
            {
                // 可以在这个阶段接上连击
                if (DoCombo)
                {
                    DoCombo = false;
                    ComboCount = (ComboCount + 1) % attacks.Count;
                    StartCoroutine(Attack());
                    yield break;
                }
            }
            yield return null;
        }
        AttackState = MeeleFighterAttackState.Idle;
        ComboCount = 0;
        yield return new WaitForSeconds(AnimatorState.length * 0.1f);

        InAction = false;
    }
    IEnumerator PlayerHitReaction(Transform attacker)
    {
        InAction = true;

        // 受到攻击的方向
        var Vect = attacker.transform.position - transform.position;
        Vect.y = 0;

        OnGotHit?.Invoke();

        animator.CrossFadeInFixedTime("AttackImpact", 0.2f);
        yield return null;

        var AnimatorState = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(AnimatorState.length);

        OnHitComplete?.Invoke();

        InAction = false;
    }

    public IEnumerator PreformCombatAttack(EnemyController opponent)
    {
        InAction = true;

        InCounter = true;
        opponent.Fighter.InCounter = true;
        opponent.ChangeState(EnemyState.Dead);

        var disVector = opponent.transform.position - transform.position;
        disVector.y = 0f;
        transform.rotation = Quaternion.LookRotation(disVector);
        opponent.transform.rotation = Quaternion.LookRotation(-disVector);

        var targetPos = opponent.transform.position - disVector.normalized * 1f;

        animator.CrossFade("CounterAttack", 0.2f);
        opponent.animator.CrossFade("CounterAttackVictim", 0.2f);
        yield return null;

        var AnimatorState = animator.GetNextAnimatorStateInfo(1);

        float timer = 0f;
        while (timer <= AnimatorState.length)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);

            yield return null;

            timer += Time.deltaTime;
        }

        InCounter = false;
        opponent.Fighter.InCounter = false;

        InAction = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HitBox" && !InAction )// && this.tag == "Enemy"
        {
            StartCoroutine( PlayerHitReaction(other.GetComponentInParent<MeeleFighter>().transform) );
        }
    }

    void EnableHitBox(AttackData attack)
    {
        switch (attack._AttackHitBox)
        {
            case AttackHitBox.LeftHand:
                leftHandCollider.enabled = true;
                break;
            case AttackHitBox.RightHand:
                rightHandCollider.enabled = true;
                break;
            case AttackHitBox.LeftFoot:
                leftFootCollider.enabled = true;
                break;
            case AttackHitBox.RightFoot:
                rightFootCollider.enabled = true;
                break;
            case AttackHitBox.Sword:
                WeaponLargeSwordCollider.enabled = true;
                break;
            default:
                break;
        }
    }

    void DisableAllCollider()
    {
        if (WeaponLargeSwordCollider != null) 
        {
            WeaponLargeSwordCollider.enabled = false; 
        }

        if (leftHandCollider != null)
        {
            leftHandCollider.enabled = false;
        }
        if (rightHandCollider != null)
        {
            rightHandCollider.enabled = false;
        }
        if (leftFootCollider != null)
        {
            leftFootCollider.enabled = false;
        }
        if (rightFootCollider != null) 
        { 
            rightFootCollider.enabled = false;
        }
    }

    public List<AttackData> Attacks => attacks;
    public bool IsCounterable => AttackState == MeeleFighterAttackState.WindUp && ComboCount == 0;
}
