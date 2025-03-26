using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "sykcorSystems/AI/States/MeleeAttack", order = 2)]
public class AIMeleeAttack : AIState//, IPhysicsState//, ICoroutineState
{
    [Header("Attack Parameters")]
    public float hitboxUptime = 0.5f;
    public float attackRange = 1.0f;
    public float attackCooldown = 5.0f;
    //float ICoroutineState.intervalTime { get { return attackCooldown; } set { attackCooldown = value; } }

    [Header("Animations")]
    private static string AttackTrigger = "Attack";
    private int animId = Animator.StringToHash(AttackTrigger);
    // public override void EnterState(AgentStateArgs e)
    // {
    //     EnemyControllerV2 agent = e.agent;
    //     if (agent == null) { return; }
    //     PlayerSlotSystem slotSystem = e.player.Slots;
    //     if (slotSystem != null)
    //     {
    //         if (!slotSystem.CheckHasSlot(agent))    // If the agent does not have a slot
    //         {
    //             slotSystem.ReserveSlot(agent);      // Reserve a slot
    //         }
    //     }
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
                Debug.Log("Unreserved a slot");
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

    float distance;
    public override void Act(AgentStateArgs e)
    {
        // Move to the target
        EnemyControllerV2 agent = e.agent;
        PlayerSlotSystem slotSystem = e.player.Slots;

        Vector3 playerPos = e.player.transform.position;
        distance = Vector3.Distance(agent.transform.position, playerPos);
        // If the agent is close enough to attack, perform the attack and then wait for the interval time
        if (distance < attackRange && agent.canAttack)
        {
            agent.Attack();
            // Unreserve the slot
            if (slotSystem != null && slotSystem.CheckHasSlot(agent))
            {
                slotSystem.UnreserveSlot(agent);
            }
        }

        if (!agent.canAttack)
        {
            // Move the agent out of the attack range (move them to the slot)
            if (slotSystem.CheckHasSlot(agent))     // If the agent has a slot
            {
                // Move to the slot
                Transform slot = slotSystem.GetSlot(agent);
                float slotDistance = Vector3.Distance(slot.position, agent.transform.position);
                if (slotDistance > 0.25f)
                {
                    agent.canMove = true;
                    agent.MoveAgentNoAvoidance(slot.position);
                    agent.HandleRotation(playerPos);
                }
            }
            else
            {
                // The agent does not have a slot, reserve a slot for usage
                slotSystem.ReserveSlot(agent);
                // Possibly play animation or wander around because there is no attack position (not actively attacking)
            }
        }
        else
        {
            // Move the agent into attack range
            agent.canMove = true;
            agent.MoveAgent(playerPos);
            agent.HandleRotation(playerPos);
        }
        // if (distance <= attackRange && agent.canAttack)
        // {
        //     agent.Attack(); // Perform the attack
        // }

        // When performing an attack, stop moving

        // Logic for melee attacks, grab a slot (position around the player)
        // Move forward on the attack, then revert back to the slot position
        // Problem with perfect slot movement is that when the player moves, the agents will move with them.
    }
    // void IPhysicsState.FixedAct(AgentStateArgs e)
    // {
    //     // In the fixed act state, this is where the agent movement will be handled.

    //     // If the agent does not have a slot to the enemy. Do nothing
    //     EnemyControllerV2 agent = e.agent;
    //     PlayerController player = e.player;
    //     PlayerSlotSystem slotSystem = e.player.Slots;
    //     if (slotSystem == null && !slotSystem.CheckHasSlot(agent)) { return; }  // If the slot system does not exist, or the agent does not have a slot, early return
    //     // The slot system DOES exist, and the agent DOES have a slot
    //     // They are "allowed" to perform attacks

    //     Vector3 playerPos = e.player.transform.position;

    //     // If the agent cannot attack
    //     if (!agent.canAttack)
    //     {
    //         // Move the agent out of the attack range (move them to the slot)
    //         if (slotSystem.CheckHasSlot(agent))     // If the agent has a slot
    //         {
    //             // Move to the slot
    //             Transform slot = slotSystem.GetSlot(agent);
    //             float slotDistance = Vector3.Distance(slot.position, agent.transform.position);
    //             if (slotDistance > 0.25f)
    //             {
    //                 agent.canMove = true;
    //                 agent.MoveAgentNoAvoidance(slot.position);
    //                 agent.HandleRotation(playerPos);
    //             }
    //         }
    //         else
    //         {
    //             // The agent does not have a slot, reserve a slot for usage
    //             slotSystem.ReserveSlot(agent);
    //             // Possibly play animation or wander around because there is no attack position (not actively attacking)
    //         }
    //     }
    //     else
    //     {
    //         // Move the agent into attack range
    //         agent.canMove = true;
    //         agent.MoveAgent(playerPos);
    //         agent.HandleRotation(playerPos);
    //     }
    // }
}
