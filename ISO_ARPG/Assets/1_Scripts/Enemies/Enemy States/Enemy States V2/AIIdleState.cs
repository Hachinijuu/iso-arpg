using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "sykcorSystems/AI/States/Idle", order = 0)]
public class AIIdleState : AIState
{
    // In the idle state, do not do much, mayble play animations, mostly decision making to check if the player is in range

    // Since this is a scriptable object, the range to transition can be defined, or just simply listed in the AI Manager.
    // Having the definition here might be better
    [SerializeField] float maxRoam = 0.5f;
    [SerializeField] float minRoam = 0.25f;

    public override void Reason(AgentStateArgs e)
    {
        Vector3 agentPos = e.agent.transform.position;
        Vector3 playerPos = e.player.transform.position;

        if (e.agent is EliteController)
        {
            if (IsInCurrentRange(playerPos, agentPos, AIManager.SPECIAL_RANGE))
            { 
                e.agent.SetState(StateId.SpecialAttack);
                return;
            }
        }

        // If I am in the idle state and the alarm bell rings, transition to the Alarmed State
        if (IsInCurrentRange(playerPos, agentPos, AIManager.CHASE_RANGE))  // If the player is in chase range
        {
            e.agent.SetState(StateId.Chase);   // Transition to the chase state
            return; // Early return to prevent any further logic from executing in the reason, decision has been made to transition into cjase state
        }

    }

    public override void Act(AgentStateArgs e)
    {
        return;
        // //Debug.Log("Idling");
        // if (e.agent != null)
        // {
        //     e.agent.AnimateAgentMove();
        // }

    }
}
