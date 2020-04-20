using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBlossom : WorldObject
{
    private float _flowerHealth = 40;
    private int _totalSteps = 0;
    private float _healthPerStep = 1.0f;
    private int _difficultyIncreaseSteps = 150;
    private float _difficultyIncreaseAmount = 0.5f;
    public int Divisions = 0;

    [SerializeField] private AudioSource step1;
    [SerializeField] private AudioSource step2;

    //out of time idc
    [SerializeField] public AudioSource TreeConnect;
    [SerializeField] public AudioSource PlaceObject;
    [SerializeField] public AudioSource EnergyFill;
    [SerializeField] public AudioSource EnergyPickup;


    public float Health => _flowerHealth;

    public static Action TimeTick;


    void Awake()
    {
        _position = new Vector2Int(0, 0);
        Player.TryInteract += AddEnergy;
        PlayerMovement.PlayerMoved += UpdateFlowerHealthOnStep;
    }

    public void TransactEnergy(float amount)
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
            _data.flower.EnergyFill?.Play();
        }
    }

    public void UpdateFlowerHealthOnStep(Vector2Int position, GameObject gameObject)
    {
        _totalSteps++;
        Divisions = _totalSteps / _difficultyIncreaseSteps;
        float difficultyMod = (Divisions * _difficultyIncreaseAmount);
        if(!_data.IsPoweredTile(position))
        { 
            _flowerHealth -= (_healthPerStep + difficultyMod);
            TimeTick?.Invoke();
            step1?.Play();
        }
        else
        {
            step2?.Play();
        }

        if(_flowerHealth <= 0)
            GameManager.GameOver?.Invoke();
    }


}
