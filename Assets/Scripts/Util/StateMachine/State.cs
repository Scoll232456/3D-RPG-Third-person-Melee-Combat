using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 控制不同类型单位的状态，T 为单位控制器
public class State<T> : MonoBehaviour
{
    public virtual void Enter(T owner)
    {
        
    }
    public virtual void Execute()
    {

    }
    public virtual void Exit()
    {

    }
}
