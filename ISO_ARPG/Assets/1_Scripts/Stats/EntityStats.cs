using UnityEngine;

public class EntityStats : MonoBehaviour
{
    // All entites have a health stat.
    public TrackedStat Health { get { return health; } }

    // Made serializefield for debug purposes
    // Final implementation, stats are loaded externally
    [SerializeField] protected TrackedStat health;

    #region EVENTS
    // Send an event out whenever the entity has died, drop system will listen to this.
    public delegate void Died(GameObject go);
    public event Died OnDied;
    private void FireOnDied(GameObject go) { if (OnDied != null) { OnDied(go); } }
    #endregion

    // Assume all entities are returned to an object pool - SetActive(false)

    // Whenever the object disables, fire the OnDied
    public void Awake()
    {
        health.Changed += CheckDied;
    }
    private void CheckDied(float value)
    {
        //Debug.Log("Took damage");
        if (health.Value <= 0)
        {
            //Debug.Log("Died");
            FireOnDied(gameObject);
        }
    }
}
