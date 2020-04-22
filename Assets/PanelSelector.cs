using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Must align with leveldata's objecttypes currently, todo improve
public enum PlaceableObject
{
    None,
    Tree,
    Plant,
    Rocks
}
public class PanelSelector : MonoBehaviour
{
    //[SerializeField] private List<GameObject> _squares;
    [SerializeField] private List<GameObject> _selectIndicators;
    [SerializeField] private TextMeshProUGUI _costText;
    [Tooltip("Number of implemented/enabled slots")]
    private int _nActive = 3;
    private int _currentSelected = 0;
    private WorldBlossom _flower;
    private LevelData _data;

    public static Action ObjectPlaced;

    private Dictionary<PlaceableObject, int> _prices = new Dictionary<PlaceableObject, int>()
    { 
        { PlaceableObject.Tree, 10 },
        { PlaceableObject.Plant, 30 },
        { PlaceableObject.Rocks, 2 }
    };

    // Start is called before the first frame update
    void Awake()
    {
        _flower = FindObjectOfType<WorldBlossom>();
        _costText.text = "";
        PlaceIndicator.TryPlace += AuthorizePlace;
        _data = FindObjectOfType<LevelData>();
    }

    void OnDestroy()
    {
        PlaceIndicator.TryPlace -= AuthorizePlace;
    }

    // Update is called once per frame
    void Update()
    {
        Controls();
    }

    private void Controls()
    {
        _selectIndicators[_currentSelected].SetActive(false);
        var scroll = Input.GetAxis("ScrollWheel");
        if(scroll != 0)
        {
            if(scroll < 0 )
            {
                _currentSelected--;
                if(_currentSelected < 0)
                    _currentSelected = _nActive - 1;
            }
            else if(scroll > 0)
            {
                _currentSelected = (_currentSelected + 1) % _nActive;
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
            _currentSelected = 0 < _nActive ? 0 : _currentSelected;
        if(Input.GetKeyDown(KeyCode.Alpha2))
            _currentSelected = 1 < _nActive ? 1 : _currentSelected;
        if(Input.GetKeyDown(KeyCode.Alpha3))
            _currentSelected = 2 < _nActive ? 2 : _currentSelected;
        if(Input.GetKeyDown(KeyCode.Alpha4))
            _currentSelected = 3 < _nActive ? 3 : _currentSelected;
        if(Input.GetKeyDown(KeyCode.Alpha5))
            _currentSelected = 4 < _nActive ? 4 : _currentSelected;
        if(Input.GetKeyDown(KeyCode.Alpha6))
            _currentSelected = 5 < _nActive ? 5 : _currentSelected;
        if(Input.GetKeyDown(KeyCode.Alpha7))
            _currentSelected = 6 < _nActive ? 6 : _currentSelected;
        _selectIndicators[_currentSelected].SetActive(true);
        _costText.text = _prices[(PlaceableObject) (_currentSelected+1)].ToString();
    }
    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    private void AuthorizePlace(Vector2Int pos)
    {
        if(_flower.Health - _prices[(PlaceableObject) (_currentSelected+1)] > 0 )
        {
            _flower.TransactEnergy(-(_prices[(PlaceableObject) (_currentSelected + 1)]));
            _data.PlaceNewObject(pos, (ObjectType)_currentSelected + 1);
            switch((ObjectType)_currentSelected + 1)
            {
                case ObjectType.Plant:
                    _flower._plantsPlanted++;
                    break;
                case ObjectType.Tree:
                    _flower._treesPlanted++;
                    break;
            }
           _data.flower.PlaceObject?.Play();
            ObjectPlaced?.Invoke();
        }
    }
}
