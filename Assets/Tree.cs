using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : WorldObject
{
    private LineRenderer _line;

    [SerializeField] private Sprite _unpoweredSprite;
    [SerializeField] private Sprite _poweredSprite;

    //inherited
    //protected Vector2Int _position;
    //protected ObjectType _type; 
    //protected LevelData _data;

    //idk if I need this yet
    private List<Tree> _connections = new List<Tree>();
    private bool _powered = false;
    private SpriteRenderer _sr;
    private Vector2Int _poweredFrom;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        Player.TryInteract += AddEnergy;
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        Player.TryInteract -= AddEnergy;
    }

    void Start()
    {
        OnPlace();
    }

    private void OnPlace()
    {
        //has just been placed
        var trees = Data.GetObjectsOfType(ObjectType.Tree);

        //set flower connections
        if((_data.flower.Position.x == _position.x ||
            _data.flower.Position.y == _position.y) && !_data.IsObstructed(Position, Data.flower.Position))
        {
            _data.RegisterTreeConnection(Position, _data.flower.Position);
            _data.flower.TreeConnect?.Play();
        }

        UpdateConnections(trees);

        //power based on flower
        if((_data.flower.Position.x == _position.x ||
            _data.flower.Position.y == _position.y) && !_data.IsObstructed(Position, Data.flower.Position))
        {
            SetPowered(true, _data.flower.Position);
            return;
        }

        //power trees based on trees
        foreach(Tree t in trees)
        {
            if(t == this)
                continue;

            if(t.Position.x == _position.x && !_data.IsObstructed(Position, t.Position))
            {
                if(t._powered)
                {
                    _data.RegisterTreeConnection(Position, t.Position);
                    _data.flower.TreeConnect?.Play();
                    SetPowered(true, t._position);
                    return;
                }
            }
            else if(t.Position.y == _position.y && !_data.IsObstructed(Position, t.Position))
            {
                if(t._powered)
                {
                    _data.RegisterTreeConnection(Position, t.Position);
                    _data.flower.TreeConnect?.Play();
                    SetPowered(true, t._position);
                    return;
                }
            }
        }

    }

    public void SetPowered(bool powered, Vector2Int poweredFrom)
    {
        _powered = powered;
        
        if(powered)
        {
            _sr.sprite = _poweredSprite;
            _poweredFrom = poweredFrom;
            DrawPowerLine();
        }
        else
        {
            _sr.sprite = _unpoweredSprite;
        }

        AlertConnections(powered);
    }

    private void DrawPowerLine()
    {
        _line.enabled = true;
        _line.positionCount = 2;
        _line.material.SetColor("_TintColor", new Color(1, 0, 0, .25f));
        Direction dir = Direction.None;
        if(_poweredFrom.x==Position.x)
        {
            if(_poweredFrom.y > Position.y)
            {
                dir = Direction.Up;
            }
            else
            {
                dir = Direction.Down;
            }
        }
        else if (_poweredFrom.y == Position.y)
        {
            if(_poweredFrom.x > Position.x)
            {
                dir = Direction.Right;
            }
            else
            {
                dir = Direction.Left;
            }
        }
        if(dir == Direction.None)
        {
            Debug.Log("Direction is none when it shouldn't be");
        }
        Vector2 pos1 = Position;
        Vector2 pos2 = _poweredFrom;
        switch(dir)
        {
            case Direction.Up:
                pos1.y += 1.0f;
                pos1.x += 0.5f;
                pos2.y += 0.0f;
                pos2.x += 0.5f;
                break;
            case Direction.Right:
                pos1.y += 0.5f;
                pos1.x += 1.0f;
                pos2.y += 0.5f;
                pos2.x += 0.0f;
                break;
            case Direction.Down:
                pos1.y += 0.0f;
                pos1.x += 0.5f;
                pos2.y += 1.0f;
                pos2.x += 0.5f;
                break;
            case Direction.Left:
                pos1.y += 0.5f;
                pos1.x += 0.0f;
                pos2.y += 0.5f;
                pos2.x += 1.0f;
                break;
        }
        var points = new[] { new Vector3(pos1.x, pos1.y, 0), new Vector3(pos2.x, pos2.y, 0) };

        _line.SetPositions(points);

    }

    private void AlertConnections(bool powered)
    {
        if(!powered)
        { 
            throw new NotImplementedException();
        }
        else
        {
            foreach(Tree t in _connections)
            {
                if(t == this)
                    continue;

                if(!t._powered)
                {
                    t.UpdateConnections(Data.GetObjectsOfType(ObjectType.Tree));
                    _data.RegisterTreeConnection(Position, t.Position);
                    t.SetPowered(true, _position);
                }
            }
        }
    }

    private void UpdateConnections(List<WorldObject> trees)
    {

        //set tree connections
        foreach(Tree t in trees)
        {
            if(t == this)
                continue;

            if(t.Position.x == _position.x && !_data.IsObstructed(Position, t.Position))
            {
                if(!_connections.Contains(t))
                    _connections.Add(t);
            }
            else if(t.Position.y == _position.y && !_data.IsObstructed(Position, t.Position))
            {
                if(!_connections.Contains(t))
                    _connections.Add(t);
            }
        }
    }

    public void AddEnergy(Vector2Int pos, Direction facing, HoldableObject ho, Player player)
    {
        if(!_powered)
            return;
        if(pos + Dir.dir[facing] == _position && ho == HoldableObject.Energy)
        {
            //destroy energy
            player.Drop(true);
            _data.flower._energyDeposited++;
            _data.flower.TransactEnergy(Globals.ENERGY_HEAL_AMOUNT);
            _data.flower.EnergyFill?.Play();
        }
    }
}
