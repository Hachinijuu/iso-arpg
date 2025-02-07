using System.Collections.Generic;
using UnityEngine;

public class DropSystem : MonoBehaviour
{
    #region VARIABLES
    private static DropSystem instance = null;
    public static DropSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DropSystem>();
            if (!instance)
                Debug.LogWarning("[DropSystem]: No Drop System found");
            return instance;
        }
    }

    private List<EntityStats> enemies;
    private List<EntityStats> destructibles;

    [SerializeField] ItemObject expOrb;
    #endregion
    #region INITIALIZATION
    public void InitDropLists()
    {
        enemies = new List<EntityStats>();
        destructibles = new List<EntityStats>();
    }

    // When enemies set themselves to active, register them to the list
    public void RegisterEnemyDrop(EntityStats enemy)
    {
        enemies.Add(enemy);
        enemy.OnDied += CheckDrop;
    }

    public void UnregisterEnemyDrop(EntityStats enemy)
    {
        enemies.Remove(enemy);
        enemy.OnDied -= CheckDrop;
    }

    // When destructibles set themselves to inactive, unregister them from the list
    public void RegisterDestructibleDrop(EntityStats destructible)
    {
        destructibles.Add(destructible);
        destructible.OnDied += CheckDrop;
    }

    public void UnregisterDestructibleDrop(EntityStats destructible)
    {
        destructibles.Remove(destructible);
        destructible.OnDied -= CheckDrop;
    }

    //public void AddListeners()
    //{
    //    //Debug.Log("[DropSystem]: Has " + enemies.Count + " enemies");
    //    //foreach (EntityStats enemy in enemies)
    //    //{
    //    //    enemy.OnDied += CheckDrop;
    //    //}
    //    //Debug.Log("[DropSystem]: Added listeners");
    //}
    #endregion
    #region FUNCTIONALITY

    private void CheckDrop(GameObject whoDied) // check the thing and map it to what it is supposed to drop
    {

        float dropValue = Random.Range(0, expOrb.dropChance * 2);

        if (dropValue > expOrb.dropChance)
            CreatedDroppedObject(whoDied.transform.position, expOrb);
        else
            Debug.Log("[DropSystem]: No drops");
        //Debug.Log("Will I drop something?");
    }

    // Create dropped object
    public void CreatedDroppedObject(Vector3 pos, ItemObject item)
    {
        GameObject go = GameObject.Instantiate(item.prefab);
        // This creates the object, but where does it create the object...
        // Create it at the parent of whoever called this 

        go.transform.position = pos;

        // If it is not active
        if (!go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
    // This drop system manages all drops in the game
    // It will contain a dictionary to the possible drops and stuff
    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Awake()
    {
        InitDropLists();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}
