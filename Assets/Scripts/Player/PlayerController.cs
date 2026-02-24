using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("MoveAndDir")]
    public float MoveSpeed = 10f;

    Quaternion TargetRotation;

    CameraController _CameraController;

    public float RotationSpeed = 400f;

    Animator _Animator;

    CharacterController _CharacterController;

    [Header("Ground Check Setting")]
    public float groundCheckRadius = 0.2f;
    public Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0.1f);
    public LayerMask groundLayer;
    bool isGround;
    float SpeedY;

    [Header("Fight")]
    MeeleFighter meeleFighter;

    private void Start()
    {
        _CameraController = Camera.main.GetComponent<CameraController>();
        _Animator = GetComponent<Animator>();
        _CharacterController = GetComponent<CharacterController>();
        meeleFighter = GetComponent<MeeleFighter>();

    }
    void Update()
    {
        if (meeleFighter.InAction) 
        {
            _Animator.SetFloat("ForwardSpeed", 0);
            return; 
        }

        // WASD操作角色
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        var MoveAmount = Mathf.Clamp01( Mathf.Abs(h) + Mathf.Abs(v));

        // 未转动摄像机时，主角的移动方向
        var MoveDir_hv = (new Vector3(h,0,v)).normalized;

        // 按键且转动摄像机之后，主角的最终移动方向 只能绕Y轴转动
        var MainRoleMoveDir = _CameraController.PlanerRotation * MoveDir_hv;

        // 主角是否碰到地面的检测
        GroundCheck();

        if (isGround)
        {
            SpeedY = -0.5f;
        }
        else
        {
            SpeedY += Physics.gravity.y * Time.deltaTime;
        }

        // 主角的速度(包含了大小和方向)
        var MainRoleVelocity = MainRoleMoveDir * MoveSpeed;
        MainRoleVelocity.y = SpeedY;

        // 主角的模型朝最终方向移动
        _CharacterController.Move(MainRoleVelocity * Time.deltaTime);

        // 使得主角的正面与其移动地最终方向保持一致
        if ( MoveAmount > 0 ) 
        {
            // transform.position += MainRoleMoveDir * MoveSpeed * Time.deltaTime;
            TargetRotation = Quaternion.LookRotation(MainRoleMoveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            TargetRotation, RotationSpeed * Time.deltaTime);

        // 添加移动的动画效果，不再仅仅是模型的平移
        _Animator.SetFloat("ForwardSpeed", MoveAmount,0.1f,Time.deltaTime);
    }
    void GroundCheck()
    {
        isGround = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0,1,0);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset),groundCheckRadius);
    }
}
