using UnityEngine;

public class RegenerateState : FSMState
{
    #region VARIABLE
    float healthTimer;
    float healthTimeInterval = 1.0f;
    int healthAdd = 10;

    EnemyController controller;
    #endregion

    //Constructor
    public RegenerateState(EnemyController npc)
    {
        npc = controller;
    }
    public override void EnterStateInit()
    {
        healthTimer = 0.0f;

    }

    #region FUNCTIONALITY
    //Reason
    public override void Reason(Transform player, Transform npc)
    {
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
    }
    //Act
    public override void Act(Transform player, Transform npc)
    {
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
    #endregion
}
