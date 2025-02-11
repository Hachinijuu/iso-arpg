using UnityEngine;

public class RangedAttackState : FSMState
{
    #region VARIABLES
    float attackTimer;
    float attackInterval = 1.0f;

    private bool canAttack = false;
    EnemyController controller;
    Animator anim;
    #endregion

    //Constructor
    public RangedAttackState(EnemyController npc)
    {
        stateID = FSMStateID.RangedAttack;
        controller = npc;
        anim = controller.Anim;
    }
    #region FUNCTIONALITY
    public override void EnterStateInit()
    {
        canAttack = true;   // Agent is able to attack

        // Stop the agent
        controller.navMeshAgent.isStopped = true;

        anim.SetFloat(EnemyController.moveAnimID, 0.0f);    // visually show that the agent has stopped by stopping the animator
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
            if (attackTimer > attackInterval)
            {
                anim.SetTrigger(EnemyController.attackAnimID);
                attackTimer = 0.0f;
            }
        }
    }
    #endregion
}
