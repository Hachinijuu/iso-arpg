using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "State", menuName = "sykcorSystems/AI/State", order = 1)]
public abstract class AIState : ScriptableObject
{
    // Possible States for Agents
    public enum StateId 
    {
        None = 0,       // None is default, can relate to idle
        Idle,
        Chase,          // Agent is chasing after target
        MeleeAttack,    // Agent is melee attacking the target
        RangedAttack,   // Agent is ranged attacking the target
        Regenerating,   // Agent is regenerating themself
        Patrol,         // Agent is patrolling
        Dead,           // Agent has died
    }
    // The methods given an AI state are expected to have static functionality
    // I.e, the agent will pass themselves to the function, and act given that passed information

    // This isolates it, such that the agent does not need to have it's own state created for behavioural purporses
    // Instead, the state simply handles the action / decision making, given the agent

    // However, since most logic is abstracted, decision making for individuals may not even be necessary, if the manager handles the transitional logic
    public virtual void EnterState(AgentStateArgs e) { }
    public virtual void ExitState(AgentStateArgs e) { }
    public abstract void Reason(AgentStateArgs e);
    public abstract void Act(AgentStateArgs e);


    // Helper methods
    protected virtual bool IsInCurrentRange(Vector3 start, Vector3 goal, int range)
    {
        bool inRange = false;

        // Transform is the goal, pos is the start
        float dist = GetSquareDistance(start, goal);

        if (dist <= Mathf.Pow(range, 2))
        {
            inRange = true;
        }
        return inRange;
    }

    protected virtual bool IsInCurrentRange(Vector3 start, Vector3 goal, float range)
    {
        bool inRange = false;

        // Transform is the goal, pos is the start
        float dist = GetSquareDistance(start, goal);

        if (dist <= Mathf.Pow(range, 2))
        {
            inRange = true;
        }
        return inRange;
    }

    protected virtual float GetSquareDistance(Vector3 start, Vector3 goal)
    {
        return (goal - start).sqrMagnitude;
    }

    protected virtual Vector3 GetCirclePoint(float min, float max, Transform source)
    {
        int randPoint = Random.Range(0, CircleUtility.MAX_CIRCLE_POSITIONS);
        float offset = Random.Range(min, max);
        Vector3 target = source.position;
        target.x += (CircleUtility.CircleListInstance[randPoint].x * offset);
        target.z += (CircleUtility.CircleListInstance[randPoint].z * offset);
        return target;
    }
}
