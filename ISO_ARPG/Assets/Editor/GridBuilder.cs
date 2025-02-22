using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.AI.Navigation;

[CustomEditor(typeof(GridUtility))]
public class EditorGridBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GridBuilder util = (GridBuilder)target;
        
        if (GUILayout.Button("Build Grid"))
        {
            util.BuildGrid();
        }

        if (GUILayout.Button("Clear Obstacles"))
        {
            util.CullObstacles();
        }

        if (GUILayout.Button("Save Grid"))
        {
            util.SaveGrid();
        }

        if (GUILayout.Button("Load To Level"))
        {
            util.LoadToLevel();
        }
        

    }

    // static int cellSize;

    // [MenuItem("Window/Debug")]
    // public static void ShowWindow()
    // {
    //     EditorWindow.GetWindow(typeof(GridBuilder));
    // }
}

public class GridBuilder : MonoBehaviour
{
// Grid info
    [SerializeField] private Vector3 origin;
    public Cell[,] cells; // 2D array of grid cells  
    public List<GameObject> obstacles;
    private List<Collider> obstacleColliders;
    int gRows;
    int gColumns;


    [Header("Grid Settings")]
    [SerializeField] private NavMeshSurface levelArea;
    [SerializeField] private int cellSize = 10;
    [SerializeField] private int cellTrim = 1;
    public List<string> gridBlockTags;
    //[SerializeField] private Cell playerCell;

    [Header("Debug")]
    public bool ShowCubes;
    public bool DrawFullTile;
    #region GRID SYSTEM FUNCTIONS

    // TODO: THIS GRID SYSTEM MAY BE MOVED TO ANOTHER SCRIPT
    // THIS SCRIPT WILL HAVE THE FUNCTIONALITY THAT WILL POPULATE THE GIVEN LEVEL'S CELLS
    // THIS IS TO ORGANIZE THE SYSTEM AND MAKE IT EASIER TO BUILD GRID HELPER FUNCTIONS WHILE LEAVING THE LEVEL MANAGER CONTAINED
    
    // Data to move around and keep saved (serialized should be saved)
    public GridData grid;
    public void BuildGrid()
    {
        Bounds playArea = levelArea.navMeshData.sourceBounds;

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
        rowOffset -= (cellTrim * (cellSize / 2)) + cellTrim;
        columnOffset -= (cellTrim * (cellSize / 2)) + cellTrim;

        // origin point
        // Grid build also needs to be offset by the position of where the NavMesh surface is
        Vector3 pos = levelArea.gameObject.transform.position;

        origin = pos + new Vector3(-rowOffset, 0, columnOffset);

        // Also offset the origin by the cell trim

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

        Debug.Log("[GridUtility]: Generated " + gRows + " x " + gColumns + " grid (" + cells.Length + ") tiles" );
    }

    public void GetObstacles()
    {
        obstacles = new List<GameObject>();
        //GameObject[] arr;
        foreach (string tag in gridBlockTags)
        {
            GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    Debug.Log("Added: " + array[i].name + " to the list of obstacles");
                    obstacles.Add(array[i]);
                }
            }
            else
            {
                Debug.Log("Empty Array of Obstacles");
            }
        }
    }
    public void MarkGridObstacles()
    {
        obstacleColliders = new List<Collider>();
        if (obstacles != null && obstacles.Count > 0)
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

    public void CullObstacles()
    {
        GetObstacles();
        MarkGridObstacles();
        Debug.Log("[GridUtility]: Marked obstructed tiles as unusable");
    }

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
    public void SaveGrid()
    {
        grid = ScriptableObject.CreateInstance<GridData>();
        grid.cellList = grid.ArrayToList(cells);
        grid.cellSize = cellSize;
        grid.columns = gColumns;
        grid.rows = gRows;
        grid.origin = origin;

        string path = "Assets/0_Scenes/" + EditorSceneManager.GetActiveScene().name + "_GridData.asset";
        AssetDatabase.CreateAsset(grid, path);
        AssetDatabase.SaveAssets();

        Debug.Log("[GridUtility]: Grid saved to " + path);
    }

    public void LoadToLevel()
    {
        GridData levelGrid = new GridData(grid);
        LevelManager.Instance.grid = levelGrid;
        Debug.Log("[GridUtility]: Loaded Grid Into Current Level");
    }
    #endregion
}