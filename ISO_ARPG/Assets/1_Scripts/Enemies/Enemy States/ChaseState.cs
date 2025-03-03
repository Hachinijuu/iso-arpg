using System.IO.Pipes;
using UnityEngine;

public class ChaseState : FSMState
{
    #region VARIABLES
    EnemyController controller;
    Animator anim;
    bool moving;
    float speed;

    private int moveAnimID = Animator.StringToHash("Speed");
    #endregion
    public ChaseState(EnemyController enemy)
    {
        stateID = FSMStateID.Chase;
        curSpeed = 3.0f;
        controller = enemy;
        moving = true;
        controller.navMeshAgent.speed = curSpeed;
        anim = controller.GetComponent<Animator>();
    }
    
    #region FUNCTIONALITY
    public override void EnterStateInit()
    {
        //Debug.Log("[FSM_Chase]: Entered state");

        // Enter State
        moving = true;
        //Release slot position
        // enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.navMeshAgent.gameObject);

        
        //destPos = controller.transform.position;    // Set the position to where they are, this gets changed when within chase range

        if (controller.navMeshAgent != null)
        {
            controller.navMeshAgent.speed = curSpeed;

            controller.navMeshAgent.destination = destPos;
            if (moving)
                controller.navMeshAgent.isStopped = false;  // Allow the agent to move
        }

    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        // If the player has entered the ranged attack range, transition to the ranged state
        //float dist = GetSquareDistance(destPos, npc.position);

        // If the agent is inside the ranged area
        // Only check for this if the controller has a ranged state
        if (controller as ImpController)
        {
            if (IsInCurrentRange(npc, destPos, EnemyController.RANGEATTACK_DIST))
            {
                //Debug.Log("[FSM_ChaseState] Transitioned to Ranged");
                controller.PerformTransition(Transition.ReachPlayer);
                return;
            }
        }
        // If the player has entered the melee attack range, transition to the melee attack state
        if (IsInCurrentRange(npc, destPos, EnemyController.MELEEATTACK_DIST))
        {
            //Debug.Log("[FSM_ChaseState] Transitioned to Melee");
            controller.PerformTransition(Transition.PlayerReached);
            return;
        }
        // If the AGENT is in the player's CHASE RANGE
        else if (IsInCurrentRange(player, npc.transform.position, EnemyController.CHASE_DIST))
        {
            destPos = player.position;  // Only update the position if the player is in range
        }
        else
        {
            destPos = player.position;  // Only update the position if the player is in range
        }
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        //Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //controller.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime);
        
        // NEED TO REDUCE THIS LOAD, IT IS BEING CALLED EVERY FRAME -- DON'T KNOW IF UPDATE FREQUENCY IS BROKEN
        // IT MIGHT BE BROKEN, THEREFORE ASSUME THIS IS HAPPENING EVERY FRAME

        // WILL IMPLEMENT ANIMATOR LODS TO REDUCE STEP TIME
        // FIND OUT A WAY TO REDUCE NAVMESH CALLS / OFFLOAD WEIGHT

        if (moving)
        {
            controller.navMeshAgent.destination = destPos;
            speed = controller.navMeshAgent.velocity.magnitude;
            anim.SetFloat(EnemyController.moveAnimID, speed);
        }
    }
    #endregion
}
