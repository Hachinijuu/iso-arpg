using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.AI.Navigation;
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
}
#endregion
public class LevelManager : MonoBehaviour
{
    // Level Managers WILL NOT be persistent
    // They are bound to a scene and provide the relevant information for the given scene
    // They will get references to other persistent managers in order to handle certain scenarios
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
    [SerializeField] CinemachineVirtualCamera vCam;

    [Header("Level Settings")]
    [SerializeField] private Transform playerSpawn;
    public Transform PlayerSpawnPoint { get { return playerSpawn; } }
    public LevelType type;
    public enum LevelType { NONE, CLEAR, ELITE }
    [SerializeField] private LevelDetails details;
    public LevelDetails Details { get { return details; } }

    public float FloorOffset { get { return floorOffset; } }
    private float floorOffset = 0.25f;

    [Header("Grid Settings")]
    [SerializeField] private NavMeshSurface levelArea;
    [SerializeField] private int cellSize = 10;
    [SerializeField] private int cellTrim = 1;
    public List<string> gridBlockTags;
    [SerializeField] private Cell playerCell;

    [Header("Debug")]
    public bool ShowCubes;
    public bool DrawFullTile;

    // This level manager will handle the loading between scenes

    // Level manager builds a grid for other systems in the level to utilize
    // This would allow the AI manager and destructibles to be spawned randomly throughout the level

    // Grid info
    [SerializeField] private Vector3 origin;
    public Cell[,] cells; // 2D array of grid cells  
    public GameObject[] obstacles;
    private List<Collider> obstacleColliders;
    int gRows;
    int gColumns;

    // Gameplay info
    PlayerController player;                // Reference to the player
    //List<GameObject> levelEnemies;          // The enemies to spawn in this level
    //List<GameObject> levelDestructibles;    // The destructibles to spawn in this level
    float timeSpent;                        // The time spent in the level
    int numKilled;
    int numEliteKilled;

    public bool LevelComplete { get { return levelComplete; } }
    bool levelComplete = false;


    // TODO:
    // add local difficulty scaling


    // UNITY FUNCTIONS
    private void Awake()
    {
        InitLevel();
    }

    // LEVEL MANAGER FUNCTIONS
    public void InitLevel()
    {
        GetObstacles();
        if (levelArea != null)
            BuildGrid(levelArea);
        else
            Debug.LogWarning("[LevelManager]: Missing NavMesh Surface");

        if (obstacles != null && cells != null)
            MarkGridObstacles();
    }

    public void Start()
    {
    }

    public void LevelLoaded()
    {
        InitLevel();

        if (player == null)
            player = GameManager.Instance.Player;
            //player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (player == null)
            Debug.Log("[LevelManager]: Failed to reference player");

        // Setup the camera
        vCam.Follow = player.transform;

        playerCell = GetCellFromIndex(GetIndexFromPoint(player.transform.position));

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
            //GetIndexFromPoint(player.transform.position);
            //GetIndexFromPosition(player.transform.position);
            UpdatePlayerCell();
            //Debug.Log(timeSpent);
            CheckLevelComplete();
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
        Debug.Log("Level has been completed");
    }

    // GRID SYSTEM
    #region GRID SYSTEM FUNCTIONS
    public void BuildGrid(NavMeshSurface mesh)
    {
        Bounds playArea = mesh.navMeshData.sourceBounds;
        Vector3 playableArea = playArea.size;
        // setting up the values

        // if the play area is 100, divide it by the size of the cells
        // 100 / 2 = 50 -> 50 cells with a 2 size to fit
        // want to offset the ends of the play space, take a value from each side
        gRows = (Mathf.RoundToInt(playableArea.x) / cellSize) - cellTrim;       // Top and Bottom cutoff
        gColumns = (Mathf.RoundToInt(playableArea.z) / cellSize) - cellTrim;    // Left and Right cutoff

        // offset in each direction is half the play size
        // to build the grid accurately, tile center needs to be accounted for in the offset.
        // Mathf.RoundToInt((playableArea.x / 2f) - (cellSize / 2)) will build the first tile within the playspace given the origin
        int rowOffset = Mathf.RoundToInt((playableArea.x / 2f) - (cellSize / 2));
        int columnOffset = Mathf.RoundToInt((playableArea.z / 2f) - (cellSize / 2));

        // account for the trimmed excess, half the cell size since center.
        rowOffset -= (cellTrim * (cellSize / 2));
        columnOffset -= (cellTrim * (cellSize / 2));

        // origin point
        origin = new Vector3(-rowOffset, 0, columnOffset);

        // initialize the cells array
        cells = new Cell[gRows, gColumns];

        // build the cells
        for (int row = 0; row < gRows; row++)
        {
            for (int col = 0; col < gColumns; col++)
            {
                Cell cell = new Cell(col, row);
                Vector3 cellPos = origin;
                cellPos.x += (cellSize * row);
                cellPos.z -= (cellSize * col);
                cell.position = cellPos;
                cell.boundingBox = new Bounds(cell.position, new Vector3(cellSize, 0, cellSize));
                cells[row, col] = cell;
            }
        }
    }

