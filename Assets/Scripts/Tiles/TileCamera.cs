using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCamera : MonoBehaviour
{
    static private int W, H;
    static private int[,] MAP;
    static public Sprite[] SPRITES;
    static public Transform TILE_ANCHOR;
    static public Tile[,] TILES;
    static public string COLLISIONS;

    [Header("Settings")]
    public TextAsset _mapData;
    public Texture2D _mapTiles;
    public TextAsset _mapCollisions;
    public Tile _tilePrefab;
    public int _defaultTileNum;
    public List<TileSwap> _tileSwaps;

    private Dictionary<int, TileSwap> _tileSwapDict;
    private Transform _enemyAnchor, _itemAnchor;

    private void Awake()
    {
        COLLISIONS = Utils.RemoveLineEndings(_mapCollisions.text);
        PrepareTileSwapDict();
        _enemyAnchor = (new GameObject("Enemy Anchor")).transform;
        _itemAnchor = (new GameObject("Item Anchor")).transform;
        LoadMap();
    }

    void PrepareTileSwapDict()
    {
        _tileSwapDict = new Dictionary<int, TileSwap>();
        foreach(TileSwap ts in _tileSwaps)
        {
            _tileSwapDict.Add(ts.tileNum, ts);
        }
    }

    void CheckTileSwaps(int i, int j)
    {
        int tNum = GET_MAP(i, j);
        if (!_tileSwapDict.ContainsKey(tNum))
            return;

        TileSwap ts = _tileSwapDict[tNum];
        if(ts.swapPrefab != null)
        {
            GameObject go = Instantiate<GameObject>(ts.swapPrefab);
            Enemy e = go.GetComponent<Enemy>();

            if(e != null)
            {
                go.transform.SetParent(_enemyAnchor);
            }
            else
            {
                go.transform.SetParent(_itemAnchor);
            }

            go.transform.position = new Vector3(i, j, 0);

            if(ts.guaranteedItemDrop != null)
            {
                if(e != null)
                {
                    e._guaranteedItemDrop = ts.guaranteedItemDrop;
                }
            }
        }

        if(ts.overrideTileNum == -1)
        {
            SET_MAP(i, j, _defaultTileNum);
        }
        else
        {
            SET_MAP(i, j, ts.overrideTileNum);
        }
    }

    public void LoadMap()
    {
        GameObject go = new GameObject("TILE_ANCHOR");
        TILE_ANCHOR = go.transform;

        SPRITES = Resources.LoadAll<Sprite>(_mapTiles.name);

        string[] lines = _mapData.text.Split('\n');
        H = lines.Length;
        string[] tileNums = lines[0].Split(' ');
        W = tileNums.Length;

        System.Globalization.NumberStyles hexNum;
        hexNum = System.Globalization.NumberStyles.HexNumber;
        MAP = new int[W, H];
        for(int j = 0; j<H; j++)
        {
            tileNums = lines[j].Split(' ');

            for(int i = 0; i<W; i++)
            {
                if(tileNums[i] == "..")
                {
                    MAP[i, j] = 0;
                }
                else
                {
                    MAP[i, j] = int.Parse(tileNums[i], hexNum);
                }
                CheckTileSwaps(i, j);
            }
        }

        Debug.Log($"Parsed: {SPRITES.Length} sprites.");
        Debug.Log($"Map size: {W} wide by {H} hide.");

        ShowMap();
    }

    /// <summary>
    /// Generates tiles for the entire map at once
    /// </summary>
    void ShowMap()
    {
        TILES = new Tile[W, H];

        for(int j = 0; j<H; j++)
        {
            for(int i = 0; i<W; i++)
            {
                if(MAP[i,j] != 0)
                {
                    Tile tl = Instantiate<Tile>(_tilePrefab);
                    tl.transform.SetParent(TILE_ANCHOR);
                    tl.SetTile(i, j);
                    TILES[i, j] = tl;
                }
            }
        }
    }

    static public int GET_MAP(int x, int y)
    {
        if(x < 0 || x >= W ||
            y < 0 || y >= H)
        {
            return -1;
        }

        return MAP[x, y];
    }

    static public int GET_MAP(float x, float y)
    {
        int tX = Mathf.RoundToInt(x);
        int tY = Mathf.RoundToInt(y - 0.25f);

        return GET_MAP(tX, tY);
    }

    static public void SET_MAP(int x, int y, int tNum)
    {
        if (x < 0 || x >= W ||
           y < 0 || y >= H)
        {
            return;
        }

        MAP[x, y] = tNum;
    }
}

[System.Serializable]
public class TileSwap
{
    public int tileNum;
    public GameObject swapPrefab;
    public GameObject guaranteedItemDrop;
    public int overrideTileNum = -1;
}