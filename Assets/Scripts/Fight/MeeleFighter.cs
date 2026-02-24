using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject WeaponLargeSword;
    BoxCollider WeaponLargeSwordCollider;
    SphereCollider leftHandCollider, rightHandCollider, leftFootCollider, rightFootCollider;
    public bool InAction { private set; get; } = false;

    Animator animator;
    public MeeleFighterAttackState AttackState { get; private set; }

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

    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }
        else if (AttackState == MeeleFighterAttackState.Impact || AttackState == MeeleFighterAttackState.CoolDown)
        {
            DoCombo = true;
        }
    }

    IEnumerator Attack()
    {
        InAction = true; // 避免反复触发攻击动作

        AttackState = MeeleFighterAttackState.WindUp; // 攻击状态由初始状态进入抬手状态

        // CrossFadeInFixedTime是异步的，放在前面
        animator.CrossFadeInFixedTime(attacks[ComboCount].AnimName, 0.2f);
        yield return null;

        var AnimatorState = animator.GetNextAnimatorStateInfo(1); // 切换Layer至 OverrideLayer

        // 声明一个变量来衡量动画的进度
        float timer = 0f;
        while (timer <= AnimatorState.length)
        {
            timer += Time.deltaTime;
            float AnimationProgressRatio = timer / AnimatorState.length;
            if (AttackState == MeeleFighterAttackState.WindUp)
            {
                if (AnimationProgressRatio > attacks[ComboCount].ImpactStartTime)
                {
                    AttackState = MeeleFighterAttackState.Impact; // 进入碰撞状态

                    EnableHitBox(attacks[ComboCount]);
                }
            }
            else if (AttackState == MeeleFighterAttackState.Impact)
            {
                if (AnimationProgressRatio > attacks[ComboCount].ImpactEndTime)
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
    IEnumerator PlayerHitReaction()
    {
        InAction = true;
        animator.CrossFadeInFixedTime("AttackImpact", 0.2f);
        yield return null;

        var AnimatorState = animator.GetNextAnimatorStateInfo(1);

        yield return new WaitForSeconds(AnimatorState.length);

        InAction = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HitBox" && !InAction )// && this.tag == "Enemy"
        {
            StartCoroutine(PlayerHitReaction());
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

}
