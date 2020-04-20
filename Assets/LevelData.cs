using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ObjectType
{
    None,
    Tree,
    Plant,
    Rocks,
    Flower,
    Energy
}

public struct Connection
{
    public Vector2Int start;
    public Vector2Int end;

    public Connection(Vector2Int a, Vector2Int b)
    {
        start = a;
        end = b;
    }
}

public class LevelData : MonoBehaviour
{
    private const int INITIALWORLDSIZE = 200;

    [SerializeField] private Tilemap _backgroundTilemap;
    [SerializeField] private Tilemap _foregroundTilemap;

    //cosmetic tiles
    [SerializeField] private Tile _grassTile;
    [SerializeField] private Tile _blankTile;

    private int _halfWorldSize = 0;

    private Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();

    //only invoked after initial generation
    public static Action<Vector2Int, ObjectType> ObjectPlaced;

    private readonly Dictionary<ObjectType, string> _names = new Dictionary<ObjectType, string>
    {
        {ObjectType.Flower, "worldBlossom"},
        {ObjectType.Energy, "energy"},
        {ObjectType.Rocks, "rocks"},
        {ObjectType.Tree, "tree"},
        {ObjectType.Plant, "plant"}
    };
    //TODO remove, redundant
    private ObjectType[] worldData;

    private List<WorldObject> worldObjects = new List<WorldObject>();
    private ICollidable[] worldObjectColliders;
    public WorldBlossom flower;

    private List<Connection> treeConnections;

    public List<WorldObject> GetObjectsOfType(ObjectType type_in)
    {
        var resultList = new List<WorldObject>();
        foreach(WorldObject wo in worldObjects)
        {
            if(wo.Type == type_in)
            {
                resultList.Add(wo);
            }    
        }
        return resultList;
    }

    public void RemoveFromWorldObjects(Vector2Int pos)
    {
        pos.x += _halfWorldSize;
        pos.y += _halfWorldSize;
        pos.y = INITIALWORLDSIZE - pos.y -1;

        worldObjectColliders[pos.x + pos.y * INITIALWORLDSIZE] = null; 
        //this is redundant in some cases, fix later
        worldData[pos.x + pos.y * INITIALWORLDSIZE] = ObjectType.None;
    }

    public void AddToWorldObjects(Vector2Int pos, ICollidable collider, ObjectType type_in)
    {
        pos.x += _halfWorldSize;
        pos.y += _halfWorldSize;
        pos.y = INITIALWORLDSIZE - pos.y - 1;

        worldObjectColliders[pos.x + pos.y * INITIALWORLDSIZE] = collider;
        worldData[pos.x + pos.y * INITIALWORLDSIZE] = type_in;
    }

    //might be slow compared to other memory intensive options but no time left
    public void RegisterTreeConnection( Vector2Int pos1, Vector2Int pos2)
    {
        treeConnections.Add(new Connection(pos1, pos2));
    }

