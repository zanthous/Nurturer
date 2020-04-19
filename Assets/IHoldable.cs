using UnityEngine;

public interface IHoldable
{
    void Attach();
    void Detach(Vector2Int playerPosition);
}