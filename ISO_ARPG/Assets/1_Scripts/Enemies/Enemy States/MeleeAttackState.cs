using UnityEngine;

public class MeleeAttackState : FSMState
{
    #region VARIABLES
    float attackTimer;
    float attackInterval = 1.0f;

    private bool canAttack = false;
    EnemyController controller;
    Animator anim;

    // private int moveAnimID = Animator.StringToHash("Speed");
    // private int attackAnimID = Animator.StringToHash("Ability1"); // This will be changed with custom controller
    #endregion
    //Constructor
    public MeleeAttackState(EnemyController npc)
    {
        stateID = FSMStateID.MeleeAttack;
        controller = npc;
        anim = controller.Anim;
    }
    #region FUNCTIONALITY
    public override void EnterStateInit()
    {
        controller.navMeshAgent.isStopped = false;
    }


    //Reason
    public override void Reason(Transform player, Transform npc)
    {
        // If the enemy exits melee range, transition to the chase state
        if (!(IsInCurrentRange(player, npc.position, EnemyController.MELEEATTACK_DIST)))
        {
            controller.PerformTransition(Transition.ChasePlayer);
            return;
        }
        else
        {
            // Enemy is in the attack range, perform attacks
            destPos = player.position;      
            // Make sure can attack is set
            if (canAttack != true)
                canAttack = true;
        }
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
        // Move towards the player if not close enough
        controller.navMeshAgent.destination = destPos;

        float speed = controller.navMeshAgent.velocity.magnitude;
        if (speed > 0)
        {
            anim.SetFloat(EnemyController.moveAnimID, speed);
        }

        //Rotate towards Position
        // Check possibility of a coroutine here as the timestep instead of counter
        if (canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer > attackInterval)
            {
                anim.SetTrigger(EnemyController.attackAnimID);
                controller.DamageSource.AllowDamageForTime(0.5f);
                attackTimer = 0.0f;
                return;
                // GIVE ATTACK REFERENCE TO THE HURTBOX, AND TELL HITBOX HOW LONG TO BE ACTIVE FOR
                // PREVENT MULTI-FRAME COLLISIONS SO PLAYER DOES NOT GET ONESHOT
            }
        }
    }
    #endregion
}
