using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyControllerV2 : MonoBehaviour
{
    // This script will NOT have it's own update functionality, instead it will rely on inserted states, and handle the transiiton
    // It will be responsible for moving the agent that is attached to it, and holding references to the respective components on the agent
    //public static float agentAvoidance = 0.25f;  // How large the area THIS agent will search around when avoiding others
    //public static float agentSpacing = 1.5f;    // The space to shift agents
    public LayerMask agentLayer;
    public LayerMask avoidLayer;
    public static float avoidDistance = 5.0f;
    public static float avoidSpacing = 7.5f;
    public static float stopDistance = 0.5f;
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

    public AIState.StateId StateID { get { return currStateId; } }
    AIState.StateId currStateId;

    public AIState State { get { return currState; } }
    AIState currState;

    public float CurrentSpeed { get { return currSpeed; } } 

    public float rotationSpeed = 20.0f;

    //public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    private static string moveAnimation = "Speed";
    private int moveId = Animator.StringToHash(moveAnimation);

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
        AIManager.Instance.UpdateDeathNumbers(this);        // Unregister from the AI List
        DropSystem.Instance.UnregisterEnemyDrop(stats); // Unregister from the drop system
        
        // Check for owned slots
        PlayerSlotSystem slotSystem = PlayerManager.Instance.currentPlayer.Slots;
        if (slotSystem != null && slotSystem.CheckHasSlot(this))
        {
            slotSystem.UnreserveSlot(this);
        }
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

    public bool ActRunning { get { return actRunning; } }
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
            currState.Act(e);   // Do the action
            yield return new WaitForSeconds(state.intervalTime);    // Countdown time
            // Then set coroutine to null, to cancel out the usage
            actRunning = false; // Then set to false to allow reusage
            //if (cycleComplete)
            //{
            //    currState.Act(e);
            //}
        }
    }

    [Header("Attack Parameters")]
    public float hitboxUptime = 0.5f;
    public float attackRange = 1.0f;
    public float attackClock = 0.0f;
    public float attackCooldown = 5.0f;
    public bool canAttack = true;
    //float ICoroutineState.intervalTime { get { return attackCooldown; } set { attackCooldown = value; } }

    [Header("Animations")]
    private static string AttackTrigger = "Attack";
    private int animId = Animator.StringToHash(AttackTrigger);
    public virtual void Attack()
    {
        if (canAttack)
        {
            // Perform the attack (play the animation)
            canAttack = false;
            //Debug.Log("Performed Attack");
            //animator.SetTrigger(animId);
            hitbox.Attack();
            //hitbox.AttackForTime(hitboxUptime);
            //hitbox.AllowDamageForTime(hitboxUptime);
            //Invoke(nameof(ResetAttack), attackCooldown);
            StartCoroutine(AttackTimer());
        }
        // Default attack is 
    }

    IEnumerator AttackTimer()
    {
        while (!canAttack)
        {
            attackClock += Time.deltaTime;
            //Debug.Log(attackClock);
            if (attackClock > attackCooldown)
            {
                canAttack = true;
                attackClock = 0.0f;
            }
            yield return null;
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    // Elite will derive from this controller, states will check for elite to perform ranged attack override instead of calling attack
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
            steer += (transform.position - hit.transform.position) / 4; // Reduce the strength, prefer side
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
        return steer.normalized * steerPower;
    }
    float steerPower = 5.0f;
    float speedShift = 0.25f;    // 0.5 * 0 = 0
    float minSpeed = 0.05f;
    float currSpeed;
    public bool canMove = true;
    Vector3 debugTarget;

    public void AnimateAgentMove()
    {
        //currSpeed = rb.velocity.magnitude;
        animator.SetFloat(moveId, currSpeed);
    }

    public void MoveAgent(Vector3 target)
    {
        if (target == null) { return; }
        if (!canMove) { currSpeed = 0; return; }
        // When moving the agent, we need to avoid other agents if possible (for nicer control)
        // Therefore, how can spacing be handled nicely?
        //debugTarget = target;
        //Vector3 current = transform.position;
        //Vector3 avoid = AvoidOthers();
        //avoid.y = 0;
        //Vector3 offset = target + (avoid * avoidSpacing);
//
        ////if (CheckStop()) { return; } // If I SHOULD stop
        //// If you cannot get to your target without intersecting an existing agent, stop.
        //Vector3 movement = Vector3.MoveTowards(transform.position, offset, ((stats.MoveSpeed.Value * (1 - (speedShift * avoid.magnitude))) + minSpeed) * Time.deltaTime);
        //movement.y = 0; // This is expected 0, y-level for all levels, otherwise, movement will not look as expected
        
        Vector3 movement = Vector3.MoveTowards(transform.position, target, stats.MoveSpeed.Value * Time.deltaTime);
        transform.position = movement;

        currSpeed = (transform.position - target).magnitude;    // Magnitude of the direction
        //Debug.Log(speed);
        //Debug.Log(agent.desiredVelocity);
        //speed = body.velocity.magnitude;//(transform.position - moveTarget).magnitude;    // Magnitude of the direction
        if (currSpeed <= 0.25f)
            currSpeed = 0;
        //rb.MovePosition(movement);
        //transform.position = movement;

        // float moved = Vector3.Distance(current, movement);
        // currSpeed = moved / Time.deltaTime;
        // animator.SetFloat(moveId, currSpeed);
        // if (currSpeed <= 0.1f)
        //     currSpeed = 0;
        //HandleRotation(target); // Call handle rotation externally, so that positioning is proper
        //rb.MovePosition(movement);
        //transform.position = movement;
    }

    public void MoveAgentNoAvoidance(Vector3 target)
    {
        if (target == null) { return; }
        if (!canMove) { currSpeed = 0; return; }
        Vector3 movement = Vector3.MoveTowards(transform.position, target, stats.MoveSpeed.Value * Time.deltaTime);
        movement.y = 0; // This is expected 0, y-level for all levels, otherwise, movement will not look as expected
        
        transform.position = movement;
        //rb.MovePosition(movement);
    }

    public void MoveAgent(Transform target)
    {
        MoveAgent(target.position);
    }

    public void MoveAgentAround(Vector3 target, Transform avoid = null)
    {
        Vector3 avoidPos = Vector3.zero;
        if (avoid == null)
        {
            // Calculate avoidance based on avoid function
            avoidPos = AvoidOthers();
        }
        else
        { 
            avoidPos = avoid.position;
        }

        //Vector3 offset = 

        // Move the agent to the target position while steering away from the avoid
        // With the steering calculated

    }

    public void HandleRotation(Vector3 direction)
    {
        // Apply the look
        Quaternion targetRotation = Quaternion.LookRotation(direction - transform.position);
        Quaternion look = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        //rb.MoveRotation(look);
    }
    #endregion

    #if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        Vector3 textPos = transform.position;
        textPos.y = 2.5f;
        Handles.Label(textPos, "State: " + currStateId, style);
        Debug.DrawRay(transform.position, transform.forward * avoidDistance);

        //Vector3 avoid = AvoidOthers();
        //avoid.y = 0;
        //Vector3 offset = debugTarget + (avoid * avoidSpacing);
        //Debug.DrawRay(transform.position, (offset - transform.position) * Vector3.Distance(transform.position, offset), Color.green);
        //Debug.DrawRay(transform.position, (debugTarget - transform.position) * Vector3.Distance(transform.position, debugTarget), Color.blue);
    }
    #endif
}
