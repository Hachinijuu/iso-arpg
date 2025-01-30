using UnityEngine;

public class MeleeAttackState : FSMState
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

    float attackTimer;
    float attackInterval = 1.0f;

    private bool canAttack = false;
    private bool canMove = false;
    EnemyController controller;
    Animator anim;

    float square_rangeDist;
    float square_meleeDist;
    float square_chaseDist;
    //Constructor
    public MeleeAttackState(EnemyController npc)
    {
        stateID = FSMStateID.MeleeAttack;
        controller = npc;
        anim = controller.Anim;

        square_rangeDist = EnemyController.RANGEATTACK_DIST * EnemyController.RANGEATTACK_DIST;
        square_meleeDist = EnemyController.MELEEATTACK_DIST * EnemyController.MELEEATTACK_DIST;
        square_chaseDist = EnemyController.CHASE_DIST * EnemyController.CHASE_DIST;
        //waypoints = wp;
        //stateID = FSMStateID.MeleeAttacking;
        //curSpeed = 5.0f;
        //curRotSpeed = 2.0f;
        //npcControl = npc;
        //moving = false;
        //healthTimer = 0.0f;
        //
        //npcControl.navMeshAgent.speed = curSpeed;
        //availableSlotIndex = -1;

    }
    public override void EnterStateInit()
    {
        //Debug.Log("[FSM_Melee]: Entered state");
        controller.navMeshAgent.isStopped = false;
        // Enter State
        //Releasse slot position
        //npcControl.playerSlotManager.ReleaseSlot(availableSlotIndex, npcControl.navMeshAgent.gameObject);
        //
        //availableSlotIndex = -1;
        //healthTimer = 0.0f;

    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        // If the enemy exits melee range, transition to the chase state
        if (!(IsInCurrentRange(player, npc.position, EnemyController.MELEEATTACK_DIST)))
        {
            //Debug.Log("[FSM_MeleeState]: Transitioned to Chase");
            controller.PerformTransition(Transition.ChasePlayer);
            return;
        }

        // Enemy is in the attack range, perform attacks
        destPos = player.position;      
        // Make sure can attack is set
        if (canAttack != true)
            canAttack = true;
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        // Move towards the player if not close enough
        controller.navMeshAgent.destination = destPos;

        float speed = controller.navMeshAgent.velocity.magnitude;
        if (speed > 0)
        {
            anim.SetFloat("Speed", speed);
        }

        //Rotate towards Position
        if (canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer > attackInterval)
            {
                anim.SetTrigger("Ability1");
                controller.DamageSource.AllowDamageForTime(0.5f);
                attackTimer = 0.0f;
                return;
                // GIVE ATTACK REFERENCE TO THE HURTBOX, AND TELL HITBOX HOW LONG TO BE ACTIVE FOR
                // PREVENT MULTI-FRAME COLLISIONS SO PLAYER DOES NOT GET ONESHOT
            }
            // Tell the animator to play the attack animation
            //Debug.Log("[FSM_MeleeAttack]: Attacking the player");
        }
        //Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        ////Snap
        ////npcControl.navMeshAgent.transform.rotation = targetRotation; 
        //// Slower Rotation
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
