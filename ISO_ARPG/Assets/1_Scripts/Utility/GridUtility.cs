using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridUtility : MonoBehaviour
{
    public static bool CheckPointInCell(Vector3 point, Cell toCheck)
    {
        return toCheck.boundingBox.Contains(point);
    }
    public static bool PointInBounds(GridData grid, Vector3 point)
    {
        float width = grid.rows * grid.cellSize;
        float height = grid.columns * grid.cellSize;

        return ((point.x >= grid.origin.x) && (point.x <= (grid.origin.x + width)) &&
                (point.z <= grid.origin.z) && (point.z >= (grid.origin.z - height)));
    }
    public static CellIndex GetIndexFromPoint(GridData grid, Vector3 pos)
    {
        // SHOULD ADD CHECK TO PREVENT A SEARCH POS IN NON PLAYSPACE
        //Debug.Log(PointInBounds(pos));
        if (PointInBounds(grid, pos))
        {
            pos -= grid.origin; // gets the difference between the point and the origin

            // start counting the rows and columns from the grid point
            int row = (int)(pos.x / grid.cellSize);
            int col = (int)(pos.z / grid.cellSize);

            // based on where the origin is, normalize the values to be non-negative
            if (grid.origin.x < 0)
                col *= -1;
            else
                row *= -1;

            return new CellIndex(row, col);
        }
        return new CellIndex(-1, -1); // return a vector with an INVALID location in the array
    }

    public static CellIndex GetRandomIndex(GridData grid, int tryCycles)
    {
        // Only return valid indices to use, valid indicies are
        // Unoccupied
        // Not obstacles
        int count = 0;
        do
        {
            count++;
            int row = Random.Range(0, grid.columns);
            int col = Random.Range(0, grid.rows);

            // If the cell is not an obstacle or occupied, use that cell
            if (!(grid.cells[row, col].isObstacle) || !grid.cells[row, col].isOccupied)
            {
                return new CellIndex(row, col);
            }
        }
        while (count <= tryCycles);
        return new CellIndex(-1, -1);
    }

    // Get the cell given two int values
    public static Cell GetCellFromIndex(GridData grid, int x, int y)
    {
        if (x != -1 && y != -1)
        {
            return grid.cells[x, y];
        }
        return null;
    }

    // Get the cell given a package of 2 int values
    public static Cell GetCellFromIndex(GridData grid, CellIndex packIndex)
    {
        if (packIndex.x != -1 && packIndex.y != -1)
        {
            return grid.cells[packIndex.x, packIndex.y];
        }
        return null;
    }

        // GRID SYSTEM
}
