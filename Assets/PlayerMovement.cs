using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    None,
    Up,
    Right,
    Down,
    Left
}

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private Tilemap _foreground;

    private float _timeSinceLastMove = 0.0f;
    private float _moveInterval = .04f;
    private bool _moved = false;

    public static Action<Vector2Int, GameObject> PlayerMoved;
    private Vector2Int _position = new Vector2Int(0, 0);
    private LevelData _data;

    private Direction _facing;
    public Direction Facing => _facing;

    public Vector2Int Position => _position;

    void Awake()
    {
        _data = FindObjectOfType<LevelData>();
        _position = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        _timeSinceLastMove += Time.deltaTime;
        var moveDir = Direction.None;
        if(Input.GetKeyDown(KeyCode.W))
        {
            moveDir = Direction.Up;
        }
        else if(Input.GetKeyDown(KeyCode.A)) 
        {
            moveDir = Direction.Left;
        }
        else if(Input.GetKeyDown(KeyCode.S)) 
        {
            moveDir = Direction.Down;
        }
        else if(Input.GetKeyDown(KeyCode.D)) 
        {
            moveDir = Direction.Right;
        }

        if(_timeSinceLastMove > _moveInterval && moveDir != Direction.None)
        {
            TryMove(moveDir);
            return;
        }
    }

    private void TryMove(Direction dir)
    {
        _moved = false;
        switch(dir)
        {
            case Direction.None:
                Debug.Log("Stop trying to move nowhere");
                return;
            case Direction.Up:
                _facing = Direction.Up;
                if(_position.y == 99)
                    break;
                if(_data.IsValidMove(_position + new Vector2Int(0, 1)))
                { 
                    _position += new Vector2Int(0, 1);
                    _timeSinceLastMove = 0.0f;
                    _moved = true;
                }
                break;
            case Direction.Right:
                _facing = Direction.Right;
                if(_position.x == 99)
                    break;
                if(_data.IsValidMove(_position + new Vector2Int(1, 0)))
                { 
                    _position += new Vector2Int(1, 0);
                    _timeSinceLastMove = 0.0f;
                    _moved = true;
                }
                break;
            case Direction.Down:
                _facing = Direction.Down;
                if(_position.y == -100)
                    break;
                if(_data.IsValidMove(_position + new Vector2Int(0, -1)))
                { 
                    _position += new Vector2Int(0, -1);
                    _timeSinceLastMove = 0.0f;
                    _moved = true;
                }
                break;
            case Direction.Left:
                _facing = Direction.Left;
                if(_position.x == -100)
                    break;
                if(_data.IsValidMove(_position + new Vector2Int(-1, 0)))
                {
                    _position += new Vector2Int(-1, 0);
                    _timeSinceLastMove = 0.0f;
                    _moved = true;
                }                    
                break;
        }
        //player moved
        if(_moved)
        { 
            transform.position = new Vector3(_position.x + .5f, _position.y + .5f, 0);
            PlayerMoved.Invoke(_position, gameObject);
        }
    }
}