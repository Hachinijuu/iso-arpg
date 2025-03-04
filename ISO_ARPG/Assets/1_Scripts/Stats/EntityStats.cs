using UnityEngine;

public enum EntityID { NONE, PLAYER, ORC, IMP, BARREL, ROCK, WAGON, POT }

public class EntityStats : MonoBehaviour
{
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
        health.Changed += CheckDied;
    }
    #endregion
    #region FUNCTIONALITY
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
