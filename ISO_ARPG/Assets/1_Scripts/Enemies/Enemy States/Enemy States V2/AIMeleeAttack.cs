using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "sykcorSystems/AI/States/MeleeAttack", order = 2)]
public class AIMeleeAttack : AIState//, IPhysicsState
{
    [Header("Attack Parameters")]
    public float hitboxUptime = 0.5f;

    [Header("Animations")]
    private static string AttackTrigger = "Attack";
    private int animId = Animator.StringToHash(AttackTrigger);
    // public override void EnterState(AgentStateArgs e)
    // {
    //     if (e.agent == null) { return; }
    //     // Check if they are already part of the moving list
    //     if (AIManager.Instance.movingEnemies.Contains(e.agent)) { return; }
    //     // Otherwise, add them to the moving list
    //     AIManager.Instance.RegisterMoving(e.agent);

    //     // For melee attacks, request a spot around the player
    //     // If the player has all slots occupied, agent will stand still
    //     // Logic is handled in act
    // }

    public override void ExitState(AgentStateArgs e)
    {
        EnemyControllerV2 agent = e.agent;
        if (agent == null) { return; }
        agent.canMove = true;
        
        PlayerSlotSystem slotSystem = e.player.Slots;
        if (slotSystem != null)
        {
            if (slotSystem.CheckHasSlot(agent))
            {
                slotSystem.UnreserveSlot(agent);
            }
        }
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
            agent.SetState(StateId.Chase, e);
            return;
        }
    }

    public override void Act(AgentStateArgs e)
    {
        // Move to the target
        EnemyControllerV2 agent = e.agent;

        PlayerSlotSystem slotSystem = e.player.Slots;
        if (slotSystem == null) { return; }
        if (slotSystem.CheckHasSlot(agent))  // if the agent already has a slot
        {
            // Move to the slot
            Transform slot = slotSystem.GetSlot(agent);
            float slotDistance = Vector3.Distance(slot.position, agent.transform.position);
            //Debug.Log(slotDistance);
            if (slotDistance > 2f)
            {
                agent.canMove = true;
                agent.MoveAgent(slot);
                agent.HandleRotation(e.player.transform.position);
            }
            else
            {
                // Agent is within the threshold distance, perform the attack
                // If the agent is close enough to the player, perform the attack
                // Once the attack has been made, transition out of the state or stay in this state?
                // Otherwise just attempt to perfom attacks

                // When attacking, stop movement
                agent.canMove = false;
                agent.HandleRotation(e.player.transform.position);
                agent.Animator.SetTrigger(animId);
                agent.Hitbox.AllowDamageForTime(hitboxUptime);
            }
        }
        else
        {
            // Try getting a slot, if it fails, do nothing
            slotSystem.ReserveSlot(agent);
        }
    }

    // void IPhysicsState.FixedAct(AgentStateArgs e)
    // {
    //     PlayerSlotSystem slotSystem = e.player.Slots;
    //     EnemyControllerV2 agent = e.agent;
    //     agent.HandleRotation(e.player.transform.position);
    //     if (slotSystem == null) { return; }
    //     if (slotSystem.CheckHasSlot(agent)) // If the agent has a slot
    //     {
    //         // The agent will move to the slot, and attack if allowed
    //         // If the distance between the agent and the slot is greater than a threshold of nearness
    //         Transform slot = slotSystem.GetSlot(agent);
    //         float slotDistance = Vector3.Distance(slot.position, agent.transform.position);
    //         if (slotDistance > 0.25f)
    //         {
    //             // Move to the slot
    //             agent.MoveAgent(slot);
    //         }
    //     }
    //     // If the agent does not have a slot, they will do nothing, stand still
    // }
}
