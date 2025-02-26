using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData : ScriptableObject
{
    // GRID DATA IS THE INFORMATION SAVED FROM THE GRID THAT IS BUILT IN ENGINE
    // THIS WILL ONLY BE USED TO CONTAIN THE SAVED INFORMATION, EACH GRID NEEDED BY EACH OF THE SYSTEMS WILL HAVE TO UNPACK THE GRID DATA
    //public Cell[,] cells;
    public List<Cell> cellList;
    //public Cell[,] cells;
    public Vector3 origin;
    public int rows;
    public int columns;
    // public Vector2Int width;
    // public Vector2Int height;
    public int cellSize;
    public GridData()
    {
        cellList = null;
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
        cellList = new List<Cell>(cellList);
        origin = copy.origin;
        rows = copy.rows;
        columns = copy.columns;
        cellSize = copy.cellSize;
    }

    // // Need methods to convert to a 2D array and a List, vice-versa
    // public void LoadFromList()
    // {
    //     cells = ArrayFromList(cellList);
    //     Debug.Log(cells.Length);
    // }
    // public void LoadFromList(List<Cell> list)
    // {
    //     cells = ArrayFromList(list);
    // }
    // public List<Cell> ArrayToList(Cell[,] array)
    // {
    //     List<Cell> temp = new List<Cell>();

    //     foreach (Cell c in array)
    //     {
    //         temp.Add(c);
    //     }
    //     return temp;
    // }

    // public Cell[,] ArrayFromList(List<Cell> list)
    // {
    //     Cell[,] array = new Cell[columns,rows];

    //     foreach(Cell c in list)
    //     {
    //         array[c.x, c.y] = c;
    //     }
    //     return array;
    // }
}
