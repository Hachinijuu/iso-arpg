using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridDirection
{
    public readonly Vector2Int vector;
    public static implicit operator Vector2Int(GridDirection direction)
    {
        return direction.vector;
    }
    private GridDirection(int x, int y)
    {
        vector = new Vector2Int(x, y);
    }

    // Origin is 0,0 top left, these directions need to be relative to that
    public static readonly GridDirection None = new     GridDirection( 0,   0);

    // Cardinal Directions
    public static readonly GridDirection North = new    GridDirection( 0,  1);
    public static readonly GridDirection South = new    GridDirection(0,   -1);
    public static readonly GridDirection East = new     GridDirection(-1,   0);
    public static readonly GridDirection West = new     GridDirection(1,   0);

    // Diagonal Directions
    public static readonly GridDirection NorthEast = new    GridDirection(-1,  1);
    public static readonly GridDirection NorthWest = new    GridDirection(1,  1);
    public static readonly GridDirection SouthEast = new    GridDirection(-1,  -1);
    public static readonly GridDirection SouthWest = new   GridDirection(1,  -1);

    // Cardinal Directions
    // public static readonly GridDirection North = new    GridDirection( 1,  0);
    // public static readonly GridDirection South = new    GridDirection( -1,   1);
    // public static readonly GridDirection East = new     GridDirection(0,   -1);
    // public static readonly GridDirection West = new     GridDirection( 0,   1);

    // // Diagonal Directions
    // public static readonly GridDirection NorthEast = new    GridDirection( 1,  -1);
    // public static readonly GridDirection NorthWest = new    GridDirection(  1,  1);
    // public static readonly GridDirection SouthEast = new    GridDirection( -1,  -1);
    // public static readonly GridDirection SouthWest = new   GridDirection(  -1,  1);

    public static GridDirection GetDirectionFromIndex(Vector2Int vectorIndex)
    {
        return Directions.DefaultIfEmpty(None).FirstOrDefault(direction => direction == vectorIndex);
    }
    public static readonly List<GridDirection> CardinalDirections = new List<GridDirection>
    {
        North, East, South, West
    };

    public static readonly List<GridDirection> Directions = new List<GridDirection>
    {
        North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
    };

    public static readonly List<GridDirection> AllDirections = new List<GridDirection>
    {
        None, North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
    };
}
