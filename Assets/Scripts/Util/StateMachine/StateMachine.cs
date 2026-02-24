using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 单位状态转换器
public class StateMachine<T>
{
    // 某个单位当前的状态
    public State<T> CurrentState { get; private set; }

    T _owner;
    
    // 确定给哪个单位转换状态
    public StateMachine(T owner)
    {
        _owner = owner;
    }

    // 让已经确定的单位，从当前状态离开进入新的状态
    public void ChangeState(State<T> newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter(_owner);
    }

    public void Execute()
    {
        CurrentState?.Execute();
    }
}
//StateMachine<EnemyController>
//StateMachine<NPCController>
