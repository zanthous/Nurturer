using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidable 
{
    void OnCollision(Vector2Int pos, GameObject other);
}
