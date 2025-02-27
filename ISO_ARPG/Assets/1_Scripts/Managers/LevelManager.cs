using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region HELPER DATA STORAGE
[System.Serializable]
public struct EntityNumber
{
    public GameObject entity;
    public int minSpawn;
    public int maxSpawn;
}

[System.Serializable]
public class LevelDetails
{
    // Use of lists instead of dictionaries because dictionaries cannot be set in editor
    public List<EntityNumber> enemiesToSpawn;
    public List<EntityNumber> destructiblesToSpawn;
    public int enemiesToKill;
    public int elitesToKill;
    public List<Horde> hordes;
}

[System.Serializable]
public class Horde
{
    // None, no condition
    // Time - Set the time the horde should spawn
    // Kills - Set the number of kills to spawn the horde
    public enum SpawnCondition { NONE, TIME, KILLS }
    public SpawnCondition hordeCondition;
    public List<EntityNumber> enemiesToSpawn;
    public int TriggerAmount;
    public bool Triggered = false;
    public bool CheckShouldSpawn(float num)
    {
        if (num >= TriggerAmount)
        {
            return true;
        }
        return false;
    }

}
#endregion
public class LevelManager : MonoBehaviour
{
    // Level Managers WILL NOT be persistent
    // They are bound to a scene and provide the relevant information for the given scene
    // They will get references to other persistent managers in order to handle certain scenarios
    #region VARIABLES
    private static LevelManager instance = null;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<LevelManager>();

            if (!instance)
                Debug.LogWarning("[LevelManager]: No level manager found");
            return instance;
        }
    }
    public Camera LevelCamera;
    [SerializeField] CinemachineVirtualCamera vCam;

    [Header("Level Settings")]
    [SerializeField] private Transform playerSpawn;
    public Transform PlayerSpawnPoint { get { return playerSpawn; } }
    [SerializeField] private List<EnemySpawnPoint> enemySpawnPoints;
    public List<EnemySpawnPoint> EnemySpawnPoints { get { return enemySpawnPoints; } }
    public LevelType type;
    public enum LevelType { NONE, CLEAR, ELITE }
    [SerializeField] private LevelDetails details;
    public LevelDetails Details { get { return details; } }
    public float FloorOffset { get { return floorOffset; } }
    private float floorOffset = 0.25f;

    public FloorMaterials material;

    // This level manager will handle the loading between scenes

    // Level manager builds a grid for other systems in the level to utilize
    // This would allow the AI manager and destructibles to be spawned randomly throughout the level
    [SerializeField] GridData gData;
    public Grid grid;
    // Gameplay info
    PlayerController player;                // Reference to the player
    //List<GameObject> levelEnemies;          // The enemies to spawn in this level
    //List<GameObject> levelDestructibles;    // The destructibles to spawn in this level
    float timeSpent;                        // The time spent in the level
    public float TimeSpent { get { return timeSpent; } }
    public int numKilled;
    public int numEliteKilled;

    public Transform loaderLocation;
    public LevelVolume levelLoader;
    public bool LevelComplete { get { return levelComplete; } }
    bool levelComplete = false;

    #endregion
    // TODO:
    // add local difficulty scaling
    #region LEVEL MANAGER
    public void LevelLoading()
    {
        // Check if the cell array is empty, if it is, load in from the data
        if (grid == null && type != LevelType.NONE)
        {
            grid = new Grid(gData);
            // if (grid.cells == null || grid.cells.Length <= 0)
            // {
            //     grid.LoadFromList();
            // }
        }
    }
    public void LevelLoaded()
    {
        player = PlayerManager.Instance.currentPlayer;
        //if (player == null)
        //    player = GameManager.Instance.Player;
        ////player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        //if (player == null)
        //    Debug.Log("[LevelManager]: Failed to reference player");

        // Setup the camera
        vCam.Follow = player.transform;
        //playerCell = GetCellFromIndex(GetIndexFromPoint(player.transform.position));

        if (type != LevelType.NONE)
            StartCoroutine(HandlePlayLevel());
    }
    // Start this enumerator when the level begins to play
    IEnumerator HandlePlayLevel()
    {
        // While the level has not been completed, increase the timer and scale the difficulty accordingly
        while (!levelComplete)
        {
            timeSpent += Time.deltaTime;
            //UpdatePlayerCell();
            CheckLevelComplete();

            // Level Manager will have Mob / Crowd lists and activation conditions, this will handle the activation of said instances
            yield return null;
        }

        // Once the level is complete, spawn the portal to exit the level
        LevelCompleted();
    }
    bool CheckLevelComplete()
    {
        switch (type)
        {
            case LevelType.CLEAR:
                if (numKilled >= details.enemiesToKill && details.enemiesToKill != -1)
                {
                    levelComplete = true;
                    return levelComplete;
                }
                break;
            case LevelType.ELITE:
                if (numEliteKilled >= details.elitesToKill && details.elitesToKill != -1)
                {
                    levelComplete = true;
                    return levelComplete;
                }
                break;
        }
        return false;
    }

    void LevelCompleted()
    {
        // Spawn the portal here
        if (levelLoader != null)
        {
            if (!levelLoader.gameObject.activeInHierarchy)
            {
                if (loaderLocation != null)
                {
                    Vector3 pos = loaderLocation.position;
                    pos.y += floorOffset;
                    levelLoader.gameObject.transform.position = pos;
                }
                levelLoader.gameObject.SetActive(true);
            }
        }

        Debug.LogWarning("Level has been completed");
    }
    #endregion

    public bool showGrid = false;
    #region DEBUG
    void OnDrawGizmos()
    {
        if (!showGrid)
        {
            return;
        }
        if (gData != null)
        {
            if (grid == null)
                grid = new Grid(gData);
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(grid.origin, 0.5f);
                Vector3 cell = new Vector3(grid.cellSize, 0, grid.cellSize);
                foreach (KeyValuePair<Vector2Int, Cell> c in grid.gridCells)
                {
                    if (c.Value.isObstacle)
                        Gizmos.color = Color.magenta;
                    else
                        Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(c.Value.position, cell);
                }
            }
        }
    }
    #endregion

    // Instead, level manager will have a prebuilt grid, and will only contain the cells to reference
}
