using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleManager : MonoBehaviour 
{
    #region VARIABLES
    private static DestructibleManager instance = null;
    public static DestructibleManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DestructibleManager>();
            if (!instance)
                Debug.LogWarning("[DestructibleManager] No Destructible manager found");
            return instance;
        }
    }

    [Tooltip("Should destructible objects spawn?")]
    [SerializeField] bool spawnDestructs;
    public bool SpawningComplete { get {return spawningComplete; } }
    bool spawningComplete;
    [SerializeField] ObjectPool[] destructibles; 


    // get a point to the grid data
    public GridData grid;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        //LevelLoading();
    }
    #endregion

    #region SCENE LOADS
    // When a level (scene) is loading, call this function
    public void LevelLoading()
    {
        // Check if the level is EXPECTING any destructibles
        if (LevelManager.Instance.Details.destructiblesToSpawn != null && LevelManager.Instance.Details.destructiblesToSpawn.Count > 0)
        {
            if (LevelManager.Instance.grid != null)
            {
                grid = LevelManager.Instance.grid;
                if (grid.cells == null)
                {
                    grid.LoadFromList();
                }
                if (destructibles != null && destructibles.Length > 0)
                {
                    StartCoroutine(SpawnDestructibles());
                }
                else
                    Debug.LogError("[DestructibleManager]: Missing Destructible Object Pools");
            }
        }
    }

    // When a level (scene) is unloading, call this function
    public void LevelUnloading()
    {
        foreach (ObjectPool destructibles in destructibles)
        {
            destructibles.Reset();
        }
    }
    #endregion  

    #region SPAWNING

    // This spawning is based on the grid system
    IEnumerator SpawnDestructibles()
    {
        foreach (EntityNumber group in LevelManager.Instance.Details.destructiblesToSpawn)
        {
            GameObject destructType = group.entity;
            Debug.Log("[DestructibleManager]: Spawining " + destructType.name);
            foreach (ObjectPool pool in destructibles)
            {
                int numSpawns = Random.Range(group.minSpawn, group.maxSpawn);
                int count = 0;
                for (int i = 0; i < numSpawns; i++)
                {
                    // Get a random cell from the grid
                    CellIndex cellIndex = GridUtility.GetRandomIndex(grid, 10);
                    // If a valid cell was found
                    if (cellIndex.x != -1 && cellIndex.y != -1)
                    {
                        // Spawn the object at the cell\
                        Cell currCell = GridUtility.GetCellFromIndex(grid, cellIndex);
                        Vector3 spawnPos = currCell.position;
                        spawnPos.y = LevelManager.Instance.FloorOffset;

                        GameObject destructible = pool.GetPooledObject();
                        if (destructible != null)
                        {
                            destructible.transform.position = currCell.position;    // Place the destructible at the cell
                            destructible.SetActive(true);                           // Set it active
                            currCell.isOccupied = true;                             // Flag the cell as occupied
                        }
                        count++;
                    }
                    else
                    {
                        Debug.Log("[DestructibleManager]: Failed to find eligible cell, skipping");
                        continue;
                    }
                }
                Debug.Log("[DestructibleManager]: Spawned " + count + " " + destructType.name + ", Expected " + numSpawns);
            }
        }
        yield return null;
    }

    #endregion
}