using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : FSMState
{
    EnemyController enemyControl;

    public DeadState(EnemyController enemy)
    { 
        stateID = FSMStateID.Dead;
        enemyControl = enemy;
    }

    public override void EnterStateInit()
    {
        // Final implementation, play animation then recycle back to manager.

        // Upon entering the state, check if I should be dead

        Debug.Log("[DeathState]: Entity died.");
        if (enemyControl.stats.Health.Value <= 0)
        { 
            GameObject.Destroy(enemyControl.gameObject);    // Destroy the enemy
        }

        // Also need to cleanup this state? Don't know if it will be cleaned up by object destruction
    }

    public override void Reason(Transform player, Transform npc)
    {

    }
    public override void Act(Transform player, Transform npc)
    {

    }

}