    public bool IsObstructed(Vector2Int pos1, Vector2Int pos2)
    {
        var difference = pos2 - pos1;
        if(difference.x > 0)
        {
            for(int i = pos1.x + 1; i < pos2.x; i++)
            {
                if(!IsValidMove(new Vector2Int(i, pos1.y)))
                {
                    return true;
                }
            }
        }
        else if(difference.x < 0)
        {
            for(int i = pos1.x - 1; i > pos2.x; i--)
            {
                if(!IsValidMove(new Vector2Int(i, pos1.y)))
                {
                    return true;
                }
            }
        }
        else if(difference.y > 0)
        {
            for(int i = pos1.y + 1; i < pos2.y; i++)
            {
                if(!IsValidMove(new Vector2Int(pos1.x, i)))
                {
                    return true;
                }
            }
        }
        else if(difference.y < 0)
        {
            for(int i = pos1.y - 1; i > pos2.y; i--)
            {
                if(!IsValidMove(new Vector2Int(pos1.x, i)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsPoweredTile(Vector2Int pos)
    {
        bool result = false;
        foreach(Connection c in treeConnections)
        {
            var direction = c.start - c.end;
            if(direction.x != 0)
            {
                if(pos.y == c.start.y && ( (pos.x > c.start.x && pos.x < c.end.x) || (pos.x > c.end.x && pos.x < c.start.x) ) )
                    result = true;
            }   
            else if(direction.y != 0 )
            {
                if(pos.x == c.start.x && ((pos.y > c.start.y && pos.y < c.end.y) || (pos.y > c.end.y && pos.y < c.start.y)) )
                    result = true;
            }
        }
        return result;
    }

    void Awake()
    {
        _halfWorldSize = (int) (INITIALWORLDSIZE / 2.0f);
        worldData = new ObjectType[INITIALWORLDSIZE * INITIALWORLDSIZE];
        worldObjectColliders = new ICollidable[INITIALWORLDSIZE * INITIALWORLDSIZE];
        treeConnections = new List<Connection>();
        //load tiles into fields
        GetObjects();

        //Generate intial area
        GenerateNewTiles(new RectInt(-_halfWorldSize, -_halfWorldSize, INITIALWORLDSIZE, INITIALWORLDSIZE));

        PlayerMovement.PlayerMoved += HandleCollisions;
    }

    void OnDestroy()
    {
        PlayerMovement.PlayerMoved -= HandleCollisions;
    }
    
    private void GetObjects()
    {
        var tiles = Resources.LoadAll<GameObject>("Objects/");

        foreach(GameObject t in tiles) 
        {
            _objects[t.name] = t;
        }
    }

    private void GenerateNewTiles(RectInt rect)
    {
        //add grass
        FillGrass(rect);
        //set world blossom at world origin
        flower = SetTile(new Vector2Int(0,0),ObjectType.Flower).GetComponent<WorldBlossom>();   
        //distribute latent energy
        FillRandomObjects(rect);
    }

    public GameObject SetTile(Vector2Int position, ObjectType objectType)
    {
        var obj = Instantiate(_objects[_names[objectType]], new Vector3(position.x + .5f, position.y + .5f, .0f), Quaternion.identity);
        var worldObj = obj.GetComponent<WorldObject>();
        if(worldObj!=null)
        { 
            worldObj.Init(position, objectType, this);
            worldObjects.Add(worldObj);
        }
        var collidable = GetComponent<ICollidable>();
        
        position.x += _halfWorldSize;
        position.y += _halfWorldSize;
        position.y = INITIALWORLDSIZE - position.y - 1;

        if(collidable != null)
        {
            worldObjectColliders[position.x + position.y * INITIALWORLDSIZE] = collidable;
        }

        worldData[position.y * INITIALWORLDSIZE + position.x] = objectType;
        return obj;
    }

    public bool IsValidMove(Vector2Int position)
    {
        position.x += _halfWorldSize;
        position.y += _halfWorldSize;
        position.y = INITIALWORLDSIZE - position.y - 1;

        var type = worldData[position.y * INITIALWORLDSIZE + position.x];
        return type == ObjectType.None || type == ObjectType.Energy;
    }

    public bool CanPlaceObject(Vector2Int position)
    {
        if(position.x > 98 || position.x < -99 || position.y > 98 || position.y < -99)
            return false;
        position.x += _halfWorldSize;
        position.y += _halfWorldSize;
        position.y = INITIALWORLDSIZE - position.y - 1;
        return (worldData[position.y * INITIALWORLDSIZE + position.x] == ObjectType.None);
    }

    public void PlaceNewObject(Vector2Int position, ObjectType type)
    {
        var result = SetTile(position, type);
        AddToWorldObjects(position, result.GetComponent<ICollidable>(), type);
        ObjectPlaced?.Invoke(position, type);
    }

    private void FillRandomObjects(RectInt rect)
    {
        for(int i = 0; i < rect.width * rect.height; i++)
        {
            var value = UnityEngine.Random.value;
            var pos = new Vector2Int(rect.xMin + (i % rect.width), rect.yMin + (i / rect.width));
            //grows as the player is further away from the center
            var sparsityScaler = (Mathf.Abs(pos.x) / 100.0f + Mathf.Abs(pos.y) / 100.0f) * .01f;

            //world blossom, player start position
            if(pos == new Vector2Int(0, 0) || pos == new Vector2Int(0,1))
                continue;

            //less chance if player is far away from center
            if(value<(.03f - sparsityScaler))
            {
                var result = SetTile(pos, ObjectType.Energy);
                AddToWorldObjects(pos, result.GetComponent<ICollidable>(), ObjectType.Energy);
            }
            //todo clumpify rocks
            else if( value > .03f && value < .05f)
            {
                var result = SetTile(pos, ObjectType.Rocks);
                AddToWorldObjects(pos, result.GetComponent<ICollidable>(), ObjectType.Rocks);
            }
        }
    }

    private void FillGrass(RectInt rect)
    {
        var tiles = new TileBase[rect.width * rect.height];
        for(int i = 0; i < tiles.Length; i++)
        {
            if( UnityEngine.Random.value < .33f)
            {
                tiles[i] = _blankTile;
            }
            else
            {
                tiles[i] = _grassTile;
            }
            
        }
        _backgroundTilemap.SetTilesBlock(new BoundsInt(rect.xMin, rect.yMin, 0, rect.width, rect.height, 1), tiles);
    }

    private void HandleCollisions(Vector2Int pos, GameObject mover)
    {
        var originalPos = pos;
        pos.x += _halfWorldSize;
        pos.y += _halfWorldSize;
        pos.y = INITIALWORLDSIZE - pos.y - 1;

        if(worldObjectColliders[pos.x + pos.y * INITIALWORLDSIZE] != null)
        {
            worldObjectColliders[pos.x + pos.y * INITIALWORLDSIZE].OnCollision(originalPos, mover);
        }
    }
}
