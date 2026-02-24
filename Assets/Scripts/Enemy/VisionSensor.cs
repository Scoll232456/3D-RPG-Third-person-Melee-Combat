using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    public EnemyController enemy;

    // 将进入范围的目标添加，离开范围的目标移除
    private void OnTriggerEnter(Collider other)
    {
        var fighter = other.GetComponent<MeeleFighter>();
        if(fighter != null)
        {
            enemy.TargetsInRange.Add(fighter);
            EmenyManager.i.AddEnemyInRange(enemy);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var fighter = other.GetComponent<MeeleFighter>();
        if (fighter != null)
        {
            enemy.TargetsInRange.Remove(fighter);
            EmenyManager.i.RemoveEnemyInRange(enemy);
        }
    }
}
