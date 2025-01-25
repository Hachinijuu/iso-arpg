using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenerateState : FSMState
{
    const int HIDE_WAYPOINT_DIST = 2;
    const int HIDE_WAYPOINT_INDEX = 5;
    MonsterControllerAI npcControl;
    bool moving;
    int availableSlotIndex;

    float healthTimer;
    float healthTimeInterval = 1.0f;
    int healthAdd = 10;


    //Constructor
    public RegenerateState(Transform[] wp, MonsterControllerAI npc)
    {
        waypoints = wp;
        stateID = FSMStateID.Regenerating;
        curSpeed = 3.0f;
        curRotSpeed = 2.0f;
        npcControl = npc;
        moving = true;
        healthTimer = 0.0f;

        npcControl.navMeshAgent.speed = curSpeed;

        //destPos = waypoints[HIDE_WAYPOINT_INDEX].position;
        availableSlotIndex = -1;
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
        npcControl.hideSlotManager.ReleaseSlot(availableSlotIndex, npcControl.navMeshAgent.gameObject);
        // Reserve the Position
        availableSlotIndex = npcControl.hideSlotManager.ReserveSlotAroundObject(npcControl.gameObject);
        if (availableSlotIndex != -1)
        {
            destPos = npcControl.hideSlotManager.GetSlotPosition(availableSlotIndex);
        }
        healthTimer = 0.0f;

    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        moving = true;
        if (npcControl.GetHealth() == 0)
        {
            npcControl.hideSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
            npcControl.PerformTransition(Transition.NoHealth);

            return;
        }
        // ORDER DOES MATTER
        if (IsInCurrentRange(npc, player.position, MonsterControllerAI.CHASE_DIST))
        {
            // See the player...
            npcControl.hideSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
            npcControl.PerformTransition(Transition.SawPlayer);
        }
        else if (IsInCurrentRange(npc, destPos, HIDE_WAYPOINT_DIST))
        {
            moving = false;
        }

    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        if (moving)
        {
            //Rotate towards Position
            Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
            //Snap
            //npcControl.navMeshAgent.transform.rotation = targetRotation; 
            // Slower Rotation
            npcControl.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);


            //move to destination
            npcControl.navMeshAgent.destination = destPos;
        }
        else
        {
            // Rest and increment Health
            if (npcControl.GetHealth() < 100)
            {
                //Heal while hiding
                healthTimer += Time.deltaTime;
                if (healthTimer > healthTimeInterval)
                {
                    npcControl.AddHealth(healthAdd);
                    healthTimer = 0;
                }
            }

        }
    }
}
