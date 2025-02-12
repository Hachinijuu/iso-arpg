using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct UpdateInterval
{
    public int range;
    public float time;
}
public class AIManager : MonoBehaviour
{
    #region VARIABLES
    private static AIManager instance = null;
    public static AIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AIManager>();
            if (!instance)
                Debug.LogWarning("[AIManager]: No AI manager found");
            return instance;
        }
    }

    // AI Manager gets the information from the level manager, get a reference to the level manager, can also use the instance
    PlayerController player;
    //LevelManager levelManager;

    [Header("Spawning Settings")]
    [Tooltip("Should enemies spawn?")]
    [SerializeField] bool spawnEnemies;
    [Tooltip("How much overflow a pool can reserve")]
    [SerializeField] float overFlowThreshold = 2.0f;    // if there is this much more, it is considered overflow (multiply, 1 = 1)
    [Tooltip("How much buffer the pools should provide")]
    [SerializeField] float bufferThreshold = 1.5f;      // buffer value (multipled, 1 = 1)
    [Tooltip("Minimum distance from the player an Agent can spawn")]
    [SerializeField] float minSpawnDistance = 2.5f;
    [Tooltip("Maximum distance from the player an Agent can spawn")]
    [SerializeField] float maxSpawnDistance = 5.0f;
    [Tooltip("The pools of enemies that can be spawned")]
    [SerializeField] ObjectPool[] enemyPools;
    [Tooltip("Time between spawning")]
    [Range(0, 3.0f)][SerializeField] float spawnInterval = 1.0f;
    [Range(0, 10.0f)][SerializeField] float groupCheckInterval = 1.0f;
    [Range(0, 3.0f)][SerializeField] float enemyCheckInterval = 1.0f;


    [Header("Update Intervals")]
    public UpdateInterval nearInterval;
    public UpdateInterval regularInterval;
    public UpdateInterval farInterval;

    public int currAmount = 0;

    //float spawnTimer = 0.0f;
    [SerializeField] float respawnInterval = 5.0f;

    // Instead of prefab updates, organize enemies into these distance groups which will dictate their frequency
    // Whenever a player enters a check based on the groups the distances
    Dictionary<UpdateInterval, List<EnemyController>> distanceGroups;
    bool distanceGroupUpdated;
    bool enemiesSpawned;
    private List<Vector3> spawnLocations;
    #endregion
    
    #region UNITY FUNCTIONS

    void Awake()
    {
        distanceGroups = new Dictionary<UpdateInterval, List<EnemyController>>();
    }
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        InitDistanceGroups();
        //LevelLoading();
    }
    #endregion
    
    #region SCENE LOADS
    // When a level (scene) is loading, call this function
    public void LevelLoading()
    {
        // Get the current level
        //levelManager = GameManager.Instance.LevelManager;
        // When a level is loading, get the quantity to spawn correct.
        // Get the number of enemies in the pool * overflowThreshold
        // If it is over the threshold, remove some enemies
        // If it is not, do not adjust the pool

        //BalanceEnemyNumbers();
        //StartCoroutine(SpawnInitialBatch());\

        // Only load enemies if enemies exist and are expected
        if (LevelManager.Instance.Details.enemiesToSpawn != null && LevelManager.Instance.Details.enemiesToSpawn.Count >= 0)
        {
            if (enemyPools != null && enemyPools.Length > 0)
            {
                //SpawnInitialEnemies();
                StartCoroutine(ManagerLoop());
            }
            else
                Debug.LogError("[AIManager]: Missing Enemy Pools");
        }
    }

    private void BalanceEnemyNumbers()
    {
        // Cycle through the enemies the level requests.
        foreach (EntityNumber enemyGroup in LevelManager.Instance.Details.enemiesToSpawn)
        {
            GameObject enemy = enemyGroup.entity;

            foreach (ObjectPool pool in enemyPools)
            {
                // Check it the pool prefab matches the enemy to spawn
                if (pool.Prefab == enemy)
                {
                    // Turn the pool on, and use it
                    pool.gameObject.SetActive(true);
                    // Check the number of enemies in the pool, if the amount of enemies in the pool is more than needed for the level, shrink the pool
                    if ((pool.pooledObjects.Count * overFlowThreshold) > (enemyGroup.maxSpawn * bufferThreshold))
                    {
                        int toRemove = Mathf.RoundToInt((pool.pooledObjects.Count * overFlowThreshold) - enemyGroup.maxSpawn);
                        pool.ShrinkPool(toRemove);
                    }
                    // If there are not enough enemies, add to the pool
                    else if (pool.pooledObjects.Count < enemyGroup.maxSpawn)
                    {
                        int toAdd = Mathf.RoundToInt((enemyGroup.maxSpawn - pool.pooledObjects.Count) * bufferThreshold);
                        pool.GrowPool(toAdd);
                    }
                    // Otherwise, amount of enemies is sufficient for gameplay
                }
            }
        }
    }

    // When a level (scene) is unloading, call this function
    public void LevelUnloading()
    {
        foreach (ObjectPool enemyPool in enemyPools)
        {
            enemyPool.Reset();
        }
    }
    #endregion
    
    #region HELPER FUNCTIONS
    float GetSquareDistance(Vector3 start, Vector3 end)
    {
        return (start - end).sqrMagnitude;
    }

    float GetSquarePlayerDistance(Vector3 pos)
    {
        return GetSquareDistance(pos, player.transform.position);
    }
    #endregion

    #region ENEMY SPAWNING

    // THIS FUNCTION WILL HANDLE ENEMY SPAWNING THROUGHOUT THE DURATION OF THE LEVEL
    IEnumerator SpawnEnemies()
    {
        enemiesSpawned = false;
        int numExpected = 0;
        int numTypes = 0;       // The types of enemies (orcs, imps)
        foreach (EntityNumber group in LevelManager.Instance.Details.enemiesToSpawn)
        {
            numTypes++;
            numExpected += Random.Range(group.minSpawn, group.maxSpawn);
        }
        // To know how much we should spawn, find the difference between the current enemies and the number expected
        // Expecting 100, have 30, 100 - 30 = 70, need 70 enemies
        int toSpawn = numExpected - currAmount;
        if (toSpawn > 0)
        {
            Debug.LogWarning("[AIManager]: Spawning " + toSpawn + ", Expected Number: " + numExpected + ", Current Count: " + currAmount);
            foreach (EntityNumber group in LevelManager.Instance.Details.enemiesToSpawn)
            {
                GameObject enemyToSpawn = group.entity;
                foreach (ObjectPool pool in enemyPools)
                {   
                    if (pool.Prefab == enemyToSpawn)
                    {
                        int enemiesToSpawn = toSpawn / numTypes;
                        for (int i = 0; i < enemiesToSpawn; i++)
                        {
                            CellIndex index = LevelManager.Instance.GetRandomIndex(1);

                            if (index.x != -1 && index.y != -1)
                            {
                                Cell currCell = LevelManager.Instance.GetCellFromIndex(index);
                                Vector3 spawnPos = currCell.position;
                                spawnPos.y += LevelManager.Instance.FloorOffset;
                                //GameObject enemy = pool.GetPooledObject();
                                SpawnEnemy(pool, spawnPos);
                            }
                            yield return new WaitForSeconds(spawnInterval);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("[AIManager: No enemies should spawn");
        }
        Debug.LogWarning("[AIManager]: Completed spawn cycle");
        enemiesSpawned = true;
    }
    
    // MOBS WILL BE TRIGGERED WHEN THERE ARE X NUMBER OF ENEMIES KILLED, OR TIME IN THE LEVEL HAS PASSED A CERTAIN THRESHOLD
    // OTHER ENEMIES WILL NOT SPAWN UNTIL THE MOB HAS BEEN DEFEATED(?)
    IEnumerator SpawnMob()
    {
        // if (player != null)
        // {
        //     Vector3 offsetPos = player.transform.position + (player.transform.forward * spawnDistance);
        //     Vector3 spawnPos = player.transform.position;
        //     spawnPos.x += (CircleUtility.CircleListInstance[rotIndex].x * spawnDistance);
        //     spawnPos.z += (CircleUtility.CircleListInstance[rotIndex].z * spawnDistance);
        //     spawnPos.y = LevelManager.Instance.FloorOffset;
        //     // Get a cell position to place the agent at
        //     CellIndex index = LevelManager.Instance.GetIndexFromPoint(spawnPos);
        //     if (index.x != -1 && index.y != -1)
        //     {
        //         // If it is a valid cell
        //         Cell currCell = LevelManager.Instance.GetCellFromIndex(index);
        //         spawnPos = currCell.position;
        //     }
        //     spawnLocations.Add(spawnPos);
        //     // Get the grid spawn pos before doing this
        //     //Debug.Log("rotIndex: " + rotIndex + ", Position: " + spawnPos);
        //     GameObject agent = pool.GetPooledObject();
        //     SpawnEnemy(pool, spawnPos);
        // }
        yield return new WaitForSeconds(spawnInterval);
    }

    public void SpawnEnemy(ObjectPool fromPool, Vector3 pos)
    {
        GameObject agent = fromPool.GetPooledObject();
        if (agent != null)
        {
            EnemyController controller = agent.GetComponent<EnemyController>();
            if (controller)
            {
                controller.Respawn();
            }

            // Add a listener to the enemies
            // This will work because the number of enemies spawned in the initial batch SHOULD NEVER exceed subsequent spawns.
            // This needs to be tested


            // Negative current amount still exists (more enemies than accounted for that die)
            currAmount++;
            //controller.stats.OnDied -= context => { };
            controller.stats.OnDied -= context => { currAmount--; LevelManager.Instance.numKilled++; };
            controller.stats.OnDied += context => { currAmount--; LevelManager.Instance.numKilled++; };

            agent.transform.position = pos;

            // Place the unit into a group based on their position;
            float distance = GetSquarePlayerDistance(pos);
            AssignGameObjectToGroup(agent, distance);
            agent.SetActive(true);
        }
    }

    // Regular spawnning throughout the level as the enemies die

    public void InitDistanceGroups()
    {
        distanceGroups.Add(nearInterval, new List<EnemyController>());
        distanceGroups.Add(regularInterval, new List<EnemyController>());
        distanceGroups.Add(farInterval, new List<EnemyController>());
        //StartCoroutine(AssignToGroups());

        //foreach (KeyValuePair<UpdateInterval, List<EnemyController>> pair in distanceGroups)
        //{
        //    Debug.Log("[AIManager]: " + pair.Value.Count + " Enemies update at an interval of: " + pair.Key.time);
        //}
    }

    public void AssignGameObjectToGroup(GameObject go, float distance)
    {
        EnemyController controller = go.GetComponent<EnemyController>();

        AssignControllerToGroup(controller, distance);
    }

    public void AssignControllerToGroup(EnemyController controller, float distance)
    {
        if (distance > farInterval.range * farInterval.range)       // if the agent is far from the player
        {
            controller.UpdateInterval = farInterval.range;
            controller.PhysicsInterval = farInterval.range;
            distanceGroups[farInterval].Add(controller);
        }
        else if (distance < nearInterval.range * nearInterval.range) // if the agent is close to the player
        {
            controller.UpdateInterval = nearInterval.time;
            controller.PhysicsInterval = nearInterval.time;
            distanceGroups[nearInterval].Add(controller);
        }
        else    // the agent is not too far, and not too close
        {
            controller.UpdateInterval = regularInterval.time;
            controller.PhysicsInterval = regularInterval.time;
            distanceGroups[regularInterval].Add(controller);
        }
    }
    #region RUNTIME FUNCTIONS
    IEnumerator UpdateDistanceGroups()
    {
        distanceGroupUpdated = false;
        //Debug.Log("[AIManager]: Started to update Distance Groups");
        // Do a staggered update of the distance groups, checking if the enemies should shift to a different one.
        foreach (KeyValuePair<UpdateInterval, List<EnemyController>> group in distanceGroups)
        {
            // Check each enemy in the current distance group and evaluate if they should be moved
            // Use a for loop since the size is being modified

            // This is inefficient when it comes to large sizes,
            // Cut this value into batches to make it more manageable

            // If I have a group of 600, I am possibly doing 600 reassignments
            for (int i = 0; i < group.Value.Count; i++)
            {
                // Get the distance between the agent and the player
                EnemyController enemy = group.Value[i];
                float dist = GetSquarePlayerDistance(enemy.transform.position);

                // If the distance is outside of the specified range of the CURRENT GROUP the agent is a member of
                if (dist < group.Key.range || dist > group.Key.range)
                {
                    // Remove the agent from the group
                    group.Value.Remove(enemy);
                    // Reassign the agent to another group
                    AssignControllerToGroup(enemy, dist);
                }

                // Wait some time between each enemy update
                yield return new WaitForSeconds(enemyCheckInterval);
            }
            yield return new WaitForSeconds(nearInterval.time); // When cycling through the intervals, wait for some time before checking the next set
        }
        distanceGroupUpdated = true;
    }


    // Commands will be sent on intervals based on the distance from the player
    // See Level Manager Cell/Grid System for spacing and boundaries
    IEnumerator ManagerLoop()
    {
        // While the level has not ended, run updates for the distance groups and spawn enemies
        // Update Coroutine
        distanceGroupUpdated = true;
        enemiesSpawned = true;
        float groupTimer = 0.0f;
        float spawnTimer = 0.0f;

        while (!LevelManager.Instance.LevelComplete)
        {
            groupTimer += Time.deltaTime;
            spawnTimer += Time.deltaTime;

            if (groupTimer > groupCheckInterval)
            {
                if (distanceGroupUpdated)
                {
                    StartCoroutine(UpdateDistanceGroups());
                    groupTimer = 0.0f;
                }
            }
            if (spawnTimer > respawnInterval)
            {
                if (enemiesSpawned)
                {
                    Debug.LogWarning("[AIManager]: Spawning start");
                    StartCoroutine(SpawnEnemies());
                    spawnTimer = 0.0f;
                }
            }
            // Start Coroutines to run the functions, don't start additional coroutines
            yield return null;
        }
        yield return null;
    }
    #endregion

    // Modify FSM to include update frequency variable
    // This will determine how often the FSM updates

    // Need flocking behaviours in chase state to stay next to the leader of the unit group
    // This will keep the update and behaviour of the unit's relatively similar

    // When dying, make use of events for transitions, because otherwise the FSM will rely on update variables
    // This reliance will cause enemies not to die even if they have ran out of health.

    // Have an event for the interval changed event, this will change all of the intervals of the FSM's IN THE BUNDLE
    // Upon spawning, the update interval will be set based on the distance
    // Every now and then update the interval values
    // Or whenever the player enters the player ranges, update their intervals


    // Test to do
    // Group the enemies into mini groups
    // Update these mini groups if the they are active

    // There is a reference to the object pool
    // The pool should match the quantity of enemies expected in the level
    // This pool will be batched into smaller groups
    // In each of the smaller groups
    // Call the FSM updates based on the intervals

    // i.e, instead of each FSM having the overhead of calling their own update
    // The AI manager will tell the FSM to update, given the interval and time passed

    #endregion

    #region DEBUG

    Vector3 cubeSize = new Vector3(0.25f, 0.25f, 0.25f);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (spawnLocations != null && spawnLocations.Count > 0)
        {
            foreach (Vector3 pos in spawnLocations)
            {
                Gizmos.DrawCube(pos, cubeSize);
            }
        }
    }
    #endregion
}