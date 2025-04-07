using System.Collections;
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

    [Header("Popup Settings")]
    [SerializeField] GameObject popupPanel;
    [SerializeField] GameObject popupPrefab;
    [SerializeField] float destroyTime = 0.25f;


    private List<EntityStats> enemies;
    private List<EntityStats> destructibles;
    private List<EntityStats> chests;

    [SerializeField] List<DropTable> dropTables;
    //[SerializeField] ItemData expOrb;
    #endregion
    #region INITIALIZATION
    public void InitDropLists()
    {
        enemies = new List<EntityStats>();
        destructibles = new List<EntityStats>();
        chests = new List<EntityStats>();
        if (dropTables == null)
            dropTables = new List<DropTable>();
    }
    public float GetShortestDestructibleDistance(Transform pos)
    {
        float shortest = float.MaxValue;
        foreach (EntityStats agent in destructibles)
        {
            if (agent == null) continue;
            float distance = Vector3.Distance(agent.transform.position, pos.position);
            {
                if (distance < shortest)
                    shortest = distance;
            }
        }
        return shortest;
    }

    public GameObject[] destructionParticles;

    // When enemies set themselves to active, register them to the list
    public void RegisterEnemyDrop(EntityStats enemy)
    {
        enemies.Add(enemy);
        enemy.OnDied += HandleEnemyDrops;

        //context => { CheckDrop(enemy); };
    }

    public void UnregisterEnemyDrop(EntityStats enemy)
    {
        //Debug.Log("Unregistered, stopped listening");
        enemies.Remove(enemy);

        enemy.OnDied -= HandleEnemyDrops; //context => { CheckDrop(enemy); };
    }
    void HandleEnemyDrops(GameObject go)
    {
        EntityStats stats = go.GetComponent<EntityStats>(); // This function is required to have proper unassignment of the event.
        CheckDrop(stats);

        // With lambda functionality for context passing to events, it needs to be the same function, the lambda -= context => { CheckDrop() } is not the same
        // Therefore have a function and handle straight across without context passing
    }

    // When destructibles set themselves to inactive, unregister them from the list
    public void RegisterDestructibleDrop(EntityStats destructible)
    {
        destructibles.Add(destructible);
        destructible.OnDied += HandleDestructibleDrop;
    }

    public void UnregisterDestructibleDrop(EntityStats destructible)
    {
        destructibles.Remove(destructible);
        destructible.OnDied -= HandleDestructibleDrop;
    }

    public void HandleDestructibleDrop(GameObject go)
    {
        EntityStats stats = go.GetComponent<EntityStats>();
        CheckDrop(stats);
        SpawnParticle(go);
        HandleDestruction(go);
    }
    public void HandleDestruction(GameObject source)
    {
        source.SetActive(false);    // cleanup
        // play a destruction sound
    }

    public void RegisterChestDrops(EntityStats chest)
    {
        chests.Add(chest);
        chest.OnDied += HandleChestDrop;
    }
    public void UnregisterChestDrops(EntityStats chest)
    {
        chests.Remove(chest);
        chest.OnDied -= HandleChestDrop;
    }

    public void HandleChestDrop(GameObject go)
    {
        EntityStats stats = go.GetComponent<EntityStats>();
        CheckDrop(stats, true);
        //FadeObject(go);
    }

    public void FadeObject(GameObject source)
    {
        Renderer r = source.GetComponent<Renderer>();
        StartCoroutine(HandleFade(r));
    }

    public float objectFadeRate = 0.25f;
    IEnumerator HandleFade(Renderer render)
    {
        Color c = render.material.color;
        while (render.material.color.a > 0)
        {
            c.a -= objectFadeRate * Time.deltaTime;
            yield return null;
        }
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
        foreach (DropTableModifier tableMod in dropTable.items)
        {
            float dropValue = Random.Range(0, 100f);

            // If I get 80 and I have a 20% chance to drop an item
            // I do not get the item, because I rolled an 80.
            // I have an 80% chance NOT to get the item 20-100
            if (dropValue <= (tableMod.item.dropChance + tableMod.dropModifier))
            {
                ItemData item = tableMod.item;
                if (item == null) { return; }
                CreatedDroppedObject(whoDied.transform.position, item); // This will create the rune item with the modded data?
                Debug.Log("[DropSystem]: Dropped an item");
                return;
            }
        }

        if (dropTable.runesTable == null) { return; }
        foreach (DropTableModifier runeModifer in dropTable.runesTable.runes)
        {
            float dropValue = Random.Range(0, 100f);

            // If I get 80 and I have a 20% chance to drop an item
            // I do not get the item, because I rolled an 80.
            // I have an 80% chance NOT to get the item 20-100
            if (dropValue <= (runeModifer.item.dropChance + runeModifer.dropModifier))
            {
                RuneData item = runeModifer.item as RuneData;
                if (item == null) { return; }
                if (item is RuneData rune && item.type == ItemTypes.RUNE) // If the item that dropped is a rune, do a roll in the rune system to generate the stats
                {
                    // Do another roll for the rune rarity, this is relative to the difficulty
                    item = RuneSystem.Instance.RollRune(rune);
                }
                CreatedDroppedObject(whoDied.transform.position, item); // This will create the rune item with the modded data?
                Debug.Log("[DropSystem]: Dropped an item");
                return;
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

    private void CheckDrop(EntityStats whoDied, bool repeat) // check the thing and map it to what it is supposed to drop
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
        foreach (DropTableModifier tableMod in dropTable.items)
        {
            float dropValue = Random.Range(0, 100f);

            // If I get 80 and I have a 20% chance to drop an item
            // I do not get the item, because I rolled an 80.
            // I have an 80% chance NOT to get the item 20-100
            if (dropValue <= tableMod.item.dropChance + tableMod.dropModifier)
            {
                ItemData item = tableMod.item;
                if (item == null) { return; }
                CreatedDroppedObject(whoDied.transform.position, item); // This will create the rune item with the modded data?
                Debug.Log("[DropSystem]: Dropped an item");
                if (repeat) { continue; } else { return; }
            }
        }

        if (dropTable.runesTable == null) { return; }
        foreach (DropTableModifier runeModifer in dropTable.runesTable.runes)
        {
            float dropValue = Random.Range(0, 100f);

            // If I get 80 and I have a 20% chance to drop an item
            // I do not get the item, because I rolled an 80.
            // I have an 80% chance NOT to get the item 20-100
            if (dropValue <= (runeModifer.item.dropChance + runeModifer.dropModifier))
            {
                RuneData item = runeModifer.item as RuneData;
                if (item == null) { return; }
                if (item is RuneData rune && item.type == ItemTypes.RUNE) // If the item that dropped is a rune, do a roll in the rune system to generate the stats
                {
                    // Do another roll for the rune rarity, this is relative to the difficulty
                    item = RuneSystem.Instance.RollRune(rune);
                }
                CreatedDroppedObject(whoDied.transform.position, item); // This will create the rune item with the modded data?
                Debug.Log("[DropSystem]: Dropped an item");
                if (repeat) { continue; } else { return; }
            }
        }
    }

    // Create dropped object
    public void CreatedDroppedObject(Vector3 pos, ItemData item)
    {
        // Create a copy of the item, this will have modified data

        ItemData droppedItem = Instantiate(item);
        //foreach (Ability ab in playerClass.Abilities)
        //{
        //    Abilities.Add(Instantiate(ab));
        //}
        //identity = Instantiate(playerClass.IdentityAbility);    // Create a 'new' identity based on what the class has for game use


        GameObject go = GameObject.Instantiate(droppedItem.prefab);
        // This creates the object, but where does it create the object...
        // Create it at the parent of whoever called this
        if (droppedItem is RuneData)
        {
            MeshRenderer render = go.GetComponent<MeshRenderer>();
            if (render != null)
            {
                render.material = RuneSystem.Instance.SetRuneMaterial(droppedItem.rarity);
            }
        }

        // Call specific functions based on the type it is, pickup vs interactable
        // Let the created object know what kind of item it is
        Item temp = go.GetComponent<Item>();
        if (temp)
        {
            temp.LoadItemData(droppedItem);
            pos.y += temp.dropYOffset;
            go.transform.position = pos;

            // Add a listener to the drop if it should provide information when it is picked up.
            if (droppedItem.showPopup)
            {
                droppedItem.OnItemAcquired += ItemPopup; // Once the item has popped up, need to stop listening to the event
            }
        }
        else
        {
            Debug.Log("[DropSystem]: Created " + droppedItem.itemName + " but object does not have Item Component");
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
        HandlePopup(item);

        //Debug.Log("[DropSystem]: Show popup for: " + item.itemName);
        item.OnItemAcquired -= ItemPopup;
    }
    // This drop system manages all drops in the game
    // It will contain a dictionary to the possible drops and stuff
    #endregion

    public void HandlePopup(ItemData data)
    {
        // Five popups appear when potions are capped out at 3...
        GameObject copy = GameObject.Instantiate(popupPrefab, popupPanel.transform);
        PickupPopup popup = copy.GetComponent<PickupPopup>();
        if (popup != null)
        {
            popup.SetupPopup(data);
        }

        // Set the created object to destroy after time to clear up the popup list
        // Alternatively, maintain count of list and destroy once full
        Destroy(copy, destroyTime); // can fade out instead of straight up destroy
        //Debug.LogWarning("Popup Setup: " + data.name);
    }

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
