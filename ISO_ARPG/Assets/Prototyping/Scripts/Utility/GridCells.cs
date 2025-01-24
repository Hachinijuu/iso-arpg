using UnityEngine;
public struct CellIndex
{
    // Custom container for index, uses INT instead of Vector2 float
    public int x;
    public int y;

    public CellIndex(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

}

[System.Serializable]
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