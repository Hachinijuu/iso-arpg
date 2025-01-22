using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Cell 
{
    public int x;
    public int y;
    public Vector3 position;    // this is the centre of the cell in gamespace
    public Bounds boundingBox;
    public bool isObstacle = false;

    public Cell()
    {
        x = 0;
        y = 0;
    }
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
public class LevelManager : MonoBehaviour
{
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
    public int EnemyNumber { get {return numEnemies; } }
    public int DestructibleNumber { get {return numDestructibles; } }
    [Header("Settings")]    
    [SerializeField] private NavMeshSurface levelArea;
    [SerializeField] private int cellSize = 10;
    [SerializeField] private int cellTrim = 1;
    [SerializeField] private int numEnemies;
    [SerializeField] private int numDestructibles;

    public List<string> gridBlockTags;

    [Header("Debug")]
    public bool ShowCubes;
    public bool DrawFullTile;

    // This level manager will handle the loading between scenes

    // Level manager builds a grid for other systems in the level to utilize
    // This would allow the AI manager and destructibles to be spawned randomly throughout the level
    
    // Grid info
    private Vector3 origin;
    public Cell[,] cells; // 2D array of grid cells  
    public GameObject[] obstacles;
    private List<Collider> obstacleColliders;
    int gRows;
    int gColumns;

    // Gameplay info
    List<GameObject> levelEnemies;          // The enemies to spawn in this level
    List<GameObject> levelDestructibles;    // The destructibles to spawn in this level
    float timeSpent;                        // The time spent in the level
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
        StartCoroutine(HandlePlayLevel());
    }

    // Start this enumerator when the level begins to play
    IEnumerator HandlePlayLevel()
    {
        // While the level has not been completed, increase the timer and scale the difficulty accordingly
        while (!levelComplete)
        {
            timeSpent += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
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
                Cell cell = new Cell(row, col);
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
                    if (col.bounds.Intersects(cells[i,j].boundingBox))
                        cells[i,j].isObstacle = true;
                }
            }
        }
    }
    public bool CheckPointInCell(Vector3 point, Cell toCheck)
    {
        return toCheck.boundingBox.Contains(point);
    }
    #endregion
    private void OnDrawGizmos() 
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
                    cell = new Vector3(cellSize, 0.125f, cellSize);
                else
                    cell = new Vector3(cellSize / cellSize, 0.125f, cellSize / cellSize);
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
                        Cell currentCell = cells[row,col];

                        if (!currentCell.isObstacle)
                            Gizmos.DrawWireCube(currentCell.boundingBox.center, currentCell.boundingBox.size);
                    }
                }
            }
        }
    }
}
