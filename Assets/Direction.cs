using System.Collections.Generic;
using UnityEngine;

class Dir
{
    public static Dictionary<Direction, Vector2Int> dir = new Dictionary<Direction, Vector2Int>()
    {
        { Direction.None, new Vector2Int() },
        { Direction.Up, new Vector2Int(0,1) },
        { Direction.Right, new Vector2Int(1,0) },
        { Direction.Down, new Vector2Int(0,-1) },
        { Direction.Left, new Vector2Int(-1,0) }
    };
}
