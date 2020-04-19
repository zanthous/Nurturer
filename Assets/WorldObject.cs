using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    protected Vector2Int _position;
    protected ObjectType _type;
    protected LevelData _data;

    public Vector2Int Position => _position;
    public ObjectType Type => _type;
    public LevelData Data => _data;


    public void Init(Vector2Int pos, ObjectType type_in, LevelData data)
    {
        _type = type_in;
        _position = pos;
        _data = data;
    }
}
