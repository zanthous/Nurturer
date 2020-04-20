using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceIndicator : MonoBehaviour
{
    [SerializeField] Direction direction;

    private SpriteRenderer _sr;
    private Color _hoverColor = new Color(255 / 255.0f, 102 / 255.0f, 102 / 255.0f, 150 / 255.0f);
    private Color _normalColor = new Color(255 / 255.0f, 102 / 255.0f, 102 / 255.0f, 67 / 255.0f);

    private LevelData _data;

    public static Action<Vector2Int> TryPlace;
    private Vector2Int _pos;


    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = _normalColor;
        PlayerMovement.PlayerMoved += EnableDisable;
        _data = FindObjectOfType<LevelData>();
        if(direction == Direction.None)
            Debug.Log("Indicator direction not set");

        PanelSelector.ObjectPlaced += EnableDisableWrapper;
    }

    private void EnableDisableWrapper()
    {
        EnableDisable(_pos, gameObject);
    }

    void Start()
    {
        //todo un-hard code
        EnableDisable(new Vector2Int(0,1), gameObject);
    }

    void OnDestroy()
    {
        PlayerMovement.PlayerMoved += EnableDisable;
        PanelSelector.ObjectPlaced -= EnableDisableWrapper;
    }

    void OnMouseOver()
    {
        _sr.color = _hoverColor;
        if(Input.GetMouseButtonDown(0))
        {
            TryPlace?.Invoke(_pos + Dir.dir[direction]);
        }
    }

    void OnMouseExit()
    {
        _sr.color = _normalColor;
    }

    private void EnableDisable(Vector2Int pos, GameObject go)
    {
        _pos = pos;

        if(_data.CanPlaceObject(pos + Dir.dir[direction]))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