    public void GetObstacles()
    {
        if (obstacles == null)
        {
            foreach (string tag in gridBlockTags)
            {
                obstacles = GameObject.FindGameObjectsWithTag(tag);
            }
        }
    }
    public void MarkGridObstacles()
    {
        obstacleColliders = new List<Collider>();
        if (obstacles != null && obstacles.Length > 0)
        {
            foreach (GameObject obstacle in obstacles)
            {
                Collider col = obstacle.GetComponent<Collider>();
                obstacleColliders.Add(col);
            }
        }

        // once obstacles have been created, go through the existing grid and invalidate any cells that do not exist
        foreach (Collider col in obstacleColliders)
        {
            for (int i = 0; i < gRows; i++)
            {
                for (int j = 0; j < gColumns; j++)
                {
                    if (col.bounds.Intersects(cells[i, j].boundingBox))
                        cells[i, j].isObstacle = true;
                }
            }
        }
    }
    public bool CheckPointInCell(Vector3 point, Cell toCheck)
    {
        return toCheck.boundingBox.Contains(point);
    }

    // Active Grid allows grid tiles to detect real-time gameplay information
    // This will provide the player location

    public void UpdatePlayerCell()
    {
        if (playerCell != null) // if the player cell has already been found
        {
            // check the magnitude between the player and the cell
            // if the magnitude is NOT larger than the size of a cell,
            //Debug.Log((Vector3.SqrMagnitude(playerCell.position - player.transform.position)) > cellSize);
            //Debug.Log(!(Vector3.SqrMagnitude(playerCell.position - player.transform.position) > cellSize));
            if (!(Vector3.SqrMagnitude(playerCell.position - player.transform.position) > cellSize))
            {
                return;
            }
            else
            {
                CellIndex cellIndex = GetIndexFromPoint(player.transform.position);
                playerCell = GetCellFromIndex(cellIndex);
            }
        }
    }

    public bool PointInBounds(Vector3 point)
    {
        float width = gRows * cellSize;
        float height = gColumns * cellSize;

        return ((point.x >= origin.x) && (point.x <= (origin.x + width)) &&
                (point.z <= origin.z) && (point.z >= (origin.z - height)));
    }
    public CellIndex GetIndexFromPoint(Vector3 pos)
    {
        // SHOULD ADD CHECK TO PREVENT A SEARCH POS IN NON PLAYSPACE
        //Debug.Log(PointInBounds(pos));
        if (PointInBounds(pos))
        {
            pos -= origin; // gets the difference between the point and the origin

            // start counting the rows and columns from the grid point
            int row = (int)(pos.x / cellSize);
            int col = (int)(pos.z / cellSize);

            // based on where the origin is, normalize the values to be non-negative
            if (origin.x < 0)
                col *= -1;
            else
                row *= -1;

            return new CellIndex(row, col);
        }
        return new CellIndex(-1, -1); // return a vector with an INVALID location in the array
    }

    public CellIndex GetRandomIndex(int tryCycles)
    {
        // Only return valid indices to use, valid indicies are
        // Unoccupied
        // Not obstacles

        int count = 0;
        do
        {
            count++;
            int row = Random.Range(0, gRows);
            int col = Random.Range(0, gColumns);

            // If the cell is not an obstacle or occupied, use that cell
            if (!(cells[row, col].isObstacle) || !cells[row, col].isOccupied)
            {
                return new CellIndex(row, col);
            }
        }
        while (count <= tryCycles);
        return new CellIndex(-1, -1);
    }

    // Get the cell given two int values
    public Cell GetCellFromIndex(int x, int y)
    {
        if (x != -1 && y != -1)
        {
            return cells[x, y];
        }
        return null;
    }

    // Get the cell given a package of 2 int values
    public Cell GetCellFromIndex(CellIndex packIndex)
    {
        if (packIndex.x != -1 && packIndex.y != -1)
        {
            return cells[packIndex.x, packIndex.y];
        }
        return null;
    }

    #endregion
    private void OnDrawGizmosSelected()
    {
        if (ShowCubes)
        {
            Gizmos.color = Color.green;
            if (origin != null)
                Gizmos.DrawSphere(origin, 2.0f);
            if (cells != null)
            {
                Vector3 cell;
                if (DrawFullTile)
                    cell = new Vector3(cellSize, 0, cellSize);
                else
                    cell = new Vector3(cellSize / cellSize, 0, cellSize / cellSize);
                for (int row = 0; row < gRows; row++)
                {
                    for (int col = 0; col < gColumns; col++)
                    {
                        if (row % 2 == 0)
                        {
                            Gizmos.color = Color.blue;
                            if (col % 2 == 0)
                                Gizmos.color = Color.red;
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                            if (col % 2 == 0)
                                Gizmos.color = Color.blue;
                        }
                        Cell currentCell = cells[row, col];

                        if (!currentCell.isObstacle)
                            Gizmos.DrawWireCube(currentCell.position, cell);
                    }
                }
            }
        }
    }
}
