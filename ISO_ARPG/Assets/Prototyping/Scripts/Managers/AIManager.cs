using System.Collections;
using System.Text.RegularExpressions;
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
    LevelManager levelManager;

    [SerializeField] float overFlowThreshold = 2.0f;    // if there is this much more, it is considered overflow (multiply, 1 = 1)
    [SerializeField] float bufferThreshold = 1.5f;      // buffer value (multipled, 1 = 1)
    [SerializeField] float spawnDistance = 5.0f;
    [SerializeField] ObjectPool[] enemyPools;

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
        foreach (EntityNumber group in LevelManager.Instance.Details.enemiesToSpawn)
        {
            GameObject enemy = group.entity;
            foreach (ObjectPool pool in enemyPools)
            {
                // Cycle the number of enemies to spawn
                if (pool.Prefab == enemy)
                {
                    int numEnemies = Random.Range(group.minSpawn, group.maxSpawn);
                    for (int i = 0; i < numEnemies; i++)
                    {
                        if (player != null)
                        {
                            Vector3 offsetPos = player.transform.position * spawnDistance;
                            Vector3 spawnPos = offsetPos;
                            int rotIndex = Random.Range(0, CircleUtility.MAX_CIRCLE_POSITIONS);
                            spawnPos.x = offsetPos.x + (CircleUtility.CircleListInstance[rotIndex].x);
                            spawnPos.x = offsetPos.x + (CircleUtility.CircleListInstance[rotIndex].x);
                            spawnPos.y = LevelManager.Instance.FloorOffset;

                            GameObject enemyGroup = pool.GetPooledObject();
                            if (enemyGroup != null)
                            {
                                enemyGroup.transform.position = spawnPos;
                                enemyGroup.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }
    IEnumerator SendCommands()
    {
        yield return null;
    }
    #endregion
}