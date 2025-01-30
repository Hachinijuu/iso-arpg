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
        //Debug.Log("[FSM_Chase]: Entered state");

        // Enter State
        moving = true;
        //Release slot position
        // enemyControl.playerSlotManager.ReleaseSlot(availableSlotIndex, enemyControl.navMeshAgent.gameObject);
        controller.navMeshAgent.speed = curSpeed;
        availableSlotIndex = -1;

        controller.navMeshAgent.destination = destPos;
        if (moving)
            controller.navMeshAgent.isStopped = false;  // Allow the agent to move
    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        // If the player has entered the ranged attack range, transition to the ranged state
        float dist = GetSquareDistance(destPos, npc.position);

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
        controller.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime);
        if (moving)
        {

            destPos = player.position;
            //move to destination
            controller.navMeshAgent.destination = destPos;
            speed = controller.navMeshAgent.velocity.magnitude;
            anim.SetFloat("Speed", speed);
            //Debug.Log("Chase");
        }


    }

}
