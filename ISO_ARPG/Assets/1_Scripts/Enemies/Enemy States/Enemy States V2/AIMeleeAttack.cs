using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "sykcorSystems/AI/States/MeleeAttack", order = 2)]
public class AIMeleeAttack : AIState, IPhysicsState//, ICoroutineState
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

        Vector3 playerPos = e.player.transform.position;
        distance = Vector3.Distance(agent.transform.position, playerPos);
        // If the agent is close enough to attack, perform the attack and then wait for the interval time
        if (distance <= attackRange && agent.canAttack)
        {
            agent.Attack(attackCooldown);   // This will disable attacks, and re-enable attacks after the time has passed
            agent.canMove = false;
            agent.Animator.SetTrigger(animId);
            agent.Hitbox.AllowDamageForTime(hitboxUptime);
        }

        // When performing an attack, stop moving

        // Logic for melee attacks, grab a slot (position around the player)
        // Move forward on the attack, then revert back to the slot position
        // Problem with perfect slot movement is that when the player moves, the agents will move with them.
    }
    void IPhysicsState.FixedAct(AgentStateArgs e)
    {
        // In the fixed act state, this is where the agent movement will be handled.

        // If the agent does not have a slot to the enemy. Do nothing
        EnemyControllerV2 agent = e.agent;
        PlayerController player = e.player;
        PlayerSlotSystem slotSystem = e.player.Slots;
        if (slotSystem == null && !slotSystem.CheckHasSlot(agent)) { return; }  // If the slot system does not exist, or the agent does not have a slot, early return
        // The slot system DOES exist, and the agent DOES have a slot
        // They are "allowed" to perform attacks

        Vector3 playerPos = e.player.transform.position;

        // If the agent cannot perform the attack, because it is on cooldown (EnemyController coroutine)
        if (player.Movement.Speed <= 0) // If the player is not moving, do the fancy circling
        {
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
        }
        // Agent is moving, chase after
        else
        {
            // Move the agent into attack range
            agent.canMove = true;
            agent.MoveAgent(playerPos);
            agent.HandleRotation(playerPos);
        }

        // If the attack is on cooldown (because of act running)
        // if (agent.ActRunning)
        // {
            
        // }
        // // Attack is NOT on cooldown
        // else if (!agent.ActRunning || !agent.canAttack)  // Or you cannot attack
        // {
        //     if (distance <= attackRange)    // If the agent is in attacking range, set the flag
        //     {
        //         agent.canAttack = true;
        //     }
        //     else
        //     {
        //         // The agent is not in attack range, move them INTO attack range
        //         agent.canMove = true;
        //         agent.MoveAgent(playerPos);
        //         agent.HandleRotation(playerPos);
        //     }
        // }

        // if (!agent.canAttack)   // If the agent cannot attack
        // {

        // }
        // // The agent can attack, move them forward into the attack distance
        // else
        // {
        //     if (distance <= attackRange)    // If the agent is in attacking range, set the flag
        //     {
        //         agent.canAttack = true;
        //     }
        //     else
        //     {
        //         // The agent is not in attack range, move them INTO attack range
        //         agent.canMove = true;
        //         agent.MoveAgent(playerPos);
        //         agent.HandleRotation(playerPos);
        //     }
        // }


        // If the player is too far to attack, but they can attack
        // if (distance > attackRange)
        // {
        //     // Move towards the player and allow attacks
        //     agent.canMove = true;
        //     agent.MoveAgent(playerPos);
        //     agent.HandleRotation(playerPos);
        // }
        

        // // The agent can perform an attack, move to the target
        // if (agent.canAttack)
        // {

        // }
        // // The agent cannot perform an attack, navigate to the slot
        // else
        // {
        //     if (slotSystem.CheckHasSlot(agent))     // If the agent has a slot
        //     {
        //         // Move to the slot
        //         Transform slot = slotSystem.GetSlot(agent);
        //         float slotDistance = Vector3.Distance(slot.position, agent.transform.position);
        //         if (slotDistance > 0.25f)
        //         {
        //             agent.canMove = true;
        //             agent.MoveAgent(slot);
        //             agent.HandleRotation(playerPos);
        //         }
        //     }
        //     else
        //     {
        //         // Try reserving one
        //         slotSystem.ReserveSlot(agent);
        //     }
        // }
    }

    //     // Move to the target
    //     EnemyControllerV2 agent = e.agent;

    //     PlayerSlotSystem slotSystem = e.player.Slots;
    //     if (slotSystem == null) { return; }
    //     if (slotSystem.CheckHasSlot(agent))  // if the agent already has a slot
    //     {
    //         // Move to the slot
    //         Transform slot = slotSystem.GetSlot(agent);
    //         float slotDistance = Vector3.Distance(slot.position, agent.transform.position);
    //         //Debug.Log(slotDistance);
    //         if (slotDistance > 0.25f)
    //         {
    //             agent.canMove = true;
    //             agent.MoveAgent(slot);
    //             agent.HandleRotation(e.player.transform.position);
    //         }
    //     }
    //     else
    //     {
    //         // Try getting a slot, if it fails, do nothing
    //         if (!slotSystem.CheckHasSlot(agent))
    //         {
    //             slotSystem.ReserveSlot(agent);
    //         }
    //         agent.HandleRotation(e.player.transform.position);
    //     }

    //     // If you can't attack, back up.
    //     // If you can, move forward
    // }

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
