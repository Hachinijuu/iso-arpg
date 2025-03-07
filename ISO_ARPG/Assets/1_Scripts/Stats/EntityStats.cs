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
    [SerializeField] protected TrackedStat health;

    // Defensive
    public SubStat Armour { get { return armour; } }
    public SubStat Dodge { get { return dodge; } }

    // Defensive
    [SerializeField] protected SubStat armour;
    [SerializeField] protected SubStat dodge;
    public SubStat Damage { get { return damage; } }
    [SerializeField] protected SubStat damage;
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
            Debug.Log(damage.Value);
        }
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
