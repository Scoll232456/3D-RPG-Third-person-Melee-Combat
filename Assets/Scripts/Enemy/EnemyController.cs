using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyState
{
    Idle,
    CombatMovement,
    Attack,
    RetreatAfterAttack,
    Dead
}

public class EnemyController : MonoBehaviour
{
    // 敌人正面朝向所拥有的视野
    [field: SerializeField] public float Fov { get; private set; } = 180f;

    // 存储进入视野范围内的目标，如：玩家、玩家友方
    public List<MeeleFighter> TargetsInRange { get; set; } = new List<MeeleFighter>();
    public StateMachine<EnemyController> StateMachine { get; private set; }
    public MeeleFighter Target { get;set; }
    public float CombatMovementTimer { get; set; } = 0f;

   
    public Dictionary<EnemyState, State<EnemyController>> stateDict { get;private set; }

     public NavMeshAgent NavMeshAgent { get; private set; }
     public CharacterController CharacterController { get; private set; }
     public Animator animator { get; private set; }
     public MeeleFighter Fighter { get; private set; }
     public SkinnedMeshHighlighter MeshHighlighter { get; private set; }
     public VisionSensor Visioner { get;  set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        CharacterController = GetComponent<CharacterController>();
        Fighter = GetComponent<MeeleFighter>();
        MeshHighlighter = GetComponent<SkinnedMeshHighlighter>();

        stateDict = new Dictionary<EnemyState, State<EnemyController>>();
        stateDict[EnemyState.Idle] = GetComponent<IdleState>();
        stateDict[EnemyState.CombatMovement] = GetComponent<CombatMovementState>();
        stateDict[EnemyState.Attack] = GetComponent<AttackState>();
        stateDict[EnemyState.RetreatAfterAttack] = GetComponent<RetreatAfterAttackState>();
        stateDict[EnemyState.Dead] = GetComponent<DeadState>();

        StateMachine = new StateMachine<EnemyController>(this);

         StateMachine.ChangeState(stateDict[EnemyState.Idle]);
    }

    private Vector3 PrePos;
    private void Update()
    {
        StateMachine.Execute();

        // 求出其在这一帧的瞬时速度
        // 当前帧所处的位置减去上一帧所处的位置除以两帧的间隔时间
        var DeltaPos = animator.applyRootMotion? Vector3.zero : transform.position - PrePos;
        var Velocity = DeltaPos / Time.deltaTime;

        float ForwardSpeed = Vector3.Dot(Velocity,transform.forward);

        animator.SetFloat("ForwardSpeed", ForwardSpeed / NavMeshAgent.speed,0.2f,Time.deltaTime);
        
        float angle = Vector3.SignedAngle(transform.forward, Velocity, Vector3.up);
        float StrafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
        animator.SetFloat("StrafeSpeed", StrafeSpeed, 0.2f, Time.deltaTime);

        PrePos = transform.position;
    }
    public void ChangeState(EnemyState state)
    {
        StateMachine.ChangeState(stateDict[state]);
    }


    public bool IsInState(EnemyState state)
    {
       return StateMachine.CurrentState == stateDict[state];
    }
}
