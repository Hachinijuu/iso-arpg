using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.UIElements;




#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
public class GridBuilder : MonoBehaviour
{
// Grid info
    [SerializeField] private Vector3 origin;
    public Cell[,] cells; // 2D array of grid cells  
    public List<Cell> removeCells;
    public List<GameObject> obstacles;
    private List<Collider> obstacleColliders;
    int gRows;
    int gColumns;

    public bool cellMarker;


    [Header("Grid Settings")]
    public NavMeshSurface levelArea;
    public int cellSize = 10;
    public int cellTrim = 1;
    public List<string> gridBlockTags;
    //[SerializeField] private Cell playerCell;
    Grid builtGrid;

    [Header("Debug")]
    public bool ShowCube = true;
    public bool DrawFullTile = true;
    #region GRID SYSTEM FUNCTIONS

    // TODO: THIS GRID SYSTEM MAY BE MOVED TO ANOTHER SCRIPT
    // THIS SCRIPT WILL HAVE THE FUNCTIONALITY THAT WILL POPULATE THE GIVEN LEVEL'S CELLS
    // THIS IS TO ORGANIZE THE SYSTEM AND MAKE IT EASIER TO BUILD GRID HELPER FUNCTIONS WHILE LEAVING THE LEVEL MANAGER CONTAINED
    
