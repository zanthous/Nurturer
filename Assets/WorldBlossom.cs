using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBlossom : WorldObject
{
    private int _flowerHealth = 40;

    public int Health => _flowerHealth;

    void Awake()
    {
        _position = new Vector2Int(0, 0);
        Player.TryInteract += AddEnergy;
        PlayerMovement.PlayerMoved += UpdateFlowerHealthOnStep;
    }

    public void TransactEnergy(int amount)
    {
        _flowerHealth += amount;
    }

    void OnDestroy()
    {
        Player.TryInteract -= AddEnergy;
        PlayerMovement.PlayerMoved -= UpdateFlowerHealthOnStep;
    }

    public void AddEnergy(Vector2Int pos, Direction facing, HoldableObject ho, Player player)
    {
        if(pos + Dir.dir[facing] == _position && ho == HoldableObject.Energy)
        {
            //destroy energy
            player.Drop(true);
            _flowerHealth += Globals.ENERGY_HEAL_AMOUNT;
        }
    }

    public void UpdateFlowerHealthOnStep(Vector2Int position, GameObject gameObject)
    {
        if(!_data.IsPoweredTile(position))
        { 
            _flowerHealth--;
        }

        if(_flowerHealth <= 0)
            GameManager.GameOver?.Invoke();
    }


}
