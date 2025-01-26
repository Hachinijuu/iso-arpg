using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : FSMState
{
    //const int HIDE_WAYPOINT_DIST = 2;
//
    //EnemyController npcControl;
    //bool moving;
    //int availableSlotIndex;
//
    //float healthTimer;
    //float healthTimeInterval = 1.0f;
    //int healthDeduction = 5;

    private bool canAttack = false;
    private bool canMove = false;

    EnemyController controller;
    Animator anim;

    float square_rangeDist;
    float square_meleeDist;
    float square_chaseDist;
    //Constructor
    public RangedAttackState(EnemyController npc)
    {
        stateID = FSMStateID.RangedAttack;
        controller = npc;
        anim = controller.Anim;

        square_rangeDist = EnemyController.RANGEATTACK_DIST * EnemyController.RANGEATTACK_DIST;
        square_meleeDist = EnemyController.MELEEATTACK_DIST * EnemyController.MELEEATTACK_DIST;
        square_chaseDist = EnemyController.CHASE_DIST * EnemyController.CHASE_DIST;
        //waypoints = wp;
        //stateID = FSMStateID.RangedAttacking;
        //curSpeed = 5.0f;
        ////curRotSpeed = 2.0f;
        //npcControl = npc;
        //moving = false;
        //healthTimer = 0.0f;
//
        //npcControl.navMeshAgent.speed = curSpeed;
        //availableSlotIndex = -1;

    }
    
    public override void EnterStateInit()
    {
        canAttack = true;   // Agent is able to attack
        canMove = false;    // Agent should not move

        // Stop the agent

        controller.navMeshAgent.isStopped = true;
        // Enter State
        //Releasse slot position
        //npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.navMeshAgent.gameObject);

        //availableSlotIndex = -1;
        //healthTimer = 0.0f;

    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        // LOGIC BREAKDOWN
        // If the enemy is in ranged attack range
        // Stay in this state, and attack the enemy
        // If the enemy comes too close and enters the melee threshold, enter the melee attack state
        // If the enemy is too far to attack, enter the chase state and chase after them

        // If the agent is in ranged attack range, continue

        float dist = GetSquareDistance(npc, destPos);
        if (dist > square_meleeDist && dist <= square_rangeDist)
            return;
        // If the enemy is too far transtion to the chase state
        if (dist >= square_chaseDist)
        {
            controller.PerformTransition(Transition.ChasePlayer);
        }
        // If the enemy has entered the melee threshold, enter the melee attack state
        else if (dist < square_meleeDist)
        {
            controller.PerformTransition(Transition.PlayerReached);
        }
        //else
        //{
        //    destPos = player.position;
        //    
        //    // Choose not to move
        //}

        //destPos = player.position;
        
//
        ////Releasse slot position
        ////npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.navMeshAgent.gameObject);
        //availableSlotIndex = -1;
        //// Reserve the Position
        ////availableSlotIndex = npcControl.playerSlotManager.ReserveSlotAroundObject(npcControl.gameObject);
        //if (availableSlotIndex != -1)
        //{
        //    //destPos = npcControl.playerSlotManager.GetSlotPosition(availableSlotIndex);
        //}
//
        ////if (npcControl.GetHealth() == 0)
        ////{
        ////    npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
        ////    npcControl.PerformTransition(Transition.NoHealth);
////
        ////    return;
        ////}
        //// ORDER DOES MATTER
        //if (IsInCurrentRange(npc, destPos, EnemyController.ATTACK_DIST))
        //{
        //    moving = false; //close to the player so stop moving
        //    if (npcControl.GetHealth() < 50)
        //    {
        //        // if our health is low perform low health transition
        //        //npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
        //        npcControl.PerformTransition(Transition.LowHealth);
//
//
        //    }
        //}
        //else if (IsInCurrentRange(npc, destPos, MonsterControllerAI.CHASE_DIST))
        //{
        //    if (npcControl.GetHealth() < 50)
        //    {
        //        // if our health is low perform low health transition
        //        npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
        //        npcControl.PerformTransition(Transition.LowHealth);
//
        //    }
        //    else
        //    {
        //        moving = true;
        //    }
        //}
        //else if (!IsInCurrentRange(npc, destPos, MonsterControllerAI.LOST_DIST))
        //{
        //    //Player is far away so perform Lost Player
        //    npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.gameObject);
        //    npcControl.PerformTransition(Transition.LostPlayer);
        //}

        

    }
    //Act
    public override void Act(Transform player, Transform npc)
    { 
        // Aim at the player
        if (canAttack)
        {
            // Tell the animator to play the attack animation
            Debug.Log("[FSM_RangedAttack]: Attacking the player");
        }


        //Rotate towards Position
        //Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        //Snap
        //npcControl.navMeshAgent.transform.rotation = targetRotation; 
        // Slower Rotation
        //npcControl.navMeshAgent.transform.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        //if (moving)
        //{
        //    //move to destination
        //    npcControl.navMeshAgent.destination = destPos;
        //}
        //else
        //{
        //    // Rest and increment Health
        //    if (npcControl.GetHealth() > 0)
        //    {
        //        //Heal while hiding
        //        healthTimer += Time.deltaTime;
        //        if (healthTimer > healthTimeInterval)
        //        {
        //            npcControl.DecHealth(healthDeduction);
        //            healthTimer = 0;
        //        }
        //    }
//
        //}
    }
}
