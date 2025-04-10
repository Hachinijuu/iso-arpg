using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "sykcorSystems/AI/States/RangedAttack", order = 3)]
public class AIRangedAttack : AIState
{
    //[Header("Attack Parameters")]
    //public float hitboxUptime = 0.5f;

    [Header("Animations")]
    private static string AttackTrigger = "Attack";
    private int animId = Animator.StringToHash(AttackTrigger);

    [Header("Attack Parameters")]
    [SerializeField] bool moveAfterAttacks;
    [SerializeField] float minMoveDistance = 0.5f;
    [SerializeField] float maxMoveDistance = 1.25f;
    [SerializeField] float moveThreshold = 0.25f;

    public override void EnterState(AgentStateArgs e)
    {
        e.agent.Destination = e.agent.transform.position;
    }

    public override void Reason(AgentStateArgs e)
    {
        // Does any movement get driven when in this state, or is it simply shoot and transition out given the conditions

        // If the enemy has entered the melee threshold, enter the melee attack state

        EnemyControllerV2 agent = e.agent;
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
            if (agent.stats.id == EntityID.ELITE)   // If I am an elite enemy
            {
                // And the target is outside of my ranged attack range
                if (!(IsInCurrentRange(playerPos, agentPos, AIManager.RANGED_RANGE)))
                {
                    // Check if I am low on health, if I am regen.
                    if (agent.stats.Health.Value < agent.stats.Health.MaxValue)
                    {
                        agent.SetState(StateId.Regenerating);
                    }
                    else
                    {
                        agent.SetState(StateId.SpecialAttack);
                    }
                }
            }
            // If the target is not in ranged attack distance (further range)
            else
            {
                if (!(IsInCurrentRange(playerPos, agentPos, AIManager.RANGED_RANGE)))
                {
                    agent.SetState(StateId.Chase);
                    return;
                }
            }
        }


        //throw new System.NotImplementedException();
    }

    public override void Act(AgentStateArgs e)
    {
        //throw new System.NotImplementedException();
        EnemyControllerV2 agent = e.agent;
        agent.HandleRotation(e.player.transform.position);
        if (agent.canAttack) 
        { 
            if (agent.stats.id == EntityID.BIG_ELITE)
            {
                EliteController elite = agent as EliteController;
                if (elite)
                {
                    elite.RangedAttack(e.player.transform.position);
                }
            }
            else
            {
                agent.Attack();
            }
        }

        if (!moveAfterAttacks) { return; }
        if (!agent.canAttack)   // If the agent has performed their attack
        {
            float dist = Vector3.Distance(agent.transform.position, agent.Destination);
            if (dist < moveThreshold)
            {
                Vector3 shiftPos = GetCirclePoint(minMoveDistance, maxMoveDistance, agent.transform);   // This just causes agent jittering
                // Get the direction away from the player
                Vector3 dir = (agent.transform.position - e.player.transform.position).normalized;
                agent.Destination = shiftPos + dir;
            }
            // Check the distance between the agent and their destination, it if is small, then move them again

            agent.MoveAgent();
            // Need to store a destination position
            //agent.MoveAgent(shiftPos);
            //agent.HandleRotation(shiftPos);
        }
        // after performing an attack, the agent can shift if they are NOT a turret
        // this can be dictated as a flag
        //agent.Animator.SetTrigger(animId);

        // Vector3 playerPos = e.player.transform.position;
        // distance = Vector3.Distance(agent.transform.position, playerPos);
        // // If the agent is close enough to attack, perform the attack and then wait for the interval time
        // if (distance < attackRange && agent.canAttack)
        // {
        //     // Unreserve the slot
        //     if (e.player.Slots != null && e.player.Slots.CheckHasSlot(agent))
        //     {
        //         e.player.Slots.UnreserveSlot(agent);
        //     }
        // }
        // Fire a projectile from the AI's projectile source
        // But if projectiles are bound to the 
    }
}
