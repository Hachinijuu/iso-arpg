using UnityEngine;

public class EnemyController : AdvancedFSM
{
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

    // public SlotManager playerSlotManager;
    private void Awake()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }
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

        if (hurtbox != null)
        {
            hurtbox.onDamaged += DamageTaken;
        }

        //if (stats != null)
        //{
        //    stats.OnDied += Died;
        //}

        // Register for drops

        if (DropSystem.Instance != null)
        {
            DropSystem.Instance.RegisterEnemyDrop(stats);
            Debug.Log("Registered to drops");
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
        // TODO: Fill in the States
        //Create States

        // Dead State
        DeadState dead = new DeadState(this);

        // TEMPORARY, MOVE ADDITIONAL STATES TO DERIVED CLASSES
        ChaseState chase = new ChaseState(this);
        chase.AddTransistion(Transition.ReachPlayer, FSMStateID.RangedAttack);
        chase.AddTransistion(Transition.PlayerReached, FSMStateID.MeleeAttack);
        chase.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        MeleeAttackState melee = new MeleeAttackState(this);
        melee.AddTransistion(Transition.ChasePlayer, FSMStateID.Chase);
        melee.AddTransistion(Transition.NoHealth, FSMStateID.Dead);


        RangedAttackState ranged = new RangedAttackState(this);
        ranged.AddTransistion(Transition.ChasePlayer, FSMStateID.Chase);
        ranged.AddTransistion(Transition.ReachPlayer, FSMStateID.MeleeAttack);
        ranged.AddTransistion(Transition.NoHealth, FSMStateID.Dead);


        // Chase State
        //ChaseState chase = new ChaseState(this);
        //chase.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        // Melee Attack State

        // Ranged Attack State

        // Regenerate State

        // ADD all states here
        //AddFSMState(chase);        // Starting state
        // AddFSMState(rangedAttack);
        // AddFSMState(meleeAttack);   
        // AddFSMState(regen);
        AddFSMState(chase);
        AddFSMState(dead);
        AddFSMState(melee);
        AddFSMState(ranged);
    }

    //private void Died()
    //{
    //    PerformTransition(Transition.NoHealth);
    //}

    // Event function so the health checks are not dependent on update loop
    private void DamageTaken(float value)
    {
        Debug.Log("Took damage");
        if (stats.Health.Value <= 0)
        {
            PerformTransition(Transition.NoHealth);
            //Debug.Log("Died");
        }
    }
    private void OnDestroy()
    {
        // When this controller is destroyed, delete the states...

        // Go through all the states added, and delete them

        // This will not be called since the list will be imploded on the destruction of the class.
        //DeleteState(FSMStateID.Chase);
        //DeleteState(FSMStateID.Dead);
    }
}
