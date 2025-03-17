using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Flowfield navigation system is based on Turbo Makes Games pathfinding
// It has been adapted to match the internal grid system structure.

public class Flowfield
{
    // The flowfield will access the established grid that is loaded into the level / referenced already
    // This will allow for the base grid to be created and iterated upon, with the obstacles defined already.
    // Spawned obstacles will have to mark themsleves to increase avoidance values

    // Flowfield can exist as non monobehaviour and be plugged into AIManager for navigation purposes

    private Grid defaultGrid;
    public Grid grid;
    public Cell destination;

    // Constructors
    public Flowfield()
    {
        grid = null;
        destination = null;
    }
    public Flowfield(Grid grid, Cell dest)
    {
        defaultGrid = grid;
        this.grid = new Grid(defaultGrid); // Create a new grid instead of referencing the existing one
        // Why create a new grid? So that the default cell costs are not overridden.
        destination = dest;
    }

    public void InitFlowfield()
    {
        this.grid = new Grid(defaultGrid);
    }

    // Functionality
    //public void InitFlowfield()
    //{
    //    foreach (KeyValuePair<Vector2Int, Cell> pair in grid.gridCells)
    //    {
    //        if (pair.Value.cost <= 0)
    //        {
    //            pair.Value.cost = 1;
    //        }
    //    }
    //}
    public void Reset(Grid rebuild)
    {
        grid = rebuild;
    }
    public void CreateIntergrationField(Cell dest)
    {
        if (dest == null)
            return;
        destination = dest;

        destination.cost = 0;
        destination.bestCost = 0;

        Queue<Cell> cellsToCheck = new Queue<Cell>();

        cellsToCheck.Enqueue(destination);

        while (cellsToCheck.Count > 0)
        {
            Cell currCell = cellsToCheck.Dequeue();
            List<Cell> neighbours = GetNeighbourCells(currCell.index, GridDirection.CardinalDirections);
            foreach (Cell neighbour in neighbours)
            {
                if (neighbour.cost == byte.MaxValue)    // If the cell is an obstacle / impassible
                {
                    continue;                           // Continue looking at the other neighbours
                }
                if (neighbour.cost + currCell.bestCost < neighbour.bestCost)
                {
                    // If the cost of the next cell plus the cost of this cell, is less than the neighbour's cost
                    // Best cost is the max value allowed
                    neighbour.bestCost = (ushort)(neighbour.cost + currCell.bestCost);
                    cellsToCheck.Enqueue(neighbour);
                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach (KeyValuePair<Vector2Int, Cell> cellPair in grid.gridCells)
        {
            List<Cell> neighbours = GetNeighbourCells(cellPair.Key, GridDirection.AllDirections);
            int bestCost = cellPair.Value.bestCost;

            foreach (Cell neighbour in neighbours)
            {
                if (neighbour.bestCost < bestCost)  // If the neighbour value is smaller than the current best cost, we want to use this flow (direction)
                {
                    bestCost = neighbour.bestCost;
                    //Vector2Int origin = new Vector2Int((int)grid.origin.x, (int)grid.origin.z);
                    cellPair.Value.bestDirection = GridDirection.GetDirectionFromIndex(neighbour.index - cellPair.Value.index);
                }
            }
        }
    }

    private List<Cell> GetNeighbourCells(Vector2Int index, List<GridDirection> directions)
    {
        List<Cell> neighbours = new List<Cell>();

        foreach (Vector2Int dir in directions)
        {
            Cell neighbour = GetRelativeCellIndex(index, dir);
            if (neighbour != null)
                neighbours.Add(neighbour);
        }
        return neighbours;
    }

    private Cell GetRelativeCellIndex(Vector2Int origin, Vector2Int relative)
    {
        Vector2Int finalPos = origin + relative;
        if (finalPos.x < 0 || finalPos.x >= grid.rows || finalPos.y < 0 || finalPos.y >= grid.columns)
            return null;
        else 
            return GridUtility.GetCellFromIndex(grid, finalPos);
    }
}
