using UnityEngine;

[System.Serializable]
public class Cell 
{
    public Vector2Int index;
    //public int x;
    //public int y;
    [HideInInspector] public Vector3 position;    // this is the centre of the cell in gamespace
    [HideInInspector] public Bounds boundingBox;  // this is the size of the cell
    [HideInInspector] public bool isObstacle = false;
    [HideInInspector] public bool isOccupied = false;
    public Cell()
    {
        index = new Vector2Int(-1,-1);
    }
    public Cell(int x, int y)
    {
        index = new Vector2Int(x, y);
    }

    public Cell (Vector2Int index)
    {
        this.index = index;
    }
}