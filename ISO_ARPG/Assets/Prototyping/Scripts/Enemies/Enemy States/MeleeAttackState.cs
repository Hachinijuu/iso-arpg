using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : FSMState
{
    const int HIDE_WAYPOINT_DIST = 2;

    MonsterControllerAI npcControl;
    bool moving;
    int availableSlotIndex;

    float healthTimer;
    float healthTimeInterval = 1.0f;
    int healthDeduction = 5;


    //Constructor
    public MeleeAttackState(Transform[] wp, MonsterControllerAI npc)
    {
        waypoints = wp;
        stateID = FSMStateID.MeleeAttacking;
        curSpeed = 5.0f;
        curRotSpeed = 2.0f;
        npcControl = npc;
        moving = false;
        healthTimer = 0.0f;

        npcControl.navMeshAgent.speed = curSpeed;
        availableSlotIndex = -1;

    }
    public override void EnterStateInit()
    {
        // Enter State
        //Releasse slot position
        npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.navMeshAgent.gameObject);

        availableSlotIndex = -1;
        healthTimer = 0.0f;

    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        destPos = player.position;

        //Releasse slot position
        npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.navMeshAgent.gameObject);
        availableSlotIndex = -1;
        // Reserve the Position
        availableSlotIndex = npcControl.playerSlotManager.ReserveSlotAroundObject(npcControl.gameObject);
        if (availableSlotIndex != -1)
        {
            destPos = npcControl.playerSlotManager.GetSlotPosition(availableSlotIndex);
        }

        if (npcControl.GetHealth() == 0)
        {
            npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
            npcControl.PerformTransition(Transition.NoHealth);

            return;
        }
        // ORDER DOES MATTER
        if (IsInCurrentRange(npc, destPos, MonsterControllerAI.ATTACK_DIST))
        {
            moving = false; //close to the player so stop moving
            if (npcControl.GetHealth() < 50)
            {
                // if our health is low perform low health transition
                npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
                npcControl.PerformTransition(Transition.LowHealth);


            }
        }
        else if (IsInCurrentRange(npc, destPos, MonsterControllerAI.CHASE_DIST))
        {
            if (npcControl.GetHealth() < 50)
            {
                // if our health is low perform low health transition
                npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
                npcControl.PerformTransition(Transition.LowHealth);

            }
            else
            {
                moving = true;
            }
        }
        else if (!IsInCurrentRange(npc, destPos, MonsterControllerAI.LOST_DIST))
        {
            //Player is far away so perform Lost Player
            npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
            npcControl.PerformTransition(Transition.LostPlayer);
        }

    }
    //Act
    public override void Act(Transform player, Transform npc)
    { //Rotate towards Position
        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //Snap
        //npcControl.navMeshAgent.transform.rotation = targetRotation; 
        // Slower Rotation
        npcControl.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        if (moving)
        {
            //move to destination
            npcControl.navMeshAgent.destination = destPos;
        }
        else
        {
            // Rest and increment Health
            if (npcControl.GetHealth() > 0)
            {
                //Heal while hiding
                healthTimer += Time.deltaTime;
                if (healthTimer > healthTimeInterval)
                {
                    npcControl.DecHealth(healthDeduction);
                    healthTimer = 0;
                }
            }

        }
    }
}
