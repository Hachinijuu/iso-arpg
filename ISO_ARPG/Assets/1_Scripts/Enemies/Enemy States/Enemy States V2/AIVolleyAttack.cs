using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVolleyAttack : AIState
{
    public override void Reason(AgentStateArgs e)
    {
        EnemyControllerV2 agent = e.agent;

        // This state needs to transition to regular ranged if target is close enough
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
