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

    [SerializeField] List<DropTable> dropTables;
    //[SerializeField] ItemData expOrb;
    #endregion
    #region INITIALIZATION
    public void InitDropLists()
    {
        enemies = new List<EntityStats>();
        destructibles = new List<EntityStats>();
        if (dropTables == null)
            dropTables = new List<DropTable>();
    }

    public GameObject[] destructionParticles;

    // When enemies set themselves to active, register them to the list
    public void RegisterEnemyDrop(EntityStats enemy)
    {
        enemies.Add(enemy);
        enemy.OnDied += context => { CheckDrop(enemy); };
    }

    public void UnregisterEnemyDrop(EntityStats enemy)
    {
        enemies.Remove(enemy);
        enemy.OnDied -= context => { CheckDrop(enemy); };
    }

    // When destructibles set themselves to inactive, unregister them from the list
    public void RegisterDestructibleDrop(EntityStats destructible)
    {
        destructibles.Add(destructible);
        destructible.OnDied += context => { CheckDrop(destructible); SpawnParticle(context); HandleDestruction(context); };
    }

    public void UnregisterDestructibleDrop(EntityStats destructible)
    {
        destructibles.Remove(destructible);
        destructible.OnDied -= context => { CheckDrop(destructible); SpawnParticle(context); HandleDestruction(context); };
    }

    public void RegisterChestDrops(EntityStats chest)
    {

    }

    public void HandleDestruction(GameObject source)
    {
        source.SetActive(false);    // cleanup
        // play a destruction sound
    }
    public void SpawnParticle(GameObject source)
    {
        if (destructionParticles != null && destructionParticles.Length > 0)
        {
            source.SetActive(false);
            GameObject particle = Instantiate<GameObject>(destructionParticles[Random.Range(0, destructionParticles.Length)]);
            particle.transform.position = source.transform.position;

            particle.SetActive(true);
            Destroy(particle, 2);
        }
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

    private void CheckDrop(EntityStats whoDied) // check the thing and map it to what it is supposed to drop
    {
        // match the entity to the drop table
        DropTable dropTable = null;
        foreach (DropTable table in dropTables)
        {
            if (table.dropId == whoDied.id)
            {
                dropTable = table;
                break;
            }
        }
        //Debug.Log(dropTable.dropId);
        if (dropTable == null)
            return;

        // Drop rolling
        foreach (ItemData item in dropTable.items)
        {
            float dropValue = Random.Range(0, 100f);

            // If I get 80 and I have a 20% chance to drop an item
            // I do not get the item, because I rolled an 80.
            // I have an 80% chance NOT to get the item 20-100
            if (dropValue <= item.dropChance)
            {
                if (item is RuneData rune && item.type == ItemTypes.RUNE) // If the item that dropped is a rune, do a roll in the rune system to generate the stats
                {
                    // Do another roll for the rune rarity, this is relative to the difficulty
                    rune = RuneSystem.Instance.RollRuneStats(rune);
                }
                CreatedDroppedObject(whoDied.transform.position, item); // This will create the rune item with the modded data?
                Debug.Log("[DropSystem]: Dropped an item");
            }
        }

        //float dropValue = Random.Range(0, expOrb.dropChance * 2);
        //
        //if (dropValue > expOrb.dropChance)
        //    CreatedDroppedObject(whoDied.transform.position, expOrb);
        //else
        //    Debug.Log("[DropSystem]: No drops");
        //Debug.Log("Will I drop something?");
    }

    // Create dropped object
    public void CreatedDroppedObject(Vector3 pos, ItemData item)
    {
        GameObject go = GameObject.Instantiate(item.prefab);
        // This creates the object, but where does it create the object...
        // Create it at the parent of whoever called this 

        go.transform.position = pos;

        // Call specific functions based on the type it is, pickup vs interactable
        // Let the created object know what kind of item it is
        Item temp = go.GetComponent<Item>();
        if (temp)
        {
            temp.LoadItemData(item);
            // Add a listener to the drop if it should provide information when it is picked up.
            if (item.showPopup)
            {
                item.OnItemAcquired += ItemPopup; // Once the item has popped up, need to stop listening to the event
            }
        }
        else
        {
            Debug.Log("[DropSystem]: Created " + item.itemName + " but object does not have Item Component");
        }
        if (!go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }

    // Get a dropped object from an object pool
    public Item GetItem(ObjectPool pool)
    {
        // Will the dropSystem contain object pools for items, perhaps.
        GameObject go = pool.GetPooledObject();
        Item item = go.GetComponent<Item>();
        return item;    // Should return null if GetComponent<>(); was unsucessful, test.
    }

    public void ItemPopup(ItemEventArgs e)
    {
        // Build the UI popup to display on screen.
        ItemData item = e.data;
        //Debug.Log("[DropSystem]: Show popup for: " + item.itemName);
        item.OnItemAcquired -= ItemPopup;
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
    #endregion

    // TODO for drop tables
    // When an entity registers to the drop system, the question is, how do I know what that enemy should drop?
    // The answer is a drop table, a list of drops associated with an entity.
    // The problem, is how are the entities matched up, i.e how do I know which table I should use for whatever enemy or object?
    // All objects and enemies contain an entityStats.
    // Add an EntityID, enum or number, to cross reference between the two systems.
    // Drop table will match the ID and do the drop rolls for the items

    // "Orc Drop Table = 0" and "Orc ID = 0" -- the two match, so do drop rolls for an orc.

    // However, for runes, drops are relative to difficulty level, and class (where main stats drop according to character class)
    // Therefore, rune drops should be structured a little differently
    // Upon dropping, a preset is dropped, but the amount is generated on the drop, based on a range allowed by the rarity

    // Upon dropping a preset rune is dropped that contains the STATS which should be rolled for
    // Stats are rolled based on the rarity ranges, and a random rune is returned on the roll

    // For item giving, give the preset with specified rolls?

    // Create a RuneSystem
    // This will be responsible for generating Rune stat rolls and handling rarity, stat, and possibly class main stat matching
}
