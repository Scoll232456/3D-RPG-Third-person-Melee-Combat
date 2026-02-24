using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackHitBox 
{
    LeftHand,
    RightHand,
    LeftFoot,
    RightFoot,
    Sword
}

// 在外部的类中应用，但不能修改，且可以在unity编辑器的Project中创建并修改。
[CreateAssetMenu(menuName ="Combat System/Create a new attack")]
public class AttackData : ScriptableObject
{ 
    [field:SerializeField]public string AnimName { private set; get; }
    [field:SerializeField]public AttackHitBox _AttackHitBox { private set; get; }
    [field: SerializeField] public float ImpactStartTime { private set; get; }
    [field: SerializeField] public float ImpactEndTime { private set; get; }
}
