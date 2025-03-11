using System.Collections.Generic;
using UnityEngine;

public enum EntityID { NONE, PLAYER, ORC, IMP, BARREL, ROCK, WAGON, POT }

public class EntityStats : MonoBehaviour
{
    public EntityData data;
    // EntityStats can have a source GO like playerstats does,
    // The source go will have the expected stats to map to, and entityStats will load from them

    #region VARIABLES
    public EntityID id;
    // All entites have a health stat.
    public TrackedStat Health { get { return health; } }

    // Made serializefield for debug purposes
    // Final implementation, stats are loaded externally
    protected TrackedStat health;

    // Defensive
    public SubStat Armour { get { return armour; } }
    public SubStat Dodge { get { return dodge; } }

    // Defensive
    protected SubStat armour;
    protected SubStat dodge;
    
    // Offensive
    public SubStat Damage { get { return damage; } }
    protected SubStat damage;

    public SubStat AttackSpeed {  get { return attackSpeed; } }
    protected SubStat attackSpeed;

    // Utility
    public SubStat MoveSpeed { get { return moveSpeed; } }
    protected SubStat moveSpeed;

    // Add attack speed and movement speed to enemies for them to have randomized values for elites

    public List<Stat> statList;

    #endregion
    #region EVENTS
    // Send an event out whenever the entity has died, drop system will listen to this.
    public delegate void Died(GameObject go);
    public event Died OnDied;
    private void FireOnDied(GameObject go) { if (OnDied != null) { OnDied(go); } }
    #endregion

    // Assume all entities are returned to an object pool - SetActive(false)

    #region UNITY FUNCTIONS
    // Whenever the object disables, fire the OnDied
    public void Awake()
    {
        LoadData(data);
        FillStatList();
    }

    public void OnEnable()
    {
        health.Changed += CheckDied;
    }

    public void OnDisable()
    {
        health.Changed -= CheckDied;
    }
    #endregion
    #region FUNCTIONALITY

    public virtual void LoadData(EntityData toLoad)
    {
        if (data != null)
        {
            health = new TrackedStat(data.Health);
            armour = new SubStat(data.Armour);
            dodge = new SubStat(data.Dodge);
            damage = new SubStat(data.Damage);
            attackSpeed = new SubStat(data.AttackSpeed);
            moveSpeed = new SubStat(data.MoveSpeed);
        }
    }

    public virtual void FillStatList()
    {
        statList.Add(health);
        statList.Add(armour);
        statList.Add(dodge);
        statList.Add(damage);
        statList.Add(moveSpeed);
        statList.Add(attackSpeed);
    }
    public virtual void CheckDied(float value)
    {
        //Debug.Log("Took damage");
        if (health.Value <= 0)
        {
            //Debug.Log("Died");
            FireOnDied(gameObject);
        }
    }
    #endregion
}
