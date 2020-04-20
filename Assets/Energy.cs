using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : WorldObject, ICollidable, IHoldable
{
    //inherited 
    //protected Vector2Int _position;
    //protected ObjectType _type;
    //protected LevelData _data;

    private bool _attached = false;

    public void Attach()
    {
        _attached = true;
        //remove from leveldata
        _data.RemoveFromWorldObjects(_position);
    }

    public void Detach(Vector2Int playerPosition)
    {
        _attached = false;

        //add to leveldata, or do something else
        _data.AddToWorldObjects(_position, this, ObjectType.Energy);
    }

    public void OnCollision(Vector2Int pos, GameObject other)
    {
        _position = pos;
        if(_attached)
        {
            Debug.Log("Colliding while already held");
            return;
        }
            
        if(other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if(!player.Holding)
            {
                Attach();
                player.Hold(gameObject, HoldableObject.Energy);
                _data.flower.EnergyPickup?.Play();
            }
        }
    }
}
