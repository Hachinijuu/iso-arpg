using UnityEngine;

public class RegenerateState : FSMState
{
    //const int HIDE_WAYPOINT_DIST = 2;
    //const int HIDE_WAYPOINT_INDEX = 5;
    //MonsterControllerAI npcControl;
    //bool moving;
    //int availableSlotIndex;
    //
    float healthTimer;
    float healthTimeInterval = 1.0f;
    int healthAdd = 10;

    EnemyController controller;


    //Constructor
    public RegenerateState(EnemyController npc)
    {
        npc = controller;
        //waypoints = wp;
        //stateID = FSMStateID.Regenerating;
        //curSpeed = 3.0f;
        //curRotSpeed = 2.0f;
        //npcControl = npc;
        //moving = true;
        //healthTimer = 0.0f;
        //
        //npcControl.navMeshAgent.speed = curSpeed;
        //
        ////destPos = waypoints[HIDE_WAYPOINT_INDEX].position;
        //availableSlotIndex = -1;
        // Reserve the position
        // availableSlotIndex = npcControl.hideSlotManager.ReserveSlotAroundObject(npcControl.gameObject);
        // if (availableSlotIndex != -1)
        // {
        //     destPos = npcControl.hideSlotManager.GetSlotPosition(availableSlotIndex);
        // }
    }
    public override void EnterStateInit()
    {
        // Enter State
        //Releasse slot position
        //npcControl.hideSlotManager.ReleaseSlot(availableSlotIndex, npcControl.navMeshAgent.gameObject);
        //// Reserve the Position
        //availableSlotIndex = npcControl.hideSlotManager.ReserveSlotAroundObject(npcControl.gameObject);
        //if (availableSlotIndex != -1)
        //{
        //    destPos = npcControl.hideSlotManager.GetSlotPosition(availableSlotIndex);
        //}
        healthTimer = 0.0f;

    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        // Conditions to transition out of regenerate state
        // - Has been attacked
        // - Enemy entered range
        // - Attack state conditions
        // If the player has entered the ranged attack range, transition to the ranged state
        //float dist = GetSquareDistance(npc.position, destPos);
        // Ranged attack range is greater than melee, less than or equal to ranged

        // THIS STATE IS ONLY ADDED TO THE ELITE ENEMIES
        // WHEN THE ELITE ENEMIES ARE ADDED, THE TRANSITIONS NEED TO EXIST BASED ON THE CONTROLLER
        if (controller)
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
        //if (dist > EnemyController.MELEEATTACK_DIST && dist <= EnemyController.RANGEATTACK_DIST)
        //{
        //    controller.PerformTransition(Transition.ReachPlayer);
        //}
        //// If the player has entered the melee attack range, transition to the melee attack state
        //else if (dist <= EnemyController.MELEEATTACK_DIST)
        //{
        //    controller.PerformTransition(Transition.PlayerReached);
        //}
        //else
        //{
        //    destPos = player.position;
        //}
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        //if (moving)
        //{
        //    //Rotate towards Position
        //    Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //    //Snap
        //    //npcControl.navMeshAgent.transform.rotation = targetRotation; 
        //    // Slower Rotation
        //    npcControl.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        //
        //
        //    //move to destination
        //    npcControl.navMeshAgent.destination = destPos;
        //}
        //else
        //{
        //    // Rest and increment Health
        //    if (npcControl.GetHealth() < 100)
        //    {
        //        //Heal while hiding
        //        healthTimer += Time.deltaTime;
        //        if (healthTimer > healthTimeInterval)
        //        {
        //            npcControl.AddHealth(healthAdd);
        //            healthTimer = 0;
        //        }
        //    }
        //
        //}

        if (controller.stats.Health.Value < controller.stats.Health.MaxValue)
        {
            //Heal while hiding
            healthTimer += Time.deltaTime;
            if (healthTimer > healthTimeInterval)
            {
                controller.stats.Health.Value += healthAdd;
                healthTimer = 0;
            }
        }
    }
}
