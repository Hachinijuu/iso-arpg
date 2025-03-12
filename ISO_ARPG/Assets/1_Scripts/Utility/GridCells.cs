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
    
    // Navigation variables
    public byte cost;   // the cost of the cell for movement
    public ushort bestCost;
    public GridDirection bestDirection;
    
    // Constructors
    public Cell()
    {
        index = new Vector2Int(-1,-1);
        InitCost();
        bestDirection = GridDirection.None;
    }
    public Cell(int x, int y)
    {
        index = new Vector2Int(x, y);
        InitCost();
        bestDirection = GridDirection.None;
    }

    public Cell (Vector2Int index)
    {
        this.index = index;
        InitCost();
        bestDirection = GridDirection.None;
    }

    public Cell (Cell copy)
    {
        index = copy.index;
        cost = copy.cost;
        bestCost = copy.bestCost;
        bestDirection = copy.bestDirection;
    }

    public void InitCost()
    {
        // reset the cell's values
        cost = 1;
        bestCost = ushort.MaxValue;
    }

    // Functionality
    public void IncreaseCost(int amount)
    {
        if (cost == byte.MaxValue)
        {
            return;
        }
        if (amount + cost >= 255)
        {
            cost = byte.MaxValue;
        }
        else
        {
            cost += (byte)amount;
        }
    }
}