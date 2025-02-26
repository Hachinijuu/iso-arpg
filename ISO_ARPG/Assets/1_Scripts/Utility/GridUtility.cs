using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// THIS SCRIPT CONTAINS THE GRID CLASS, A CONTAINER FOR GRID'S

public class Grid
{
    // GridData will be unpacked into this class
    // The unpacked information will likely be ONLY the relevant, playable cells, which the game can be played from
    // The grid class contains a public dictionary and any extended functionality for customized lookups that do not rely on the static grid util
    public Dictionary<Vector2Int, Cell> gridCells;
    public Vector3 origin;
    public int rows;
    public int columns;
    public int cellSize;
    // The index of the cell will be stored as keys for more efficient lookups
    public Grid(GridData data)
    {
        origin = data.origin;
        rows = data.rows;
        columns = data.columns;
        cellSize = data.cellSize;
        gridCells = new Dictionary<Vector2Int, Cell>();
        LoadGrid(data);
    }
    public void LoadGrid(GridData toLoad)
    {
        if (toLoad != null)
        {
            // if (gridCells == null && gridCells.Count < 0)
            // {
            //     gridCells = new Dictionary<Vector2Int, Cell>();
            // }
            if (gridCells != null)
            {
                foreach (Cell c in toLoad.cellList)
                {
                    gridCells.Add(c.index, c);
                }
            }
        }
    }
    public void DeleteGrid()
    {
        foreach (KeyValuePair<Vector2Int, Cell> gCell in gridCells)
        {
            gridCells.Remove(gCell.Key);
        }
    }
}

// IT ALSO CONTAINS GRID UTILITY, A CLASS THAT ACTS AS AN STATIC INTERFACE TO CALL FUNCTIONS GIVE VALID GRID INFORMATION

public class GridUtility
{
    #region Grid Packing and Unpacking
    // Array to List
    public static List<Cell> ArrayToList(Cell[,] array)
    {
        List<Cell> cellList = new List<Cell>();
        foreach(Cell c in array)
        {
            cellList.Add(c);
        }
        return cellList;
    }
    #endregion

    public static bool CheckPointInCell(Vector3 point, Cell toCheck)
    {
        return toCheck.boundingBox.Contains(point);
    }
    public static bool PointInBounds(Grid grid, Vector3 point)
    {
        float width = grid.rows * grid.cellSize;
        float height = grid.columns * grid.cellSize;

        return ((point.x >= grid.origin.x) && (point.x <= (grid.origin.x + width)) &&
                (point.z <= grid.origin.z) && (point.z >= (grid.origin.z - height)));
    }
    public static Vector2Int GetIndexFromPoint(Grid grid, Vector3 pos)
    {
        // SHOULD ADD CHECK TO PREVENT A SEARCH POS IN NON PLAYSPACE
        //Debug.Log(PointInBounds(pos));
        if (PointInBounds(grid, pos))
        {
            pos -= grid.origin; // gets the difference between the point and the origin

            // start counting the rows and columns from the grid point
            int x = (int)(pos.z / grid.cellSize);
            int y = (int)(pos.x / grid.cellSize);

            // based on where the origin is, normalize the values to be non-negative
            if (grid.origin.x < 0)
                x *= -1;
            else
                y *= -1;

            return new Vector2Int(x, y);
        }
        return new Vector2Int(-1, -1); // return a vector with an INVALID location in the array
    }

    public static Cell GetCellFromPoint(Grid grid, Vector3 pos)
    {
        Cell foundCell = null;
        if (PointInBounds(grid, pos))
        {
            Vector2Int index = GetIndexFromPoint(grid, pos);
            if (index.x != -1 && index.y != -1)
            {
                foundCell = GetCellFromIndex(grid, index);
            }
        }
        return foundCell;
    }

    public static Vector2Int GetRandomIndex(Grid grid, int tryCycles)
    {
        // Only return valid indices to use, valid indicies are
        // Unoccupied
        // Not obstacles
        int count = 0;
        do
        {
            count++;
            int row = Random.Range(0, grid.rows);
            int col = Random.Range(0, grid.columns);
            Vector2Int randIndex = new Vector2Int(row, col); 

            if (grid.gridCells.TryGetValue(randIndex, out Cell c))  // If the grid has that index available
            {
                //Cell c = grid.gridCells[randIndex];
                if (c != null)
                {
                    if (!c.isObstacle && !c.isOccupied) // if the cell is not an obstacle and not occupied, return the cell
                    {
                        return randIndex;
                    }
                }
                // Check if the cell is an obstacle, or if it is occupied
            }
            // If the cell is not an obstacle or occupied, use that cell
            
            //if (!(grid.cells[row, col].isObstacle) || !grid.cells[row, col].isOccupied)
            //{
            //    return new Vector2Int(row, col);
            //}
        }
        while (count <= tryCycles);
        return new Vector2Int(-1, -1);
    }

    // // Get the cell given two int values
    public static Cell GetCellFromIndex(Grid grid, int x, int y)
    {
        if (grid.gridCells.TryGetValue(new Vector2Int(x, y), out Cell c))
            return c;
        return null;
    
        //if (x != -1 && y != -1)
        //{
        //    return grid.cells[x, y];
        //}
        //return null;
    }

    // // Get the cell given a package of 2 int values
    public static Cell GetCellFromIndex(Grid grid, Vector2Int index)
    {
        return GetCellFromIndex(grid, index.x, index.y);
        // if (packIndex.x != -1 && packIndex.y != -1)
        // {
        //     return grid.cells[packIndex.x, packIndex.y];
        // }
        // return null;
    }

    public static Cell GetRandomCell(Grid grid, int tryCycles)
    {
        Vector2Int index = GetRandomIndex(grid, tryCycles);
        Cell c = GetCellFromIndex(grid, index);
        return c;
    }
}
