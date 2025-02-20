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
    [SerializeField] float minSpawnDistance = 2.5f;
    [Tooltip("Maximum distance from the player an Agent can spawn")]
    [SerializeField] float maxSpawnDistance = 5.0f;
    [Tooltip("The pools of enemies that can be spawned")][SerializeField] ObjectPool[] enemyPools;
    [Tooltip("Time between spawning")][Range(0, 3.0f)][SerializeField] float spawnInterval = 1.0f;
    [Range(0, 10.0f)][SerializeField] float groupCheckInterval = 1.0f;
    [Range(0, 3.0f)][SerializeField] float enemyCheckInterval = 1.0f;


    [Header("Update Intervals")]
    public UpdateInterval nearInterval;
    public UpdateInterval regularInterval;
    public UpdateInterval farInterval;
    public int currAmount = 0;

    //float spawnTimer = 0.0f;
    [SerializeField] float respawnInterval = 5.0f;
    [SerializeField] float hordeCheckInterval = 1.25f;

    // Instead of prefab updates, organize enemies into these distance groups which will dictate their frequency
    // Whenever a player enters a check based on the groups the distances
    Dictionary<UpdateInterval, List<EnemyController>> distanceGroups;
    bool distanceGroupUpdated;
    bool enemiesSpawned;
    bool canSpawn;

    // CLEANUP VARIABLES SECTION ASAP
    //List<Horde> hordes;

    // To handle, get a reference to the level details to make everything much more streamlined
    LevelDetails details;

    List<Transform> spawnPoints;    // This will get the points from the level ON LOAD, and cleanup for different levels ON UNLOAD

    public GridData grid;
    #endregion

    #region UNITY FUNCTIONS

    void Awake()
    {
        distanceGroups = new Dictionary<UpdateInterval, List<EnemyController>>();
        spawnPoints = new List<Transform>();
    }
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        InitDistanceGroups();
    }
    #endregion

    #region SCENE LOADS
    // When a level (scene) is loading, call this function
    public void LevelLoading()
    {
        // When loading the level, if the level type is none, don't need to set anything up
        if (LevelManager.Instance.type != LevelManager.LevelType.NONE)
        {
            if (LevelManager.Instance.grid != null)
            {
                grid = LevelManager.Instance.grid;
                if (grid.cells == null)
                {
                    grid.LoadFromList();
                }
            }

            details = LevelManager.Instance.Details;    // Get a reference to the level details
            if (details != null)    // If level details exist
            {
                LoadSpawnPoints();  // Load the spawns

                // Only load enemies if enemies exist and are expected
                if (details.enemiesToSpawn != null && details.enemiesToSpawn.Count > 0)
                {
                    if (enemyPools != null && enemyPools.Length > 0)
                    {
                        //SpawnInitialEnemies();

                        if (spawnEnemies)
                        {
                            canSpawn = true;
                        }
                        SpawnEnemies();                 // Spawn enemies while loading the level
                        StartCoroutine(ManagerLoop());  // Setup the loop to handle the enemies within the level
                    }
                    else
                        Debug.LogError("[AIManager]: Missing Enemy Pools");
                }
            }
        }

    }

    // When a level (scene) is unloading, call this function
    public void LevelUnloading()
    {
        UnloadSpawnPoints();
        foreach (ObjectPool enemyPool in enemyPools)
        {
            enemyPool.Reset();
        }
    }

    public void LoadSpawnPoints()
    {
        spawnPoints = LevelManager.Instance.EnemySpawnPoints;
    }

    public void UnloadSpawnPoints()
    {
        spawnPoints.Clear();
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
    #region Standard Spawning
    // This function will spawn enemies at the desired locations setup in the LEVEL'S spawn list
    public void SpawnEnemies()
    {
        // First step is to check if enemies should be spawned
        Debug.Log("[AIManager]: Should spawn enemies? " + spawnEnemies);
        if (!spawnEnemies)
            return;
        // Next step is to check if enemies CAN spawn
        if (canSpawn)
        {
            if (spawnPoints != null && spawnPoints.Count > 0)
            {
                StartCoroutine(HandlePointsSpawns());
            }
            else
            {
                Debug.LogError("[AIManager]: No enemy spawn point(s) set");
            }
        }
    }
    IEnumerator HandlePointsSpawns()
    {
        // Once the coroutine has begun spawning, set can spawn flag to false
        enemiesSpawned = false;
        canSpawn = false;
        foreach (EntityNumber group in details.enemiesToSpawn)
        {
            GameObject enemy = group.entity;
            foreach (ObjectPool pool in enemyPools)
            {
                if (pool.Prefab == enemy)   // If the enemy matches the pool (key), we want to spawn the number of enemies designated in the group
                {
                    int numSpawns = Random.Range(group.minSpawn, group.maxSpawn);
                    Debug.Log("[AIManager]: Spawning " + numSpawns + " " + enemy.name);
                    // To know how much enemies to spawn at each spawnpoint, take the total number of spawns / number of locations
                    int perPoint = numSpawns / spawnPoints.Count;
                    for (int pointIndex = 0; pointIndex < spawnPoints.Count; pointIndex++)
                    {
                        SpawnEnemyBatch(spawnPoints[pointIndex], perPoint, pool);
                        yield return new WaitForSeconds(spawnInterval);
                    }
                }
            }
        }
        enemiesSpawned = true;  // Once all of the enemies have been spawned, flag as true
        canSpawn = true;
    }

    IEnumerator HandleHordeSpawns(Horde horde)
    {
        foreach (EntityNumber group in horde.enemiesToSpawn)
        {
            Debug.Log("[AIManager]: Horde spawning: " + group.entity + " with " + group.minSpawn);
            GameObject enemy = group.entity;
            foreach (ObjectPool pool in enemyPools)
            {
                if (pool.Prefab == enemy)
                {
                    for (int i = 0; i < group.minSpawn; i++)
                    {
                        GameObject agent = pool.GetPooledObject();
                        if (agent != null)
                        {
                            int randPoint = Random.Range(0, CircleUtility.MAX_CIRCLE_POSITIONS);
                            float spawnDist = Random.Range(minSpawnDistance, maxSpawnDistance);
                            Vector3 spawnPos = player.transform.position;

                            // Build a circle around the spawn point
                            spawnPos.x += (CircleUtility.CircleListInstance[randPoint].x * spawnDist);
                            spawnPos.z += (CircleUtility.CircleListInstance[randPoint].z * spawnDist);
                            spawnPos.y = LevelManager.Instance.FloorOffset;

                            SpawnEnemy(agent, spawnPos);
                            yield return new WaitForSeconds(spawnInterval);
                        }
                    }
                }
            }
        }
    }

    public void SpawnEnemyBatch(Transform origin, int numToSpawn, ObjectPool source)
    {
        // around the origin, where are the eligble spots to spawn?

        for (int i = 0; i < numToSpawn; i++)
        {
            GameObject agent = source.GetPooledObject();
            if (agent != null)
            {
                // Randomize the spawn position
                int randPoint = Random.Range(0, CircleUtility.MAX_CIRCLE_POSITIONS);
                float spawnDist = Random.Range(minSpawnDistance, maxSpawnDistance);
                Vector3 spawnPos = origin.position;

                // Build a circle around the spawn point
                spawnPos.x += (CircleUtility.CircleListInstance[randPoint].x * spawnDist);
                spawnPos.z += (CircleUtility.CircleListInstance[randPoint].z * spawnDist);
                spawnPos.y = LevelManager.Instance.FloorOffset;

                // Cleanup the position passed to the agent (valid grid position)
                CellIndex index = GridUtility.GetIndexFromPoint(grid, spawnPos);
                if (index.x != -1 && index.y != -1) // if the index is valid
                {
                    Cell c = GridUtility.GetCellFromIndex(grid, index);
                    if (c != null)
                    {
                        if (!c.isObstacle && !c.isOccupied) // if the cell is not an obstacle and not occupied. (NOTE: FIGURE OUT WHAT FLAGS AS OCCUPIED --> OBSTACLE SPAWN?)
                        {
                            spawnPos = c.position;  // Spawn the agent at that cell position
                        }
                        else // If it is occupied or an obstacle, spawn the agent as a straggler instead (random position)
                        {
                            index = GridUtility.GetRandomIndex(grid, 3);
                            if (index.x != -1 && index.y != -1) // if the index is valid
                            {
                                c = GridUtility.GetCellFromIndex(grid, index);
                                spawnPos = c.position;
                            }
                            else    // If the index is not valid, just skip the spawn
                            {
                                continue;   // Skip this spawn (won't spawn the enemy, but continue the loop)
                            }
                        }
                        SpawnEnemy(agent, spawnPos);
                    }
                }
            }
        }
    }
    public void SpawnEnemy(GameObject agent, Vector3 pos)
    {
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
            currAmount += 1;
            //controller.stats.OnDied -= context => { };
            //controller.stats.OnDied -= context => { UpdateDeathNumbers(); };
            //controller.stats.OnDied += context => { UpdateDeathNumbers(ref controller); };

            // This position should be verified based on level's grid.

            agent.transform.position = pos;

            // Place the unit into a group based on their position;
            float distance = GetSquarePlayerDistance(pos);
            AssignGameObjectToGroup(agent, distance);
            agent.SetActive(true);
        }
    }
    public void SpawnEnemy(ObjectPool fromPool, Vector3 pos)
    {
        // THIS FUNCTION MAY BE DEPRECATE, DO NOT USE SINCE NO AGENT VALID CHECK
        GameObject agent = fromPool.GetPooledObject();
        SpawnEnemy(agent, pos);
    }

    // Because events with number changing is not very reliable, rely on a function to handle the increase and decrease
    public void UpdateDeathNumbers()
    {
        currAmount--;
        LevelManager.Instance.numKilled++;
        //controller.stats.OnDied -= context => { } ;  // stop listening to the function
    }
    #endregion
    #region Horde Spawning
    public IEnumerator HordeHandler()
    {
        do
        {
            foreach (Horde horde in details.hordes)
            {
                // Check the spawn conditions
                if (!horde.Triggered)
                {
                    //Debug.Log("Checking horde");
                    switch (horde.hordeCondition)
                    {
                        case Horde.SpawnCondition.TIME: //  Check time spent in the level
                            if (horde.CheckShouldSpawn(LevelManager.Instance.TimeSpent))
                            {
                                // once the horde has been spawned, we do not want to check for that horde anymore.
                                StartCoroutine(HandleHordeSpawns(horde));
                                horde.Triggered = true;
                            }
                            // check the time
                            break;
                        case Horde.SpawnCondition.KILLS: // Check number of kills
                            if (horde.CheckShouldSpawn(LevelManager.Instance.numKilled))
                            {
                                // once the horde has been spawned, we do not want to check for that horde anymore.
                                StartCoroutine(HandleHordeSpawns(horde));
                                horde.Triggered = true;
                            }
                            break;
                    }
                }
                // don't want to check every single frame
            }
            yield return new WaitForSeconds(hordeCheckInterval);
        } while (!LevelManager.Instance.LevelComplete);
    }
    #endregion
    #region Distance Groups
    public void InitDistanceGroups()
    {
        distanceGroups.Add(nearInterval, new List<EnemyController>());
        distanceGroups.Add(regularInterval, new List<EnemyController>());
        distanceGroups.Add(farInterval, new List<EnemyController>());
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
    #endregion
    #endregion
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

        StartCoroutine(HordeHandler());

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
            // Only spawn more enemies once the count has reached 0.
            if (canSpawn)
            {
                if (currAmount <= 0)
                {
                    SpawnEnemies();
                }
            }
            // Start Coroutines to run the functions, don't start additional coroutines
            yield return null;
        }
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
}