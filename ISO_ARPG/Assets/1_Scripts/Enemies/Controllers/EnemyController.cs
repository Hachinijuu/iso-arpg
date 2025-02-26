using UnityEngine;

public class EnemyController : AdvancedFSM
{
    #region  VARIABLES
    public const int DIST_BUFFER = 5;
    public const int MELEEATTACK_DIST = 5;
    public const int RANGEATTACK_DIST = 25;
    public const int CHASE_DIST = 50;

    public bool debugDraw;
    public EntityStats stats;

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField] protected Hitbox damageSource;
    public Hitbox DamageSource { get { return damageSource; } }

    [SerializeField] protected Animator anim;
    public Animator Anim { get { return anim; } }
    [SerializeField] protected Hurtbox hurtbox;

    public static int attackAnimID = Animator.StringToHash("Ability1");
    public static int moveAnimID = Animator.StringToHash("Speed");
    // Do the same for attacks so that each state does not need to cache

    // public SlotManager playerSlotManager;
    #endregion
    #region  UNITY FUNCTIONS
    private void Awake()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.enabled = false;   // DO NOT WANT TO ACTIVATE THE AGENT UNTIL NEEDED
    }
    #endregion
    #region FSM SETUP
    protected override void Initialize()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        if (stats == null)
        {
            stats = new EntityStats();
        }
        //health = new TrackedStat(TrackedStatTypes.HEALTH, 100, 100);
        ConstructFSM();
        // text = StateText.text;   
        if (hurtbox == null)
        {
            hurtbox = GetComponentInChildren<Hurtbox>();
        }

        if (stats != null)
        {
            stats.OnDied += DeathTrigger;
        }

        // Register for drops

        if (DropSystem.Instance != null)
        {
            DropSystem.Instance.RegisterEnemyDrop(stats);
            //Debug.Log("Registered to drops");
        }
        if (AIManager.Instance != null)
        {
            stats.OnDied += context => { AIManager.Instance.UpdateDeathNumbers(); };
        }
    }

    protected override void FSMUpdate()
    {
        if (CurrentState == null) return;                   // If no state is active, do nothing in this function
        CurrentState.Reason(playerTransform, transform);    // If a state exists, make the decision
        CurrentState.Act(playerTransform, transform);       // Then act out the decision
        //StateText.text = text + GetStateString();
        if (debugDraw)
        {
            UsefullFunctions.DebugRay(transform.position, transform.forward * 5.0f, Color.red);
        }
    }

    // Construct FSM is virutal, so unique enemy controllers can derive and modify which states they use.
    // This is a wrapper function to get the order correct
    protected virtual void ConstructFSM()
    {
        // Dead State
        DeadState dead = new DeadState(this);
        AddFSMState(dead);
    }
#endregion

#region  FUNCTIONALITY

    public virtual void Respawn()
    {
        navMeshAgent.enabled = true;
        if (stats.Health.Value <= 0)
        {
            stats.Health.Value = stats.Health.MaxValue; // Reset health to max
        }

        // Override back to default transition
        //if (CurrentState != null)
        //    PerformTransition(Transition.ChasePlayer);
    }

    private void DeathTrigger(GameObject go)
    {
        if (stats.Health.Value <= 0)
        {
            PerformTransition(Transition.NoHealth);
        }
    }

    // Event function so the health checks are not dependent on update loop
    //private void DamageTaken(float value)
    //{
    //    //Debug.Log("Took damage");
    //    if (stats.Health.Value <= 0)
    //    {
    //        PerformTransition(Transition.NoHealth);
    //    }
    //}
#endregion
}
