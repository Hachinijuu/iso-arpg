using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System;

public class EnemyControllerV2 : MonoBehaviour
{
    // This script will NOT have it's own update functionality, instead it will rely on inserted states, and handle the transiiton
    // It will be responsible for moving the agent that is attached to it, and holding references to the respective components on the agent
    //public static float agentAvoidance = 0.25f;  // How large the area THIS agent will search around when avoiding others
    //public static float agentSpacing = 1.5f;    // The space to shift agents
    public LayerMask agentLayer;
    public LayerMask avoidLayer;
    public static float avoidDistance = 1.5f;
    public static float avoidSpacing = 3.0f;
    public static float stopDistance = 0.5f;
    public Hurtbox Hurtbox { get { return hurtbox; } }
    [SerializeField] private Hurtbox hurtbox;
    
    public Hitbox Hitbox { get { return hitbox; } }
    [SerializeField] private Hitbox hitbox;

    public EntityStats stats;
    // public Rigidbody Body { get { return rb; } }
    // [SerializeField] Rigidbody rb;

    public Animator Animator { get { return animator; } }
    [SerializeField] Animator animator;

    // Possibly FSM v2 which contains the states and the state transitions?

    // StateHolder, AI get's a reference to a static list of states
    // Only take the states they need

    // Given the current state, the agent will behave based on whatever is defined in the state
    public StateContainer States { get { return states; } set { states = value; } }
    [SerializeField] StateContainer states;

    public AIState.StateId StateID { get { return currStateId; } }
    AIState.StateId currStateId;

    public AIState State { get { return currState; } }
    AIState currState;

    public float CurrentSpeed { get { return currSpeed; } } 

    public float rotationSpeed = 20.0f;

