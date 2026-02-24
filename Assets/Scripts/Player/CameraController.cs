using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform TargetFollow;
    // public Vector3 RealFollowPos;
    public float Distance = 5f;

    public float RotationSpeed = 2.5f;
    private float RotationX;
    public float MaxRotationX = 90;
    public float MinRotationX = -10f;
    
    private float RotationY;

    public Vector3 MainCameraOffect;

    // 改变因鼠标滑动而转向的方向
    private int InvertValueX;
    private int InvertValueY;
    public bool InvertX;
    public bool InvertY;

    // ---- 防穿墙相关参数 ----
    [Header("Camera Collision")]
    public LayerMask ObstacleMask = 6;       // 哪些层级会被视为障碍物（默认所有）
    public float CollisionRadius = 0.2f;      // 射线检测的球体半径，使检测更稳定
    public float MinDistance = 0.4f;           // 相机离角色的最小距离
    public float SmoothTime = 0.1f;            // 避让位置平滑移动的时间

    private float currentDistance;             // 实际计算出的距离
    private float velocityDistance;            // 用于 SmoothDamp 的引用变量

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentDistance = Distance;            // 初始化为理想距离
    }

    private void Update()
    {
        // 通过鼠标滑动来操作相机的移动
        InvertValueX = (InvertX) ? 1 :-1 ;
        InvertValueY = (InvertY) ? 1 :-1 ;
        // RotationX -= Input.GetAxis("Mouse Y") * RotationSpeed * InvertValueX;
        RotationX -= Input.GetAxis("Camera Y") * RotationSpeed * InvertValueX;
        RotationX = Mathf.Clamp(RotationX, MinRotationX, MaxRotationX);
        
        // RotationY += Input.GetAxis("Mouse X") * RotationSpeed * InvertValueY;
        RotationY += Input.GetAxis("Camera X") * RotationSpeed * InvertValueY;

        var TargetRotation = Quaternion.Euler(RotationX, RotationY, 0);
        var FoucsPostion = TargetFollow.position + MainCameraOffect;

        //Vector3 CameraPosInfluenceByMouse;
        //if (RotationX < 0)
        //{
        //    var ApproximateDown = (RotationX - MinRotationX) / (4 * Mathf.Abs(MinRotationX));

        //    CameraPosInfluenceByMouse = FoucsPostion -
        //          TargetRotation * new Vector3(0, 0, Distance) * ApproximateDown + new Vector3(0, 0.8f, 0);
        //}
        //else
        //{
        //    CameraPosInfluenceByMouse = FoucsPostion -
        //          TargetRotation * new Vector3(0, 0, Distance);
        //}

        // 先算出“理想”的相机位置（即没有障碍物时，只凭借玩家操作鼠标移动的相机的位置）
        Vector3 desiredCameraPos = FoucsPostion -
        TargetRotation * new Vector3(0, 0, Distance);

        // ---- 防穿墙计算 ----
        // 使用射线检测（或 SphereCast）检查从目标点到期望相机位置之间是否有障碍物
        RaycastHit hit;
        Vector3 direction = (desiredCameraPos - FoucsPostion).normalized;
        float maxCheckDistance = Vector3.Distance(desiredCameraPos, FoucsPostion);

        if (Physics.SphereCast(FoucsPostion, 
            CollisionRadius, direction, 
            out hit, maxCheckDistance, ObstacleMask))
        {
            // 如果碰到障碍物，将相机位置放在碰撞点稍微靠前的位置
            Debug.Log("相机发出的球形射线碰到了"+ hit.transform.name);
            float safeDistance = Mathf.Max(hit.distance - CollisionRadius, MinDistance);
            currentDistance = Mathf.SmoothDamp(currentDistance, safeDistance, ref velocityDistance, SmoothTime);
        }
        else
        {
            // 没有障碍物，平滑恢复到理想距离
            currentDistance = Mathf.SmoothDamp(currentDistance, Distance, ref velocityDistance, SmoothTime);
        }

        // 根据计算出的当前距离重新计算相机位置
        Vector3 finalCameraPos = FoucsPostion - TargetRotation * Vector3.forward * currentDistance;

        // 相机最终所处的位置和旋转度
        transform.position = finalCameraPos;
        transform.rotation = TargetRotation;
    }


    public Quaternion PlanerRotation => Quaternion.Euler(0,RotationY,0);
    public Quaternion GetPlanerRotation()
    {
        return Quaternion.Euler(0, RotationY, 0);
    }
}
