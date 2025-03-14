using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMeleeAttack : AIState
{
    [Header("Attack Parameters")]
    public float hitboxUptime = 0.5f;

    [Header("Animations")]
    public static string AttackTrigger = "Attack";
    public int animId = Animator.StringToHash(AttackTrigger);
    public override void EnterState(AgentStateArgs e)
    {
        if (e.agent == null) { return; }
        // Check if they are already part of the moving list
        if (AIManager.Instance.movingEnemies.Contains(e.agent)) { return; }
        // Otherwise, add them to the moving list
        AIManager.Instance.RegisterMoving(e.agent);

        // For melee attacks, request a spot around the player
        // If the player has all slots occupied, agent will stand still
        // Logic is handled in act
    }

    public override void ExitState(AgentStateArgs e)
    {
        if (e.agent == null) { return; }
        if (!AIManager.Instance.movingEnemies.Contains(e.agent)) { return; }
        AIManager.Instance.UnregisterMoving(e.agent);
    }

    public override void Reason(AgentStateArgs e)
    {
        //throw new System.NotImplementedException();
        // For the melee attack, decide the transitions here

        // Get a slot reference to the player to move to, if the slot does not exist
        // If all slots are occupied, do not bother moving into the slot position

        // When dying, cleanup the slot positioning

        // If the enemy exits melee range, transition out of this state to the chase state

        EnemyControllerV2 agent = e.agent;
        Vector3 agentPos = agent.transform.position;
        Vector3 playerPos = e.player.transform.position;
        if (!(IsInCurrentRange(playerPos, agentPos, AIManager.MELEE_RANGE)))    // If the player is NOT in melee range, enter chase
        { 
            agent.SetState(StateId.MeleeAttack);
            return;
        }
    }

    public override void Act(AgentStateArgs e)
    {
        EnemyControllerV2 agent = e.agent;
        // If the agent is close enough to the player, perform the attack
        // Once the attack has been made, transition out of the state or stay in this state?
        agent.Animator.SetTrigger(animId);
        agent.Hitbox.AllowDamageForTime(hitboxUptime);

        //throw new System.NotImplementedException();
    }
}
