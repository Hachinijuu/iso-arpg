using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRangedAttack : AIState
{
    //[Header("Attack Parameters")]
    //public float hitboxUptime = 0.5f;

    [Header("Animations")]
    public static string AttackTrigger = "Attack";
    public int animId = Animator.StringToHash(AttackTrigger);
    public override void Reason(AgentStateArgs e)
    {
        // Does any movement get driven when in this state, or is it simply shoot and transition out given the conditions

        // If the enemy has entered the melee threshold, enter the melee attack state

        EnemyControllerV2 agent = e.agent;
        Vector3 agentPos = e.agent.transform.position;
        Vector3 playerPos = e.player.transform.position;
        if (IsInCurrentRange(playerPos, agentPos, AIManager.MELEE_RANGE))
        {
            agent.SetState(StateId.MeleeAttack);
            return;
        }
        // If the target is not in ranged attack distance (further range)
        else if (!(IsInCurrentRange(playerPos, agentPos, AIManager.RANGED_RANGE)))
        {
            agent.SetState(StateId.Chase);
            return;
        }

        //throw new System.NotImplementedException();
    }

    public override void Act(AgentStateArgs e)
    {
        //throw new System.NotImplementedException();
        EnemyControllerV2 agent = e.agent;
        agent.Animator.SetTrigger(animId);
        // Fire a projectile from the AI's projectile source
        // But if projectiles are bound to the 
    }
}