    #region Initalization
    private void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (DropSystem.Instance != null)
        {
            DropSystem.Instance.RegisterEnemyDrop(stats);
        }
        if (AIManager.Instance != null)
        {
            stats.OnDied += context => { Died(); };
        }
    }

    // How to handle hit-reaction and hit-stun
    // health.onChanged notifies the listerner when the health value has changed
    // This is both for gaining health and losing health, how do we know health has been lost?
    // The old value..
    // If the old value is greater than the new incoming value, we have lost health (not as much as before)
    // In those instances, we want to do the action of taking damage
    // Alternatively, in the take damage calls.

    // When a hitbox / hurtbox interact, the hit detection logic occurs
    // We can find the controller from the box, and play the hit reaction there.
    public void Respawn()
    {
        stats.Health.Value = stats.Health.MaxValue; // reset the health value to max
        AIManager.Instance.RegisterToList(this);

        if (currState == null && states != null)
        {
            if (states.stateMap.Count > 0)
            {
                SetState(AIState.StateId.Idle); // Force chase temporary check
            }
        }
        
        //Debug.Log("Agent respawned");
    }

    public void Died()
    {
        // Unregister from the list
        AIManager.Instance.UnregisterFromList(this);
        AIManager.Instance.UpdateDeathNumbers();        // Unregister from the AI List
        DropSystem.Instance.UnregisterEnemyDrop(stats); // Unregister from the drop system
        gameObject.SetActive(false);    // Disable the agent
    }
    #endregion
    #region State Handling
    // public void SetState(AIState.StateId id)
    // {
    //     // When the state is set, if a state already exists, call the exit on it
    //     AgentStateArgs args = new AgentStateArgs();
    //     args.agent = this;
    //     if (currState != null)
    //     {
    //         currState.ExitState(args);
    //     }

    //     // Once the previous state has been cleaned up, set the new ID and lookup the next state
    //     currStateId = id;
    //     if (states.stateMap.TryGetValue(currStateId, out AIState foundState))
    //     {
    //         currState = foundState;
    //         currState.EnterState(args);
    //     }
    // }

    public void SetState(AIState.StateId id, AgentStateArgs args = null)
    {
        // When the state is set, if a state already exists, call the exit on it
        if (args == null)
        {
            args = new AgentStateArgs();
        }
        args.agent = this;
        if (currState != null)
        {
            currState.ExitState(args);
        }

        // Once the previous state has been cleaned up, set the new ID and lookup the next state
        currStateId = id;
        if (states.stateMap.TryGetValue(currStateId, out AIState foundState))
        {
            currState = foundState;
            currState.EnterState(args);
        }
    }

    bool actRunning = false;
    public void HandleState(AgentStateArgs e)
    {
        if (currState == null) { return; }
        // If the state exists, call the logic on the state
        currState.Reason(e);

        if (currState is ICoroutineState)
        {
            IEnumerator act = CoroutineAct(e);
            if (!actRunning)    // If the act is not running, start the coroutine
            { 
                StartCoroutine(act);
            }
        }
        else
        {
            //Debug.Log("ACT");
            currState.Act(e);
        }

        // However, is there a possiblity to detect the type of state it is
        // If it is a coroutine based state, the logic handling is different
        // It will do reason decisions based on the update loop
        // But if it is a coroutine based state, the act will run in a coroutine timer?

    }

    // How to call the function from internally in the state


    // Only want to start this coroutine if there is none running already
    IEnumerator CoroutineAct(AgentStateArgs e)
    {
        if (currState is ICoroutineState state)
        {
            actRunning = true;
            yield return new WaitForSeconds(state.intervalTime);    // Countdown time
            currState.Act(e);   // Do the action
            // Then set coroutine to null, to cancel out the usage

            actRunning = false;
            //if (cycleComplete)
            //{
            //    currState.Act(e);
            //}
        }
    }
    #endregion
    #region Movement
    // There are different move functions
    // Move agent will a CELL relies on the flowfield for navigation to move to the specified target
    // Vector3 AvoidAgents()
    // {
    //     Collider[] neighbours = Physics.OverlapSphere(transform.position, agentSpacing, agentLayer);
    //     Vector3 spacing = Vector3.zero;
    //     if (neighbours == null) { return spacing; }
    //     foreach (Collider agent in neighbours)
    //     {
    //         if (agent.gameObject == gameObject) { continue; } // If the collider that was found was myself, skip
    //         spacing += (transform.position - agent.transform.position).normalized;  // My current position, offset by the positions of the neighbour agents
    //         Debug.Log(spacing);
    //     }

    //     //Debug.Log(spacing.normalized * agentSpacing);
    //     return spacing.normalized * agentSpacing;
    // }
    public void MoveAgent(Cell target)
    {
        if (target == null) { return; }
        Vector3 direction = new Vector3(target.bestDirection.vector.x, 0, target.bestDirection.vector.y) + target.position;
        MoveAgent(direction);
        //rb.velocity = direction * stats.MoveSpeed.Value; // * Time.deltaTime;
    }

    // Move agent with a transform moves the agent directly to that position, this can presumably be used with a slot system
    private Vector3 AvoidOthers()   // Return the steering amount
    {
        Vector3 steer = Vector3.zero;
        
        // Build a cone of raycasts, can also serve as LOS
        Vector3 forward = transform.forward;
        Vector3 left = Quaternion.Euler(0, -60, 0) * forward;
        Vector3 right = Quaternion.Euler(0, 60, 0) * forward; 

        RaycastHit hit;
        if (Physics.Raycast(transform.position, forward, out hit, avoidDistance, avoidLayer))
        {
            // If there is something to avoid in FRONT of me
            steer += (transform.position - hit.transform.position);
            //return steer.normalized;
        }
        if (Physics.Raycast(transform.position, left, out hit, avoidDistance, avoidLayer))
        {
            steer += (transform.position - hit.transform.position);
            //return steer.normalized;
        }
        if (Physics.Raycast(transform.position, right, out hit, avoidDistance, avoidLayer))
        {
            steer += (transform.position - hit.transform.position);
            //return steer.normalized;
        }
        return steer.normalized;
    }
    float speedShift = 0.5f;    // 0.5 * 0 = 0
    float currSpeed;
    public bool canMove = true;
    public void MoveAgent(Vector3 target)
    {
        if (target == null) { return; }
        if (!canMove) { currSpeed = 0; return; }
        // When moving the agent, we need to avoid other agents if possible (for nicer control)
        // Therefore, how can spacing be handled nicely?
        Vector3 current = transform.position;
        Vector3 avoid = AvoidOthers();
        avoid.y = 0;
        Vector3 offset = target + (avoid * avoidSpacing);

        //if (CheckStop()) { return; } // If I SHOULD stop
        // If you cannot get to your target without intersecting an existing agent, stop.
        Vector3 movement = Vector3.MoveTowards(transform.position, offset, (stats.MoveSpeed.Value * (1 - (speedShift * avoid.magnitude))) * Time.deltaTime);
        movement.y = 0; // This is expected 0, y-level for all levels, otherwise, movement will not look as expected
        transform.position = movement;

        float moved = Vector3.Distance(current, movement);
        currSpeed = moved / Time.deltaTime;
        if (moved <= 0.1f)
            currSpeed = 0;
        //HandleRotation(target); // Call handle rotation externally, so that positioning is proper
        //rb.MovePosition(movement);
        //transform.position = movement;
    }

    public void MoveAgent(Transform target)
    {
        MoveAgent(target.position);
    }

    public void HandleRotation(Vector3 direction)
    {
        // Apply the look
        Quaternion targetRotation = Quaternion.LookRotation(direction - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    #endregion

    public void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        Vector3 textPos = transform.position;
        textPos.y = 2.5f;
        Handles.Label(textPos, "State: " + currStateId, style);
        Debug.DrawRay(transform.position, transform.forward * avoidDistance);
    }
}
