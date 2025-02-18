using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData : ScriptableObject
{
    //public Cell[,] cells;
    public List<Cell> cellList;
    public Cell[,] cells;
    public Vector3 origin;
    public int rows;
    public int columns;
    public int cellSize;

    public GridData()
    {
        cells = null;
        origin = Vector3.zero;
        rows = -1;
        columns = -1;
        cellSize = -1;
    }
    public GridData(GridData copy)
    {
        CopyGrid(copy);
    }
    public void CopyGrid(GridData copy)
    {
        cells = copy.cells;
        origin = copy.origin;
        rows = copy.rows;
        columns = copy.columns;
        cellSize = copy.cellSize;
    }

    // Need methods to convert to a 2D array and a List, vice-versa
    public void LoadFromList()
    {
        cells = ArrayFromList(cellList);
        Debug.Log(cells.Length);
    }
    public void LoadFromList(List<Cell> list)
    {
        cells = ArrayFromList(list);
    }
    public List<Cell> ArrayToList(Cell[,] array)
    {
        List<Cell> temp = new List<Cell>();

        foreach (Cell c in array)
        {
            temp.Add(c);
        }
        return temp;
    }

    public Cell[,] ArrayFromList(List<Cell> list)
    {
        Cell[,] array = new Cell[rows,columns];

        foreach(Cell c in list)
        {
            array[c.x, c.y] = c;
        }
        return array;
    }
}
