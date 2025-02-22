using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class DeadState : FSMState
{
    #region VARIABLES
    EnemyController controller;
    #endregion
    public DeadState(EnemyController npc)
    { 
        stateID = FSMStateID.Dead;
        controller = npc;
    }

    #region FUNCTIONALITY
    public override void EnterStateInit()
    {
        // Final implementation, play animation then recycle back to manager.

        // Upon entering the state, check if I should be dead

        //Debug.Log("[FSM_DeathState]: Entity died.");

        // Instead have AI Manager LISTEN for the death, this can be assigned to entity stats, or an event released from the controller.
        // DeadState will only handle animation logic.

        // Shutoff navmesh
        controller.navMeshAgent.enabled = false;

        // This will remove the lookup to the AIManager
        controller.gameObject.SetActive(false);         // Set the GameObject to false (returned to pool)


        // Once dead, unregister form the list  
        if (DropSystem.Instance != null)
        {
            DropSystem.Instance.UnregisterEnemyDrop(controller.stats);
            //Debug.Log("Unregistered from drops");
        }
        // if (controller.stats.Health.Value <= 0)
        // { 
        //     if (AIManager.Instance)
        //         AIManager.Instance.currAmount -= 1;             // Subtract from the amount of enemies
        //     if (LevelManager.Instance)
        //         LevelManager.Instance.numKilled += 1;           // Add to the number killed
            

        //     // Consideration, individual enemies are set inactive - units, causing them to be inactive
        //     // When the pooledObject is found, it sets the bundles to active, not the children within the bundles.

        //     // Therefore, more cleanup needs to be done on death - enemy bundles need to be resolved

        //     // Enemy bundles should be kept nearby to make this cleanup easier - if they all die at the same time, it is much easier to organize.
            
        //     //GameObject.Destroy(enemyControl.gameObject);    // Destroy the enemy
        // }

        // Also need to cleanup this state? Don't know if it will be cleaned up by object destruction
    }

    public override void Reason(Transform player, Transform npc)
    {

    }
    public override void Act(Transform player, Transform npc)
    {

    }
    #endregion
}
