using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
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

    [SerializeField] float overFlowThreshold = 2.0f;    // if there is this much more, it is considered overflow (multiply, 1 = 1)
    [SerializeField] float bufferThreshold = 1.5f;      // buffer value (multipled, 1 = 1)

    [SerializeField] float minSpawnDistance = 2.5f;
    [SerializeField] float maxSpawnDistance = 5.0f;

    [SerializeField] float updateInterval = 0.5f;

    [Range(0, 3.0f)]
    [SerializeField] float spawnInterval = 1.0f;
    [SerializeField] ObjectPool[] enemyPools;

    private List<Vector3> spawnLocations;

    #region UNITY FUNCTIONS
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        LevelLoading();
    }

    // level manager will send out data to all the enemies within the level
    // get rid of the object pool manager and have this level manager builds the pools accordingly

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
        StartCoroutine(SpawnEnemies());
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

    }
    #endregion
    IEnumerator SpawnEnemies()
    {
        // Get the cell the player is in
        // Build a circle, spawn enemy bundles at those positions
        spawnLocations = new List<Vector3>();
        foreach (EntityNumber group in LevelManager.Instance.Details.enemiesToSpawn)
        {
            GameObject enemy = group.entity;
            Debug.Log("[AIManager]: Spawning " + enemy.name);
            foreach (ObjectPool pool in enemyPools)
            {
                // Cycle the number of enemies to spawn
                if (pool.Prefab == enemy)
                {
                    int numEnemies = Random.Range(group.minSpawn, group.maxSpawn);
                    for (int i = 0; i < numEnemies; i++)
                    {
                        int rotIndex = Random.Range(0, CircleUtility.MAX_CIRCLE_POSITIONS);
                        float spawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

                        // Getting the rotation around the player
                        if (player != null)
                        {
                            Vector3 offsetPos = player.transform.position + (player.transform.forward * spawnDistance);
                            Vector3 spawnPos = player.transform.position;

                            spawnPos.x += (CircleUtility.CircleListInstance[rotIndex].x * spawnDistance);
                            spawnPos.z += (CircleUtility.CircleListInstance[rotIndex].z * spawnDistance);
                            spawnPos.y = LevelManager.Instance.FloorOffset;
                            spawnLocations.Add(spawnPos);
                            //Debug.Log("rotIndex: " + rotIndex + ", Position: " + spawnPos);
                            GameObject enemyGroup = pool.GetPooledObject();
                            if (enemyGroup != null)
                            {
                                enemyGroup.transform.position = spawnPos;
                                enemyGroup.SetActive(true);
                            }
                            yield return new WaitForSeconds(spawnInterval);
                        }
                    }
                }
            }
        }
        yield return null;
    }

    // Commands will be sent on intervals based on the distance from the player
    // See Level Manager Cell/Grid System for spacing and boundaries
    IEnumerator SendCommands()
    {
        // While the level has not ended, send out commands to enemies
        while (!LevelManager.Instance.LevelComplete)
        {

        }
        yield return null;
    }

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

    private void UpdatePlayerValue(GameObject leader)
    {
        EnemyController enemyControl = null;
        enemyControl.GetComponent<EnemyController>();

        // Because the expense is at the SET DESTINATION
        // (Path Recalcuation)
        // Call the path updates and recalcuations here, sending the data to the enemy controller

        // Or, reduce FSM update frequency of the enemy controllers.
        // Look at FSM base class, and tweak the update frequency based on the distance
    }

    #endregion

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
        if (CircleUtility.CircleListInstance != null && CircleUtility.CircleListInstance.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (Vector3 dot in CircleUtility.CircleListInstance)
            {
                Gizmos.DrawCube(dot, cubeSize);
            }
        }
    }
}