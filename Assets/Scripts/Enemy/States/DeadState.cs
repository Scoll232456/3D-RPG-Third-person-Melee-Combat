using System.Collections;
using UnityEngine;


public class DeadState : State<EnemyController>
{
    EnemyController enemy;
    public override void Enter(EnemyController owner)
    {
        owner.Visioner.gameObject.SetActive(false);
        EnemyManager.i.RemoveEnemyInRange(owner);
        

        owner.NavMeshAgent.enabled = false;
        owner.CharacterController.enabled = false;
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }

}