    // Data to move around and keep saved (serialized should be saved)
    public GridData grid;
    public void BuildGrid()
    {
        if (removeCells != null)
            removeCells.Clear();
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

        origin = pos + new Vector3(-rowOffset, 0, -columnOffset);

        // Also offset the origin by the cell trim

        // initialize the cells array
        cells = new Cell[gRows, gColumns];  // x vs y

        // build the cells

        for (int col = 0; col < gColumns; col++)
        {
            for (int row = 0; row < gRows; row++)
            {
                Cell cell = new Cell(row, col);
                Vector3 cellPos = origin;
                cellPos.x += (cellSize * row);
                cellPos.z += (cellSize * col);
                cell.position = cellPos;
                cell.boundingBox = new Bounds(cell.position, new Vector3(cellSize, 0, cellSize));
                cells[row, col] = cell;
            }
        }
        // for (int row = 0; row < gRows; row++)
        // {
        //     for (int col = 0; col < gColumns; col++)
        //     {
        //         Cell cell = new Cell(row, col);
        //         Vector3 cellPos = origin;
        //         cellPos.x += (cellSize * row);
        //         cellPos.z -= (cellSize * col);
        //         cell.position = cellPos;
        //         cell.boundingBox = new Bounds(cell.position, new Vector3(cellSize, 0, cellSize));
        //         cells[row, col] = cell;
        //     }
        // }

        // grid = ScriptableObject.CreateInstance<GridData>();
        // grid.cellList = GridUtility.ArrayToList(cells);
        // grid.cellSize = cellSize;
        // grid.columns = gColumns;
        // grid.rows = gRows;
        // grid.origin = origin;

        // builtGrid = new Grid(grid);

        Debug.Log("[GridBuilder]: Generated " + gRows + " x " + gColumns + " grid (" + cells.Length + ") tiles" );
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
                    {
                        cells[i, j].isObstacle = true;      // Set obstacle flag to true
                        cells[i,j].IncreaseCost(255);       // Set the cost value to max, so that the cell should not be used in navigation
                    }
                }
            }
        }

        // When creating the cost field, would making the cell's adjacent to these marked obstacles be efficient?
        // I.e, push the enemies towards the non-edge path
    }
    public void CullObstacles()
    {
        GetObstacles();
        MarkGridObstacles();
        Debug.Log("[GridBuilder]: Marked obstructed tiles as unusable");
    }

    public void ClearRemovedList()
    {
        if (ghostCells != null && removeCells != null)
        {
            ghostCells.Clear();
            removeCells.Clear();
        }
    }
    #endregion

    // When the script is attached, validate
    List<Cell> ghostCells;
    Vector3 drawSize;
    void OnValidate()
    {
        // Pre-add grid remove tag, this will remove the elements from the grid (different from obstacles / occupied), theses cells get removed entirely
        //if (gridBlockTags == null)
        //{
        //    gridBlockTags = new List<string>();
        //}
        // if (!gridBlockTags.Contains("gridRemove"))
        //     gridBlockTags.Add("gridRemove");

        if (gridBlockTags == null)
            gridBlockTags = new List<string>();

        if (!gridBlockTags.Contains("Obstacle"))
            gridBlockTags.Add("Obstacle");

        drawSize = new Vector3(cellSize, 0, cellSize);

        if (removeCells == null)
            removeCells = new List<Cell>();

        if (ghostCells == null)
            ghostCells = new List<Cell>();
    }

    public void UpdateGhostCells(List<Cell>update)
    {
        ghostCells = update;
    }
    private void OnDrawGizmosSelected()
    {
        if (ShowCube)
        {
            Gizmos.color = Color.green;
            if (origin != null)
                Gizmos.DrawSphere(origin, 2.0f);
            if (cells != null)
            {
                foreach (Cell c in cells)
                {
                    if (c.isObstacle)
                    {
                        Gizmos.color = Color.magenta;
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }
                    Gizmos.DrawWireCube(c.position, drawSize);
                }
                foreach (Cell c in ghostCells)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(c.position, drawSize);
                }
                foreach (Cell c in removeCells)
                {
                    Gizmos.color = Color.red;
                    drawSize.y = 0.1f;
                    Gizmos.DrawCube(c.position, drawSize);
                }
                //Vector3 cell;
                //if (DrawFullTile)
                //    cell = new Vector3(cellSize, 0, cellSize);
                //else
                //    cell = new Vector3(cellSize / cellSize, 0, cellSize / cellSize);
                //for (int row = 0; row < gRows; row++)
                //{
                //    for (int col = 0; col < gColumns; col++)
                //    {
                //        if (row % 2 == 0)
                //        {
                //            Gizmos.color = Color.blue;
                //            if (col % 2 == 0)
                //                Gizmos.color = Color.red;
                //        }
                //        else
                //        {
                //            Gizmos.color = Color.red;
                //            if (col % 2 == 0)
                //                Gizmos.color = Color.blue;
                //        }
                //        Cell currentCell = cells[row, col];
//
                //        if (removeCells.Contains(currentCell))
                //        {
                //            Gizmos.color = Color.red;
                //            cell.y = 0.1f;
                //            Gizmos.DrawCube(currentCell.position, cell);
                //        }
                //        else
                //        {
                //            if (currentCell.isObstacle)
                //            {
                //                Gizmos.color = Color.gray;
                //                cell.y = 0.1f;
                //            }
                //            Gizmos.DrawWireCube(currentCell.position, cell);
                //        }
                //    }
                //}
            }
        }
    }
    public void SaveGrid()
    {
        grid = ScriptableObject.CreateInstance<GridData>();
        bool gridSaved = false;

        if (!gridSaved)
        {
            List<Cell> tempList = GridUtility.ArrayToList(cells);

            // For every cell in the remove cells list
            if (removeCells != null && removeCells.Count > 0)
            {
                foreach (Cell c in removeCells)
                {
                    // Check if tempList has that matching cell, if it does, remove it from the templist
                    if (tempList.Contains(c))
                    {
                        tempList.Remove(c);
                    }
                }
            }

            // The additional grid information will need some manipulation since the size of the grid has been changed.
            // Need to know the new amount of columns and rows
            // Need to know the new origin point to offset lookups from

            float columns = 0;
            float rows = 0;
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            foreach (Cell c in tempList)
            {
                rows =  Mathf.Max(rows, c.index.x);
                columns = Mathf.Max(columns, c.index.y); 
                minX = Mathf.Min(minX, c.index.x);
                minY = Mathf.Min(minY, c.index.y);
            }
            rows += 1 - minX;      // 0 index
            columns += 1 - minY;

            Cell minCellX = null;
            Cell minCellY = null;
            // Do a lookup to setup the origin at min index.
            foreach (Cell c in tempList)
            {
                if (c.index.x == minX)
                {
                    minCellX = c;
                }
                if (c.index.y == minY)
                {
                    minCellY = c;
                }
                if (minCellX != null && minCellY != null)
                {
                    break;
                }
            }



            // Origin is the offset from the minimum cells

            origin = new Vector3(minCellX.position.x, 0, minCellY.position.z);

            // Reorganize the list so that cells match properly
            // Offset the cells
            foreach (Cell c in tempList)
            {
                c.index.x -= minX;
                c.index.y -= minY;
            }

            // Store the additional grid information
            grid.cellSize = cellSize;
            grid.columns = (int)columns;
            grid.rows = (int)rows;
            grid.origin = origin;

            // Store the tempList into the saved grid
            grid.cellList = tempList;

            gridSaved = true;
        }
        if (gridSaved)
        {
            string path = "Assets/0_Scenes/" + EditorSceneManager.GetActiveScene().name + "_GridData.asset";
            AssetDatabase.CreateAsset(grid, path);
            AssetDatabase.SaveAssets();
            Debug.Log("[GridBuilder]: Saved " + grid.rows + " x " + grid.columns + " grid (" + grid.cellList.Count + ") tiles" );
            Debug.Log("[GridBuilder]: Grid saved to " + path);
        }
    }

    public void MarkClickedCell(Bounds box, bool checking, ref List<Cell> tempList, bool shouldRemove)
    {
        //if (builtGrid != null)
        //{
        //    if (builtGrid.gridCells.)
        //}
        //List<Cell> temp;

        // how can the cell lookup be optimized, get the starting cell and the ending cell, build a box here and get the cells contained in the box
        if (cells != null && cells.Length > 0)
        {
            foreach (Cell c in cells)
            {
                if (!c.boundingBox.Intersects(box))
                {
                    if (tempList.Contains(c))
                        tempList.Remove(c);
                    continue;   // if it does not intersect, continue searching for which do
                }
                else
                {
                    if (!tempList.Contains(c)) // does intersect, add it to the list
                        tempList.Add(c);
                }
            }
        }

        if (checking == false)
        {
            foreach (Cell c in tempList)
            {
                if (shouldRemove)   // Want to remove cells
                {
                    if (!removeCells.Contains(c))   // if the proper list does not contain the same cells as the ones that should be marked
                    {
                        removeCells.Add(c);         // add the marked cells to the list
                    }
                    // Debug.Log("Removed: " + tempList.Count + " cells");
                }
                else                // Want to repair cells
                {
                    if (removeCells.Contains(c))
                    {
                        removeCells.Remove(c);
                    }
                    // Debug.Log("Repaired: " + tempList.Count + " cells");
                }
            }
            tempList.Clear();
        }

        //removeCells = new List<Cell>(tempList);
    }

    // public void LoadToLevel()
    // {
    //     GridData levelGrid = new GridData(grid);
    //     LevelManager.Instance.grid = levelGrid;
    //     Debug.Log("[GridUtility]: Loaded Grid Into Current Level");
    // }
    
}
#endif