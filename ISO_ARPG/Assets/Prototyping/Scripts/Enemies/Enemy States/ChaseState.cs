using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : FSMState
{
    
    EnemyController enemyControl;
    Animator anim;
    bool moving;
    int availableSlotIndex;
    float speed;

  public ChaseState(EnemyController enemy)
  {
        stateID = FSMStateID.Chase;
        curSpeed = 5.0f;        
        enemyControl = enemy;
        moving = true;
        enemyControl.navMeshAgent.speed = curSpeed;
        anim = enemyControl.GetComponent<Animator>();
        availableSlotIndex = -1;

  }
    public override void EnterStateInit()
    {
        // Enter State
        moving = true;
        //Release slot position
       // enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.navMeshAgent.gameObject);
        enemyControl.navMeshAgent.speed = curSpeed;
        availableSlotIndex = -1;

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
        destPos = player.position;

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
        if (enemyControl.GetHealth() == 0)
        {
           // enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.gameObject);
            enemyControl.PerformTransition(Transition.NoHealth);

            return;
        }
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
       
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        Debug.Log("ACT!");
        //Rotate towards Position
        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //Snap
        //enemyControl.navMeshAgent.transform.rotation = targetRotation; 
        // Slower Rotation
        enemyControl.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime );
        if (moving)
        {

            destPos = player.position;
            //move to destination
            enemyControl.navMeshAgent.destination = destPos;
            speed = enemyControl.navMeshAgent.velocity.magnitude;
            anim.SetFloat("Speed",speed);
            Debug.Log("Chase");
        }
        

    }

}
