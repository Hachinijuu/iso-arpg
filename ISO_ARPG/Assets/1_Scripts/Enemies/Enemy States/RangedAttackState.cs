using UnityEngine;

public class RangedAttackState : FSMState
{
    #region VARIABLES
    float attackTimer;
    float attackInterval = 1.0f;

    private bool canAttack = false;
    private bool canMove = false;

    EnemyController controller;
    Animator anim;

    float square_rangeDist;
    float square_meleeDist;
    float square_chaseDist;
    #endregion

    //Constructor
    public RangedAttackState(EnemyController npc)
    {
        stateID = FSMStateID.RangedAttack;
        controller = npc;
        anim = controller.Anim;

        square_rangeDist = EnemyController.RANGEATTACK_DIST * EnemyController.RANGEATTACK_DIST;
        square_meleeDist = EnemyController.MELEEATTACK_DIST * EnemyController.MELEEATTACK_DIST;
        square_chaseDist = EnemyController.CHASE_DIST * EnemyController.CHASE_DIST;
    }
    #region FUNCTIONALITY
    public override void EnterStateInit()
    {
        canAttack = true;   // Agent is able to attack
        canMove = false;    // Agent should not move

        // Stop the agent
        controller.navMeshAgent.isStopped = true;
    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        // If the enemy has entered the melee threshold, enter the melee attack state
        if (IsInCurrentRange(player, npc.position, EnemyController.MELEEATTACK_DIST))
        {
            //Debug.Log("[FSM_Ranged]: Transitioned to Melee");
            controller.PerformTransition(Transition.ReachPlayer);
            return;
        }
        // If the target is not in ranged attack distance
        else if (!(IsInCurrentRange(player, npc.position, EnemyController.RANGEATTACK_DIST)))
        {
            //Debug.Log("[FSM_Ranged]: Transitioned to Chase");
            controller.PerformTransition(Transition.ChasePlayer);
            return;
        }
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        // Aim at the player
        if (canAttack)
        {
            attackTimer += Time.deltaTime;
            anim.SetFloat("Speed", 0.0f);
            if (attackTimer > attackInterval)
            {
                anim.SetTrigger("Ability1");
                attackTimer = 0.0f;
            }
        }
    }
    #endregion
}
