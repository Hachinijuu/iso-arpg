using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpdateInterval
{
    public int range;
    public float time;

    public UpdateInterval()
    {
        range = -1;
        time = -1;
    }
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
    Cell playerCell;
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
    [SerializeField] float respawnInterval = 5.0f;
    [SerializeField] float hordeCheckInterval = 1.25f;

    // Instead of prefab updates, organize enemies into these distance groups which will dictate their frequency
    // Whenever a player enters a check based on the groups the distances
    Dictionary<UpdateInterval, List<EnemyControllerV2>> distanceGroups;
    bool distanceGroupUpdated;
    bool enemiesSpawned;
    bool canSpawn;

    
    public int numElites = 0;
    public int numEnemies = 0;
    // CLEANUP VARIABLES SECTION ASAP
    //List<Horde> hordes;

    // To handle, get a reference to the level details to make everything much more streamlined
    LevelDetails details;

    List<EnemySpawnPoint> spawnPoints;    // This will get the points from the level ON LOAD, and cleanup for different levels ON UNLOAD
    List<EnemySpawnPoint> eliteSpawnPoints;    // This will get the points from the level ON LOAD, and cleanup for different levels ON UNLOAD


    List<Vector3> spawnDots;
    public List<EnemyControllerV2> aliveEnemies;

    //[SerializeField] GridData gData;
    public Grid grid;
    public Flowfield flowfield; // flowfield for navigation

    // Difficulty
    float difficultyModifer;

    // Reorganize / cleanup this class once all functionality is at expected level
    // It is VERY messy.
    public static float CHASE_RANGE = 20.0f;
    public static float MELEE_RANGE = 5.5f;
    public static float RANGED_RANGE = 12.5f;
    public static float SPECIAL_RANGE = 100.0f;


    public List<StateContainer> enemyStates; 
    AgentStateArgs agentArgs;
    #endregion

    #region UNITY FUNCTIONS

    void Awake()
    {
        distanceGroups = new Dictionary<UpdateInterval, List<EnemyControllerV2>>();
        aliveEnemies = new List<EnemyControllerV2>();
        movingEnemies = new List<EnemyControllerV2>();
        spawnPoints = new List<EnemySpawnPoint>();
        eliteSpawnPoints = new List<EnemySpawnPoint>();
        spawnDots = new List<Vector3>();

        if (enemyStates != null)
        {
            if (enemyStates.Count <= 0) { return; }
            foreach (StateContainer enemyState in enemyStates)
            {
                enemyState.LoadStates();
                Debug.Log("[AIManager: Loaded ID " + enemyState.entityId + " states");
            }
        }
        else
        {
            Debug.LogError("[AIManager] Enemies do not have any states to load from");
        }
    }
    void Start()
    {
        // if (player == null)
        //     player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        InitDistanceGroups();
        GameManager.Instance.onDifficultyChanged += context => { UpdateDifficulty(); }; // Listen to the difficulty changed event, whenever it is changed, modify the local variables
        PlayerManager.Instance.onPlayerChanged += context => { GetPlayer(); };          // Listen to the player changed event, whenever the player is changed, modify the reference
        agentArgs = new AgentStateArgs();
    }
    #endregion
    #region SCENE LOADS
    // When a level (scene) is loading, call this function
    public void LevelLoading()
    {
        // When loading the level, if the level type is none, don't need to set anything up
        StopAllCoroutines();
        if (LevelManager.Instance.type != LevelManager.LevelType.NONE)
        {
            if (LevelManager.Instance.grid != null)
            {
                grid = LevelManager.Instance.grid;
                playerCell = GetPlayerCell();

                // Setup the flowfield
                if (flowfield == null)
                {
                    flowfield = new Flowfield(grid, playerCell);  // Get the player's position
                    Debug.Log("Generated a flow field");
                    // While playing, when should the manager get the player cell?
                    // For responsive gameplay, get the cell often
                    // For event based listening, get it whenever the player moves (if speed is greater than 0)
                    // Or, just get it in update with no step for finding it, just store it for this class to reference
                }
                // if (grid.cells == null)
                // {
                //     grid.LoadFromList();
                // }
            }

            details = LevelManager.Instance.Details;    // Get a reference to the level details
            if (details != null)    // If level details exist
            {
                LoadSpawnPoints();  // Load the spawns

                // Only load enemies if enemies exist and are expected
                if (details.enemiesToSpawn != null)
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
        flowfield = null;
        foreach (ObjectPool enemyPool in enemyPools)
        {
            enemyPool.Reset();
        }

        // Cleanup the lists
        spawnPoints.Clear();
        aliveEnemies.Clear();
        movingEnemies.Clear();
        ResetDistanceGroups();
    }

    public void LoadSpawnPoints()
    {
        if (spawnPoints != null) { spawnPoints = LevelManager.Instance.EnemySpawnPoints; }
        if (eliteSpawnPoints != null) { eliteSpawnPoints = LevelManager.Instance.EliteSpawnPoints; }
        
        
    }

    public void UnloadSpawnPoints()
    {
        if (spawnPoints != null) { spawnPoints.Clear(); }
        if (eliteSpawnPoints != null) { eliteSpawnPoints.Clear(); }
        //spawnPoints.Clear();
        //eliteSpawnPoints.Clear();
    }
    #endregion
    #region HELPER FUNCTIONS
    public void UpdateDifficulty()
    {
        difficultyModifer = GameManager.Instance.currDifficulty.enemyMultipler;
    }
    float GetSquareDistance(Vector3 start, Vector3 end)
    {
        return (start - end).sqrMagnitude;
    }

    float GetSquarePlayerDistance(Vector3 pos)
    {
        return GetSquareDistance(pos, player.transform.position);
    }

    private void GetPlayer()
    {
        // If a player exists, clean up old references before assigning listeners to the new one
        if (player != null)
        {
            player.Movement.onMove -= context => { UpdateField(); };
        }

        // Overwrite / assign the new player value
        player = PlayerManager.Instance.currentPlayer;
        if (player != null)
        {
            // Once the player is found, assign the listener
            player.Movement.onMove += context => { UpdateField(); };
        }
        agentArgs.player = player;    // Statically assign the player transform so ALL agents have the same target transform
        Debug.Log("[AIManager]: Player has been assigned to State Arguments ( " + agentArgs.player + " )");
        UpdateField();
    }

    public Cell GetPlayerCell()
    {
        Cell cell = null;
        if (player != null)
        {
            cell = GridUtility.GetCellFromPoint(grid, player.transform.position);
        }
        return cell;

        // Transform playerPos = PlayerManager.Instance.currentPlayer.transform;
        // if (playerPos != null)
        // {
        //     playerCell = GridUtility.GetCellFromPoint(grid, playerPos.position);
        // }
        // return playerCell;
    }

    bool fieldUpdating = false;
    bool isAlert = false;
    // Can have a list of alert enemies(?)
    public void UpdateField()
    {
        // FLOW FIELDS ONLY NEED TO BE UPDATE / USED WHENEVER THE AI ARE IN ALERT MODE (THIS IS WHEN ALL AI HAVE A REFERNCE TO THE PLAYER'S POSITION)
        if (!isAlert) { return; }
        if (flowfield == null) { return; }
        // Check if the current player's position is distanced from stored cell
        if (playerCell != null)
        {
            float dist = GetSquareDistance(player.transform.position, playerCell.position);
            if (dist < Mathf.Pow(grid.cellSize, 2)) // If the distance is less than the cell's size, early return
            {
                return;
            }
            // else // Distance is greater (player has moved enough)
            // {

            //     // If it is greater than the distance, flag that the field should be updated
            //     //fieldUpdated = false;
            // }
        }

        // Player cell exists and the player has moved far enough from the previous cell
        // Need to update the field
        if (!fieldUpdating)  // If the field is not updating
        {
            StartCoroutine(UpdateFlowfield());
        }
        //else
        //{
        //    // Player cell is null
        //    playerCell = GetPlayerCell();
        //}
        // if (!fieldUpdated)
        // {
        //     IEnumerator FlowfieldUpdate = UpdateFlowfield();
        //     StopCoroutine(FlowfieldUpdate);
        //     StartCoroutine(FlowfieldUpdate);
        //     // Otherwise, if the distance is greater, we can assume the player has travelled across cells, therefore calculate a new field
        //     // If it is, calculate a new field
        // }
    }

    IEnumerator UpdateFlowfield()
    {
        fieldUpdating = true;    // In the process of updating the field
        Debug.Log("[AIManager_Flowfield]: Started updating flow field");
        Cell temp = GetPlayerCell();
        //playerCell = GetPlayerCell();
        // Update the flow field
        if (temp == playerCell) // If the found cell is the same as the current cell
        {
            yield break;
        }
        else
        {
            playerCell = temp;
            // Once the player cell is set, update the agent args
            agentArgs.cellTarget = playerCell;
        }

        if (playerCell != null)
        {
            // Need to reinitalize the field to reset the values back to their expected numbers -- otherwise, tiles marked 0 will persist as 0
            //flowfield = new Flowfield(grid, playerCell);
            flowfield.InitFlowfield();                      // Reset the cells
            flowfield.CreateIntergrationField(playerCell);  // Get the costs
            flowfield.CreateFlowField();                    // Get the directions
            fieldUpdating = false;
        }
        else
            Debug.LogWarning("[AIManager_Flowfield]: Player Cell is NULL");
        yield return fieldUpdating = false;;
        Debug.Log("[AIManager_Flowfield]: Finished updating flow field");
    }
    
    #endregion
    #region ENEMY SPAWNING
    public void RegisterToList(EnemyControllerV2 agent)
    {
        // if (aliveEnemies == null)
        //     aliveEnemies = new List<EnemyControllerV2>();
        
        if (aliveEnemies != null)
            aliveEnemies.Add(agent);
    }

    public void UnregisterFromList(EnemyControllerV2 agent)
    {
        // if (aliveEnemies == null)
        //     aliveEnemies = new List<EnemyControllerV2>();
        
        if (aliveEnemies == null || aliveEnemies.Count == 0) { return; }
        aliveEnemies.Remove(agent);
    }

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
                    EnemyControllerV2 controller = pool.Prefab.GetComponent<EnemyControllerV2>();
                    if (controller != null)
                    {
                        if (controller.stats.id == EntityID.BIG_ELITE)
                        {
                            // If it is an elite enemy
                            int perPoint = numSpawns / eliteSpawnPoints.Count;
                            for (int pointIndex = 0; pointIndex < eliteSpawnPoints.Count; pointIndex++)
                            {
                                SpawnEnemyBatch(eliteSpawnPoints[pointIndex], perPoint, pool);
                                yield return new WaitForSeconds(spawnInterval);
                            }
                        }
                        else
                        {
                            int perPoint = numSpawns / spawnPoints.Count;
                            for (int pointIndex = 0; pointIndex < spawnPoints.Count; pointIndex++)
                            {
                                SpawnEnemyBatch(spawnPoints[pointIndex], perPoint, pool);
                                yield return new WaitForSeconds(spawnInterval);
                            }
                        }
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

    public void SpawnEnemyBatch(EnemySpawnPoint origin, int numToSpawn, ObjectPool source)
    {
        // around the origin, where are the eligble spots to spawn?

        for (int i = 0; i < numToSpawn; i++)
        {
            GameObject agent = source.GetPooledObject();
            if (agent != null)
            {
                // Randomize the spawn position
                int randPoint = Random.Range(0, CircleUtility.MAX_CIRCLE_POSITIONS);
                float spawnDist = Random.Range(origin.minSpawnDistance, origin.maxSpawnDistance);
                Vector3 spawnPos = origin.transform.position;

                // Build a circle around the spawn point
                spawnPos.x += (CircleUtility.CircleListInstance[randPoint].x * spawnDist);
                spawnPos.z += (CircleUtility.CircleListInstance[randPoint].z * spawnDist);
                spawnPos.y = LevelManager.Instance.FloorOffset;

                // Cleanup the position passed to the agent (valid grid position)
                Cell c = GridUtility.GetCellFromPoint(grid, spawnPos);
                if (c != null)
                {
                    // If a cell was found at that position
                    // Spawn at the cell
                    if (!c.isObstacle && !c.isOccupied)
                    {
                        spawnPos = c.position;
                    }
                    else
                    {
                        c = GridUtility.GetRandomCell(grid, 3);
                        if (c != null)
                        {
                            spawnPos = c.position;
                        }
                        else
                        {
                            continue;   // Skip the spawn and continue the loop
                        }
                    }
                    float eliteRoll = Random.Range(0, 100);
                    if (eliteRoll > 95)
                    {
                        SpawnElite(agent, spawnPos);    // Baby elite, simply has higher stats
                    }
                    else
                    {
                        SpawnEnemy(agent, spawnPos);
                    }
                    spawnDots.Add(spawnPos);
                    // Otherwise spawn at a random point
                }
            }
        }
    }
    public void SpawnEnemy(GameObject agent, Vector3 pos)
    {
        if (agent != null)
        {
            // EnemyController controller = agent.GetComponent<EnemyController>();
            // if (controller)
            // {
            //     // Get the modified values passed to the controller
            //     EntityStats stats = controller.GetComponent<EntityStats>();

            //     // When spawning an enemy, they have a chance to be an 'elite' enemy
            //     // This gives them additional stats, and a visual distinction from regular enemies

            //     if (stats != null)
            //     {
            //         // How do we get base, relative to adjusted stats?
            //         // i.e, what is the default?
            //         stats.Health.MaxValue = stats.data.Health.MaxValue * GameManager.Instance.currDifficulty.healthMultiplier;
            //         stats.Damage.Value = stats.data.Damage.Value * GameManager.Instance.currDifficulty.damageMultiplier;
            //     }
            //     //controller.damage *= GameManager.Instance.currDifficulty.damageMultiplier;

            //     controller.Respawn();
            // }

            EnemyControllerV2 controller = agent.GetComponent<EnemyControllerV2>();
            if (controller)
            {
                if (controller.States == null)
                {
                    foreach (StateContainer enemyState in enemyStates)
                    {
                        if (enemyState.entityId == controller.stats.id)
                        {
                            controller.States = enemyState;
                            break;
                        }
                    }
                }

                // Assign the agent to a distance group
                // if (player != null)
                // {
                //     AssignControllerToGroup(controller, player.transform);
                // }
                // else
                // {
                //     AssignControllerToGroup(controller, regularInterval);
                // }

                EntityStats stats = agent.GetComponent<EntityStats>();
                if (stats != null)
                {
                    stats.Health.MaxValue = stats.data.Health.MaxValue * GameManager.Instance.currDifficulty.healthMultiplier;
                    stats.Damage.Value = stats.data.Damage.Value * GameManager.Instance.currDifficulty.damageMultiplier;
                    //aliveEnemies.Add(stats);
                    controller.Hitbox.SetDamage(stats.Damage.Value);    // Give the agent hitbox the new value of damage
                }

                controller.Respawn();
            }

            numEnemies += 1;

            // This position should be verified based on level's grid.

            agent.transform.position = pos;

            // Place the unit into a group based on their position;
            //float distance = GetSquarePlayerDistance(pos);
            //AssignGameObjectToGroup(agent, distance);
            agent.SetActive(true);
        }
    }


    public float eliteMultipler = 1.5f;
    public void SpawnElite(GameObject agent, Vector3 pos)
    { 
        // This spawns an elite enemy
        // Select 2 stats randomly from the stat list
        // Scale / multiply those stats for elite values

        if (agent != null)
        {
            EntityStats stats = agent.GetComponent<EntityStats>();
            if (stats != null)
            {
                int statIndex1 = Random.Range(0, stats.statList.Count);
                int statIndex2 = Random.Range(0, stats.statList.Count);

                while (statIndex1 == statIndex2) // If the stats are the same, roll for different number
                {
                    statIndex2 = Random.Range(0, stats.statList.Count);
                }
                if (stats.statList[statIndex1] is TrackedStat ts)
                {
                    ts.MaxValue = ts.MaxValue * eliteMultipler;
                }
                else
                {
                    stats.statList[statIndex1].Value *= eliteMultipler;
                }
                if (stats.statList[statIndex1] is TrackedStat ts2)
                {
                    ts2.MaxValue = ts2.MaxValue * eliteMultipler;
                }
                else
                {
                    stats.statList[statIndex1].Value *= eliteMultipler;
                }
                // Modify the stats, then spawn as per usual
                SpawnEnemy(agent, pos); 
            }
        }
    }
    public void SpawnEnemy(ObjectPool fromPool, Vector3 pos)
    {
        // THIS FUNCTION MAY BE DEPRECATE, DO NOT USE SINCE NO AGENT VALID CHECK
        GameObject agent = fromPool.GetPooledObject();
        SpawnEnemy(agent, pos);
    }

    // Because events with number changing is not very reliable, rely on a function to handle the increase and decrease
    public delegate void EnemyDeathUpdate();
    public event EnemyDeathUpdate onEnemyDied;
    private void FireEnemyDied() { if (onEnemyDied != null) onEnemyDied(); }
    public void UpdateDeathNumbers(EnemyControllerV2 agent)
    {
        if (agent == null) { return; }
        if (agent.stats.id == EntityID.ELITE || agent.stats.id == EntityID.BIG_ELITE)
        {
            numElites--;
            LevelManager.Instance.numEliteKilled++;
        }
        else
        {
            numEnemies--;
            LevelManager.Instance.numKilled++;
        }
        FireEnemyDied();
        //controller.stats.OnDied -= context => { } ;  // stop listening to the function

        // THIS IS A CHEAT METHOD OF ADDING ID GENERATION
        // HOWEVER IT SHOULD WORK.

        // CONSIDERING THAT ID IS BUILT ON ENEMY KILLS, THE ENEMIES KNOW WHEN THEY ARE KILLED
        // THIS SETS THE RESPONSIBLITY OF THE ID GAIN GOING UP TO THE ENEMIES, INSTEAD OF THE PLAYER NEEDING A REFERFENCE TO THE ENEMY THAT THEY MIGHT HAVE KILLED
        // ALTERNATIVE IS PLAYER LISTENS TO THE HURTBOX THEY'VE HIT, AND CHECK ONDAMAGED FOR LETHAL DAMAGE

        if (player != null)
        {
            // Base amount gained per kill + percent increase
            player.Stats.ID_Bar.Value += player.Stats.Class.BaseIDGain * (1 + player.Stats.IDGain.Value);   // This is the percent increase, 1 + decimal to get flat
        }
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

    public EnemyControllerV2 GetNearestEnemy(Transform pos)
    {
        float closest = float.MaxValue;
        EnemyControllerV2 nearest = null;
        foreach (EnemyControllerV2 agent in aliveEnemies)
        {
            float distance = Vector3.Distance(agent.transform.position, pos.position);
            if (distance < closest)
            {
                nearest = agent;
            }
        }
        return nearest;
    }
    
    public float GetShortestDistance(Transform pos)
    {
        float shortest = float.MaxValue;
        foreach (EnemyControllerV2 agent in aliveEnemies)
        {
            float distance = Vector3.Distance(agent.transform.position, pos.position);
            {
                if (distance < shortest)
                    shortest = distance;
            }    
        }
        return shortest;
    }
    #endregion
    #endregion
    #region DISTANCE GROUPS
    
    public void InitDistanceGroups()
    {
        distanceGroups.Add(nearInterval, new List<EnemyControllerV2>());
        distanceGroups.Add(regularInterval, new List<EnemyControllerV2>());
        distanceGroups.Add(farInterval, new List<EnemyControllerV2>());
    }

    public void ResetDistanceGroups()
    {
        foreach (KeyValuePair<UpdateInterval, List<EnemyControllerV2>> group in distanceGroups)
        {
            group.Value.Clear();    // Reset the list of enemy controllerss
        }
    }
    public void AssignGameObjectToGroup(GameObject go, float distance)
    {
        EnemyControllerV2 controller = go.GetComponent<EnemyControllerV2>();
        AssignControllerToGroup(controller, distance);
    }

    public void AssignControllerToGroup(EnemyControllerV2 controller, Transform playerPos)
    {
        float distance = (controller.transform.position - playerPos.position).magnitude;
        AssignControllerToGroup(controller, distance);
    }
    public void AssignControllerToGroup(EnemyControllerV2 controller, float distance)
    {
        if (distance > farInterval.range)       // if the agent is far from the player
        {
            //controller.UpdateInterval = farInterval.range;
            //controller.PhysicsInterval = farInterval.range;
            distanceGroups[farInterval].Add(controller);
        }
        else if (distance < nearInterval.range) // if the agent is close to the player
        {
            //controller.UpdateInterval = nearInterval.time;
            //controller.PhysicsInterval = nearInterval.time;
            distanceGroups[nearInterval].Add(controller);
        }
        else    // the agent is not too far, and not too close
        {
            //controller.UpdateInterval = regularInterval.time;
            //controller.PhysicsInterval = regularInterval.time;
            distanceGroups[regularInterval].Add(controller);
        }
    }
    public void AssignControllerToGroup(EnemyControllerV2 controller, UpdateInterval group)
    {
        distanceGroups[group].Add(controller);
    }

    public UpdateInterval GetDistanceGroup(EnemyControllerV2 controller, Transform playerPos)
    {
        // Figure out which group the agent might be in
        foreach (KeyValuePair<UpdateInterval, List<EnemyControllerV2>> group in distanceGroups)
        {
            if (group.Value.Contains(controller))
            {
                return group.Key;
            }
        }
        return null;
    }
    public void RemoveControllerFromGroup(EnemyControllerV2 controller, UpdateInterval group)
    {
        distanceGroups[group].Remove(controller);
    }
    
    public void UpdateDistanceGroups()
    {
        // Start the coroutine to update the distance groups, only allow the update to happen if the group has completed it's update
        if (distanceGroupUpdated) // If the groups have been update, we want to update them again
        {
            StartCoroutine(HandleDistanceGroups());
        }
    }

    
    #endregion
    #region RUNTIME FUNCTIONS

    IEnumerator HandleDistanceGroups()
    {
        distanceGroupUpdated = false;

        // Get the near interval key, and search the list of near enemies if they should join a different distance group
        List<EnemyControllerV2> toCheck = new List<EnemyControllerV2>();
        if (distanceGroups.TryGetValue(nearInterval, out toCheck))
        {
            foreach (EnemyControllerV2 agent in toCheck)
            {
                // Check if they need to  join a different distance group
                Transform playerPos = player.transform;
                AssignControllerToGroup(agent, playerPos);
                yield return new WaitForSeconds(enemyCheckInterval);
            }
        }
        yield return new WaitForSeconds(groupCheckInterval);
        // because each step is subject to the yield, get the new values on the cycles
        if (distanceGroups.TryGetValue(nearInterval, out toCheck))
        {
            foreach (EnemyControllerV2 agent in toCheck)
            {
                // Check if they need to  join a different distance group
                Transform playerPos = player.transform;
                AssignControllerToGroup(agent, playerPos);
                yield return new WaitForSeconds(enemyCheckInterval);
            }
        }
        yield return new WaitForSeconds(groupCheckInterval);
        if (distanceGroups.TryGetValue(nearInterval, out toCheck))
        {
            foreach (EnemyControllerV2 agent in toCheck)
            {
                // Check if they need to  join a different distance group
                Transform playerPos = player.transform;
                AssignControllerToGroup(agent, playerPos);
                yield return new WaitForSeconds(enemyCheckInterval);
            }
        }
        distanceGroupUpdated = true;
        //yield return new WaitForSeconds(groupCheckInterval);

        // Next search the regular

        // Next search the far
        

        // foreach (KeyValuePair<UpdateInterval, List<EnemyControllerV2>> group in distanceGroups)
        // {
        //     // Search each group and see if they should be updated
        //     // Start with the nearest enemies

        //     yield return null;
        // }
    }

    // IEnumerator UpdateDistanceGroups()
    // {
    //     distanceGroupUpdated = false;
    //     //Debug.Log("[AIManager]: Started to update Distance Groups");
    //     // Do a staggered update of the distance groups, checking if the enemies should shift to a different one.
    //     foreach (KeyValuePair<UpdateInterval, List<EnemyController>> group in distanceGroups)
    //     {
    //         // Check each enemy in the current distance group and evaluate if they should be moved
    //         // Use a for loop since the size is being modified

    //         // This is inefficient when it comes to large sizes,
    //         // Cut this value into batches to make it more manageable

    //         // If I have a group of 600, I am possibly doing 600 reassignments
    //         for (int i = 0; i < group.Value.Count; i++)
    //         {
    //             // Get the distance between the agent and the player
    //             EnemyController enemy = group.Value[i];
    //             float dist = GetSquarePlayerDistance(enemy.transform.position);

    //             // If the distance is outside of the specified range of the CURRENT GROUP the agent is a member of
    //             if (dist < group.Key.range || dist > group.Key.range)
    //             {
    //                 // Remove the agent from the group
    //                 group.Value.Remove(enemy);
    //                 // Reassign the agent to another group
    //                 //AssignControllerToGroup(enemy, dist);
    //             }

    //             // Wait some time between each enemy update
    //             yield return new WaitForSeconds(enemyCheckInterval);
    //         }
    //         yield return new WaitForSeconds(nearInterval.time); // When cycling through the intervals, wait for some time before checking the next set
    //     }
    //     distanceGroupUpdated = true;
    // }


    // Commands will be sent on intervals based on the distance from the player
    // See Level Manager Cell/Grid System for spacing and boundaries

    public List<EnemyControllerV2> movingEnemies;
    public void RegisterMoving(EnemyControllerV2 agent)
    {
        if (movingEnemies != null)
            movingEnemies.Add(agent);
    }
    public void UnregisterMoving(EnemyControllerV2 agent)
    {
        if (movingEnemies != null)
            movingEnemies.Remove(agent);
    }

    // float nearClock = 0.0f;
    // bool nearUpdating = false;
    // float regularClock = 0.0f;
    // bool regularUpdating = false;
    // float farClock = 0.0f;
    // bool farUpdating = false;
    public void Update()
    {
        // Only update if a player exists
        if (player == null) { return; }

        // State handling can be pushed onto coroutine loops for interval steps and non update on irrelevant agents
        if (aliveEnemies == null || aliveEnemies.Count <= 0) { return; }
        // nearClock += Time.deltaTime;
        // regularClock += Time.deltaTime;
        // farClock += Time.deltaTime;

        // if (nearClock > nearInterval.time && !nearUpdating)
        // {
        //     StartCoroutine(UpdateDistanceGroup(nearInterval));
        //     StartCoroutine(FixedUpdateDistanceGroup(nearInterval));
        // }
        // if (regularClock > regularInterval.time && !regularUpdating)
        // {
        //     StartCoroutine(UpdateDistanceGroup(regularInterval));
        //     StartCoroutine(FixedUpdateDistanceGroup(nearInterval));
        // }
        // if (farClock > farInterval.time && !farUpdating)
        // {
        //     StartCoroutine(UpdateDistanceGroup(farInterval));
        //     StartCoroutine(FixedUpdateDistanceGroup(nearInterval));
        // }
        // Sort by distance groups since the amount in a given distance group is less

        

        foreach (EnemyControllerV2 enemy in aliveEnemies)
        {
            agentArgs.agent = enemy;        // Dynamically reassign the agent to the current enemy that is being evaluated
            enemy.HandleState(agentArgs);
        }
    }
    // public void FixedUpdate()
    // {
    //     // Only update if a player exists
    //     if (player == null) { return; }

    //     // State handling can be pushed onto coroutine loops for interval steps and non update on irrelevant agents
    //     if (aliveEnemies == null || aliveEnemies.Count <= 0) { return; }
    //     foreach (EnemyControllerV2 enemy in aliveEnemies)
    //     {
    //         if (enemy.State is IPhysicsState phyState)
    //         {
    //             agentArgs.agent = enemy;        // Dynamically reassign the agent to the current enemy that is being evaluated
    //             phyState.FixedAct(agentArgs);
    //         }
    //     }
    // }
    // public IEnumerator UpdateDistanceGroup(UpdateInterval group)
    // {
    //     if (group == nearInterval) { nearUpdating = true; }
    //     if (group == regularInterval) { regularUpdating = true; }
    //     if (group == farInterval) { farUpdating = true; }
    //     Debug.Log("Updating: " + group.range);
    //     foreach (EnemyControllerV2 agent in distanceGroups[group])
    //     {
    //         agentArgs.agent = agent;
    //         agent.HandleState(agentArgs);
    //         yield return null;
    //     }
    //     if (group == nearInterval) { nearUpdating = false; }
    //     if (group == regularInterval) { regularUpdating = false; }
    //     if (group == farInterval) { farUpdating = false; }
    // }

    // public IEnumerator FixedUpdateDistanceGroup(UpdateInterval group)
    // {
    //     if (group == nearInterval) { nearUpdating = true; }
    //     if (group == regularInterval) { regularUpdating = true; }
    //     if (group == farInterval) { farUpdating = true; }
    //     foreach (EnemyControllerV2 agent in distanceGroups[group])
    //     {
    //         if (agent.State is IPhysicsState physicsState)
    //         {
    //             agentArgs.agent = agent;
    //             physicsState.FixedAct(agentArgs);
    //             yield return new WaitForFixedUpdate();
    //         }
    //     }
    //     if (group == nearInterval) { nearUpdating = false; }
    //     if (group == regularInterval) { regularUpdating = false; }
    //     if (group == farInterval) { farUpdating = false; }
    // }

    // Instead of calling HandleState, call each reason and act function on the AI

    // This is because the step of the reason and act will be called diffrently
    // Reason (Decision making), will be lagged behind the actions of the agent

    // public void FixedUpdate()
    // {
    //     // if (aliveEnemies == null || aliveEnemies.Count <= 0) { return; }
    //     // if (movingEnemies == null || movingEnemies.Count <= 0) { return; }

    //     // // Instead of cycling between all enemies, cycle between the enemies that are registered in the MOVING list
    //     // // Whenever enemies enter a state where they should move, add them to the moving list

    //     // foreach (EnemyControllerV2 enemy in movingEnemies)
    //     // {
    //     //     // For a moving enemy, check their state
    //     //     // If they are in the chase state, set their target to the player

    //     //     if (enemy.State == AIState.StateId.Chase)
    //     //     {
    //     //         Cell target = GridUtility.GetCellFromPoint(flowfield.grid, enemy.transform.position);
    //     //         enemy.MoveAgent(target);
    //     //     }
    //     // }
    //     // foreach (EnemyControllerV2 enemy in aliveEnemies)
    //     // {
    //     //     Cell target = GridUtility.GetCellFromPoint(flowfield.grid, enemy.transform.position);
    //     //     enemy.MoveAgent(target);
    //     // }
    //     // foreach (EntityStats entity in aliveEnemies)
    //     // {
    //     //     Rigidbody rb = entity.GetComponent<Rigidbody>();    // Get component is probably expensive, better to have a reference
    //     //     // Only way to have a reference is another script / entity stats contains a reference
    //     //     Cell target = GridUtility.GetCellFromPoint(flowfield.grid, entity.transform.position);
    //     //     Vector3 direction = new Vector3(target.bestDirection.vector.x, 0, target.bestDirection.vector.y);

    //     //     // Vector3 movement = Vector3.MoveTowards(entity.transform.position, direction, 6.0f * Time.deltaTime);
    //     //     // entity.transform.position = movement;
    //     //     //Vector3 direction = new Vector3(target.bestDirection.vector.x, 0, target.bestDirection.vector.y);
    //     //     ////Debug.Log("Move Direction: " + target.bestDirection.vector);
    //     //     rb.velocity = direction * 6.0f;
    //     //     //Debug.Log("moved");

    //     //     // Instead, get each enemy controller

    //     //     // And handle the transitions through this loop
    //     //     // Handling the states will determine 

    //     // }

    //     // With new state setup, we can organize the chasing enemies into a list
    //     // With a limited list of chasing enemies, time of this update is reduced (don't have to update non-chasing enemies)

    // }
    IEnumerator ManagerLoop()
    {
        // While the level has not ended, run updates for the distance groups and spawn enemies
        // Update Coroutine
        distanceGroupUpdated = true;
        enemiesSpawned = true;
        float groupTimer = 0.0f;
        //float spawnTimer = 0.0f;
        //float moveTimer = 0.0f;

        StartCoroutine(HordeHandler());

        while (!LevelManager.Instance.LevelComplete)
        {
            //spawnTimer += Time.deltaTime;
            groupTimer += Time.deltaTime;
            if (groupTimer > groupCheckInterval)
            {
                UpdateDistanceGroups();
            }
            if (canSpawn)
            {
                if (numEnemies <= 0)
                {
                    SpawnEnemies();
                }
            }
            // Start Coroutines to run the functions, don't start additional coroutines until the current one has completed
            yield return null;
        }
    }
    #endregion
    #region DEBUG
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
    void OnDrawGizmos()
    {
        if (spawnDots != null && spawnDots.Count > 0)
        {
            Gizmos.color = Color.magenta;
            foreach (Vector3 dot in spawnDots)
            {
                Gizmos.DrawSphere(dot, 0.5f);
            }
        }
    }
    #endregion
}