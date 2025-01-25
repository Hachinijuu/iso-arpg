using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : AdvancedFSM
{
    public const int MELEEATTACK_DIST = 35;
    public const int RANGEATTACK_DIST = 55;
    public const int CHASE_DIST = 80;

    public bool debugDraw;
    public Text StateText;
    private string text;

    //Set/Get/Decrement/Add to Health functions

    public EntityStats stats;
    //public int health;
    //public int GetHealth() { return health; }
    //public void SetHealth(int inHealth) { health = inHealth; }
    //public void DecHealth(int amount) { health = Mathf.Max(0, health - amount); }
    //public void AddHealth(int amount) { health = Mathf.Min(100, health + amount); }

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    [SerializeField] Hurtbox hurtbox;

    // public SlotManager playerSlotManager;

    private string GetStateString()
    {
        string state = "NONE";
        if (CurrentState.ID == FSMStateID.Dead)
        {
            state = "DEAD";
        }
        
        else if (CurrentState.ID == FSMStateID.Chase)
        {
            state = "CHASING";
        }

        else if (CurrentState.ID == FSMStateID.RangedAttack)            
        {
            state = "RANGEDATTACK";
        }
        else if (CurrentState.ID == FSMStateID.MeleeAttack)
        {
            state = "MELEEATTACK";
        }
        
        else if (CurrentState.ID == FSMStateID.Regenerating)
        {
            state = "REGENERATING";
        }

        //state = state + " H " + health;

        return state;
    }
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
    }

    protected override void FSMUpdate()
    {
        if (CurrentState == null) return;
        arrowElapsedTime += Time.deltaTime;
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);
        //StateText.text = text + GetStateString();
        if (debugDraw)
        {
            UsefullFunctions.DebugRay(transform.position, transform.forward * 5.0f, Color.red);
        }
    }
    private void ConstructFSM()
    {

        // TODO: Fill in the States
        //Create States

        // Dead State
        DeadState dead = new DeadState(this);

        // Chase State
        ChaseState chase = new ChaseState(this);
        chase.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        // Melee Attack State

        // Ranged Attack State

        // Regenerate State

        // ADD all states here
        AddFSMState(chase);        // Starting state
                                   // AddFSMState(rangedAttack);
                                   // AddFSMState(meleeAttack);   
                                   // AddFSMState(regen);
        AddFSMState(dead);
    }

    // Event function so the health checks are not dependent on update loop
    private void DamageTaken(float value)
    {
        if (stats.Health.Value <= 0)
        {
            PerformTransition(Transition.NoHealth);
            Debug.Log("Died");
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
