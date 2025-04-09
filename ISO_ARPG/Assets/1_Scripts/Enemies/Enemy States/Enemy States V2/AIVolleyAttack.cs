using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VolleyAttack", menuName = "sykcorSystems/AI/States/VolleyAttack", order = 3)]
public class AIVolleyAttack : AIState
{
    public override void Reason(AgentStateArgs e)
    {
        EnemyControllerV2 agent = e.agent;

        // This state needs to transition to regular ranged if target is close enough

        Vector3 agentPos = e.agent.transform.position;
        Vector3 playerPos = e.player.transform.position;

        // Only check for melee transition if the agent is an elite
        if (agent.stats.id == EntityID.ELITE || agent.stats.id == EntityID.BIG_ELITE)
        {
            if (IsInCurrentRange(playerPos, agentPos, AIManager.MELEE_RANGE))
            {
                agent.SetState(StateId.MeleeAttack);
                return;
            }
        }
        else
        {
            // If the target is not in ranged attack distance (further range)
            if (!(IsInCurrentRange(playerPos, agentPos, AIManager.RANGED_RANGE)))
            {
                agent.SetState(StateId.Chase);
                return;
            }
        }




    }

    public override void Act(AgentStateArgs e)
    {
        EliteController agent = e.agent as EliteController;
        agent.HandleRotation(e.player.transform.position);


        // This is an elite controller, so we need to call the elite attack

        if (agent.canAttack) { agent.SpecialAttack(e.player.transform); }
        // This is all the logic that is required for this state
    }
}
