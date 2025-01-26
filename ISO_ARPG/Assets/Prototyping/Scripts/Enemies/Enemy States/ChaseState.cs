using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : FSMState
{
    
    EnemyController controller;
    Animator anim;
    bool moving;
    int availableSlotIndex;
    float speed;

    float square_rangeDist;
    float square_meleeDist;
    float square_chaseDist;
    public ChaseState(EnemyController enemy)
    {
        stateID = FSMStateID.Chase;
        curSpeed = 3.0f;        
        controller = enemy;
        moving = true;
        controller.navMeshAgent.speed = curSpeed;
        anim = controller.GetComponent<Animator>();
        availableSlotIndex = -1;

        square_rangeDist = EnemyController.RANGEATTACK_DIST * EnemyController.RANGEATTACK_DIST;
        square_meleeDist = EnemyController.MELEEATTACK_DIST * EnemyController.MELEEATTACK_DIST;
        square_chaseDist = EnemyController.CHASE_DIST * EnemyController.CHASE_DIST;
    }
    public override void EnterStateInit()
    {
        // Enter State
        moving = true;
        //Release slot position
       // enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.navMeshAgent.gameObject);
        controller.navMeshAgent.speed = curSpeed;
        availableSlotIndex = -1;

        if (moving)
            controller.navMeshAgent.isStopped = false;  // Allow the agent to move


        //  enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.navMeshAgent.gameObject);
        // availableSlotIndex = -1;
        // // Reserve the Position
        // availableSlotIndex = enemyControl.playerSlotManager.ReserveSlotAroundObject(enemyControl.gameObject);
        // if (availableSlotIndex != -1)
        // {
        //     destPos = enemyControl.playerSlotManager.GetSlotPosition(availableSlotIndex);
        // }

    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {


        //Release slot position
       // enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.navMeshAgent.gameObject);
       // availableSlotIndex = -1;
        // Reserve the Position
       // availableSlotIndex = enemyControl.playerSlotManager.ReserveSlotAroundObject(enemyControl.gameObject);
       // if (availableSlotIndex != -1)
       // {
       //     destPos = enemyControl.playerSlotManager.GetSlotPosition(availableSlotIndex);
       // }
        //NPC DEATH
        //if (enemyControl.stats.Health.Value == 0)
        //{
        //    // enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.gameObject);
        //    Debug.Log("[ChaseState]: Performing death transition");
        //    enemyControl.PerformTransition(Transition.NoHealth);
//
        //    return;
        //}
      //  // ORDER DOES MATTER
      //  //PLAYER IS IN MELEE RANGE
      //  if (IsInCurrentRange(npc, destPos, EnemyController.MELEEATTACK_DIST))
      //  {
      //      moving = false;
      //      enemyControl.PerformTransition(Transition.ReachPlayer);
      //  }
      //  //PLAYER IS IN RANGED RANGE
      //  else if (IsInCurrentRange(npc, destPos, EnemyController.RANGEATTACK_DIST))
      //  {
      //      moving = false; //close to the player so stop moving
      //      enemyControl.PerformTransition(Transition.ReachPlayer);
      //  }

        // If the player has entered the ranged attack range, transition to the ranged state
        float dist = GetSquareDistance(npc, destPos);
        // Ranged attack range is greater than melee, less than or equal to ranged
        if (dist > square_rangeDist && dist <= EnemyController.RANGEATTACK_DIST)
        {
            controller.PerformTransition(Transition.ReachPlayer);
        }
        // If the player has entered the melee attack range, transition to the melee attack state
        else if (dist <= square_meleeDist)
        {
            controller.PerformTransition(Transition.PlayerReached);
        }
        else
        {
            destPos = player.position;
        }
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        //Debug.Log("ACT!");
        //Rotate towards Position
        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //Snap
        //enemyControl.navMeshAgent.transform.rotation = targetRotation; 
        // Slower Rotation
        controller.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime );
        if (moving)
        {

            destPos = player.position;
            //move to destination
            controller.navMeshAgent.destination = destPos;
            speed = controller.navMeshAgent.velocity.magnitude;
            anim.SetFloat("Speed",speed);
            //Debug.Log("Chase");
        }
        

    }

}
