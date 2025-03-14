using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerV2 : MonoBehaviour
{
    // This script will NOT have it's own update functionality, instead it will rely on inserted states, and handle the transiiton
    // It will be responsible for moving the agent that is attached to it, and holding references to the respective components on the agent
    public Hurtbox Hurtbox { get { return hurtbox; } }
    [SerializeField] private Hurtbox hurtbox;
    
    public Hitbox Hitbox { get { return hitbox; } }
    [SerializeField] private Hitbox hitbox;

    public EntityStats stats;
    public Rigidbody Body { get { return rb; } }
    [SerializeField] Rigidbody rb;

    public Animator Animator { get { return animator; } }
    [SerializeField] Animator animator;

    // Possibly FSM v2 which contains the states and the state transitions?

    // StateHolder, AI get's a reference to a static list of states
    // Only take the states they need

    // Given the current state, the agent will behave based on whatever is defined in the state
    public StateContainer States { get { return states; } set { states = value; } }
    [SerializeField] StateContainer states;

    public AIState.StateId State { get { return currStateId; } }
    AIState.StateId currStateId;
    AIState currState;

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
                SetState(AIState.StateId.Chase); // Force chase temporary check
            }
        }
        
        Debug.Log("Agent respawned");
    }

    public void Died()
    {
        // Unregister from the list
        AIManager.Instance.UpdateDeathNumbers();        // Unregister from the AI List
        DropSystem.Instance.UnregisterEnemyDrop(stats); // Unregister from the drop system
        gameObject.SetActive(false);    // Disable the agent
    }
    #endregion
    #region State Handling
    public void SetState(AIState.StateId id)
    {
        // When the state is set, if a state already exists, call the exit on it
        AgentStateArgs args = new AgentStateArgs();
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
    public void HandleState(AgentStateArgs e)
    {
        if (currState == null) { return; }
        currState.Reason(e);
        currState.Act(e);
    }
    #endregion
    #region Movement
    // There are different move functions
    // Move agent will a CELL relies on the flowfield for navigation to move to the specified target
    public void MoveAgent(Cell target)
    {
        if (target == null) { return; }
        Vector3 direction = new Vector3(target.bestDirection.vector.x, 0, target.bestDirection.vector.y);
        rb.velocity = direction * stats.MoveSpeed.Value; // * Time.deltaTime;
    }

    // Move agent with a transform moves the agent directly to that position, this can presumably be used with a slot system
    public void MoveAgent(Vector3 target)
    {
        if (target == null) { return; }
        Vector3 movement = Vector3.MoveTowards(transform.position, target, stats.MoveSpeed.Value * Time.deltaTime);
        transform.position = movement;
    }

    public void MoveAgent(Transform target)
    {
        if (target == null) { return; }
        Vector3 movement = Vector3.MoveTowards(transform.position, target.position, stats.MoveSpeed.Value * Time.deltaTime);
        transform.position = movement;
    }
    #endregion
}
