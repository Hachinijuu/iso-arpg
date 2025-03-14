using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRegen : AIState
{
    [Header("Regen Value")]
    public float regenAmount = 20f;
    public float regenRate = 1.0f;
    public override void Reason(AgentStateArgs e)
    {
        //throw new System.NotImplementedException();

        // From the regen state, transition into

        // Transition into the ranged attack state once in ranged attack range
        // The ranged attack state can handle the ranged -> melee transition

        // Once the regen has been complete, transition into idle
        EnemyControllerV2 agent = e.agent;
        Vector3 agentPos = agent.transform.position;
        Vector3 playerPos = e.player.transform.position;
        if (IsInCurrentRange(playerPos, agentPos, AIManager.RANGED_RANGE)) // Take priority for attacking over staying in regenerate
        {
            agent.SetState(StateId.RangedAttack);
            return;
        }
        // If the health has regened to the max value, transition out of this state
        else if (agent.stats.Health.Value >= agent.stats.Health.MaxValue)
        { 
            agent.SetState(StateId.Idle);
            return;
        }
    }

    public override void Act(AgentStateArgs e)
    {
        //throw new System.NotImplementedException();
        // In the act, while in the state, increase the agent's health

        // How do we handle the interval or rate to increase when the state is listed globally
        // There is no individual tracker, UNLESS we create a list for them 

    }
}
