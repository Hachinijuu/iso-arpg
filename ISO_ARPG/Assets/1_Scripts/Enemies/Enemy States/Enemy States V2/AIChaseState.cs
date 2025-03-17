using UnityEngine;

[CreateAssetMenu(fileName = "ChaseState", menuName = "sykcorSystems/AI/States/ChaseState", order = 1)]
public class AIChaseState : AIState//, IPhysicsState
{
    private static string ChaseAnimation = "Speed";
    private int animId = Animator.StringToHash(ChaseAnimation);
    // public override void EnterState(AgentStateArgs e)
    // {
    //     // When entering the chase state, assign self to the moving group
    //     if (e.agent == null) { return; }
    //     // Check if they are already part of the moving list
    //     if (AIManager.Instance.movingEnemies.Contains(e.agent)) { return; }
    //     // Otherwise, add them to the moving list
    //     AIManager.Instance.RegisterMoving(e.agent);
    // }
    // public override void ExitState(AgentStateArgs e)
    // {
    //     if (e.agent == null) { return; }
    //     if (!AIManager.Instance.movingEnemies.Contains(e.agent)) { return; }
    //     AIManager.Instance.UnregisterMoving(e.agent);
    // }
    public override void Reason(AgentStateArgs e)
    {
        // When in the chase state, how will the agent make descisions?
        // This is where the state transition will occur
        // If the agent is within the player's 

        // Player can be passed via args, or found in a lookup from playerManager..

        // If the player is in ranged attack range, transition
        // If the player is in melee attack range, transition

        // The state machine needs to know if those states exist in order to transition to them
        EnemyControllerV2 agent = e.agent;
        Vector3 agentPos = agent.transform.position;
        Vector3 playerPos = e.player.transform.position;
        // Check if the agent contains a ranged state, if they do, check for ranged distance
        if (agent.States.stateMap.ContainsKey(StateId.RangedAttack))
        {
            if (IsInCurrentRange(playerPos, agentPos, AIManager.RANGED_RANGE))  // If the player is in ranged attack range
            {
                agent.SetState(StateId.RangedAttack);   // Transition to the ranged attack state
                return; // Early return to prevent any further logic from executing in the reason, decision has been made to transition into ranged state
            }
        }
        else
        {
            // Agent does not contain a ranged attack state, check if they are in melee distance
            // Check if the agent is further than the chase range
            if (!IsInCurrentRange(playerPos, agentPos, AIManager.CHASE_RANGE))
            {
                agent.SetState(StateId.Idle);
                return;
            }
            else if (IsInCurrentRange(playerPos, agentPos, AIManager.MELEE_RANGE))
            {
                agent.SetState(StateId.MeleeAttack);
                return;
            }
        }
        // If no conditions have changed, the player will remain in the current state
    }
    public override void Act(AgentStateArgs e)
    {
        // When in the chase state, how wil the agent behave?
        // Don't really need to do anything since movement is handled on agent level.
        // Just handle animation here, and agent level can handle the movement context

        // This will call the movement functionality
        //if (e.cellTarget == null) { return; }
        EnemyControllerV2 agent = e.agent;
        //agent.MoveAgent(e.cellTarget);

        // BECAUSE THE RANGE OF THE CHASE IS SMALL, SIMPLY MOVE THE AGENT TO THE PLAYER
        // OTHER STATES WILL HANDLE THE SWITCH LOGIC
        Vector3 target = e.player.transform.position;
        agent.MoveAgent(target);
        agent.HandleRotation(target);

        //float speed = (agent.transform.position - target).normalized.magnitude;
        agent.Animator.SetFloat(animId, agent.CurrentSpeed);

        // Should movement be passed to this function, probably, considering target driven movement is different
    }

    // void IPhysicsState.FixedAct(AgentStateArgs e)
    // {
    //     if (e.cellTarget == null) { return; }
    //     EnemyControllerV2 agent = e.agent;
    //     agent.MoveAgent(e.cellTarget);
    // }

}