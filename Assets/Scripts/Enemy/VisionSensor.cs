using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 使用球形碰撞来检测敌人的视野范围
public class VisionSensor : MonoBehaviour
{
    public EnemyController enemy;

    private void Awake()
    {
        enemy.Visioner = this;
    }

    // 将进入范围的目标添加，离开范围的目标移除
    private void OnTriggerEnter(Collider other)
    {
        var fighter = other.GetComponent<MeeleFighter>();
        if(fighter != null)
        {
            enemy.TargetsInRange.Add(fighter);
            EnemyManager.i.AddEnemyInRange(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var fighter = other.GetComponent<MeeleFighter>();
        if (fighter != null)
        {
            enemy.TargetsInRange.Remove(fighter);
            EnemyManager.i.RemoveEnemyInRange(enemy);
        }
    }
}
