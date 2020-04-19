using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldableObject
{
    None,
    Energy
}
public enum PlayerState
{
    Idle,
    Holding
}


public class Player : MonoBehaviour
{
    private bool _holding;
    public bool Holding => _holding;

    private HoldableObject _holdingType;
    private GameObject _holdingObject;
    private PlayerMovement _movement;

    private SpriteRenderer _sr;

    [SerializeField] private List<Sprite> _golemDown;
    [SerializeField] private List<Sprite> _golemUp;
    [SerializeField] private List<Sprite> _golemRight;
    [SerializeField] private List<Sprite> _golemLeft;

    [SerializeField] private List<Sprite> _golemHoldingDown;
    [SerializeField] private List<Sprite> _golemHoldingRight;
    [SerializeField] private List<Sprite> _golemHoldingLeft;
    [SerializeField] private List<Sprite> _golemHoldingUp;

    private int _animationFrame = 0;
    private float _animationFrameTime = .7f;
    private float _animationTimer = 0.0f;

    private PlayerState _state = PlayerState.Idle;
    private Direction _facingLastFrame = Direction.None;

    public static Action<Vector2Int, Direction, HoldableObject, Player> TryInteract;

    void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _sr = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Animate();

        Controls();

        AdjustHeldObject(false);
    }

    private void AdjustHeldObject(bool forceUpdate)
    {
        if((_holding && _facingLastFrame != _movement.Facing) || forceUpdate)
        {
            var lpos = _holdingObject.transform.localPosition;
            if(_movement.Facing == Direction.Down)
            {
                _holdingObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
                _holdingObject.transform.localPosition = new Vector3();
            }
            else if(_movement.Facing == Direction.Up)
            {
                _holdingObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                _holdingObject.transform.localPosition = new Vector3();
            }
            else if(_movement.Facing == Direction.Left)
            {
                _holdingObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                _holdingObject.transform.localPosition = new Vector3(-.15f, lpos.y, 0);
            }
            else if(_movement.Facing == Direction.Right)
            {
                _holdingObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
                _holdingObject.transform.localPosition = new Vector3(.15f, lpos.y, 0);
            }
        }
        _facingLastFrame = _movement.Facing;
    }

    private void Animate()
    {
        _animationTimer += Time.deltaTime;
        if(_animationTimer > _animationFrameTime)
        { 
            _animationFrame = (_animationFrame + 1) % 2;
            _animationTimer = 0.0f;
        }

        switch(_state)
        {
            case PlayerState.Idle:
                switch(_movement.Facing)
                {
                    case Direction.None:
                        _sr.sprite = _golemDown[_animationFrame];
                        break;
                    case Direction.Up:
                        _sr.sprite = _golemUp[_animationFrame];
                        break;
                    case Direction.Right:
                        _sr.sprite = _golemRight[_animationFrame];
                        break;
                    case Direction.Down:
                        _sr.sprite = _golemDown[_animationFrame];
                        break;
                    case Direction.Left:
                        _sr.sprite = _golemLeft[_animationFrame];
                        break;
                }
                break;
            case PlayerState.Holding:
                switch(_movement.Facing)
                {
                    case Direction.None:
                        _sr.sprite = _golemHoldingDown[_animationFrame];
                        break;
                    case Direction.Up:
                        _sr.sprite = _golemHoldingUp[_animationFrame];
                        break;
                    case Direction.Right:
                        _sr.sprite = _golemHoldingRight[_animationFrame];
                        break;
                    case Direction.Down:
                        _sr.sprite = _golemHoldingDown[_animationFrame];
                        break;
                    case Direction.Left:
                        _sr.sprite = _golemHoldingLeft[_animationFrame];
                        break;
                }
                break;
        }
    }

    private void Controls()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            TryInteract?.Invoke(_movement.Position, _movement.Facing, _holdingType, this);
        }
    }

    public void Hold(GameObject go, HoldableObject holdableType)
    {
        //update animation to holding animation
        go.transform.parent = transform;
        go.transform.localPosition = new Vector3(0, 0, 0);
        _holdingType = holdableType;
        _holdingObject = go;
        var h = go.GetComponent<IHoldable>();
        h.Attach();
        _holding = true;
        _state = PlayerState.Holding;
        AdjustHeldObject(true);
    }

    public void Drop(bool consumed)
    {
        if(!consumed)
        {
            _holdingObject.GetComponent<IHoldable>().Detach(_movement.Position);
            _holding = false;
        }
        else
        {
            Destroy(_holdingObject);
            _holding = false;
        }
        _state = PlayerState.Idle;
    }

}
