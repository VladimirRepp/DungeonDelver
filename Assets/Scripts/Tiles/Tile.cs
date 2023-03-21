using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Parameters")]
    public int x;
    public int y;
    public int _tileNum;

    private BoxCollider _bCol;

    private void Awake()
    {
        _bCol = GetComponent<BoxCollider>();
    }

    public void SetTile(int eX, int eY, int eTileNum = -1)
    {
        x = eX;
        y = eY;

        transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3") + "x" + y.ToString("D3");

        if(eTileNum == -1)
        {
            eTileNum = TileCamera.GET_MAP(x, y);
        }
        else
        {
            TileCamera.SET_MAP(x, y, eTileNum);
        }

        _tileNum = eTileNum;
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[_tileNum];

        SetCollider();
    }

    void SetCollider()
    {
        _bCol.enabled = true;
        char c = TileCamera.COLLISIONS[_tileNum];

        switch (c)
        {
            case 'S':
                _bCol.center = Vector3.zero;
                _bCol.size = Vector3.one;
                break;

            case 'W':
                _bCol.center = new Vector3(0, 0.25f, 0);
                _bCol.size = new Vector3(1, 0.5f, 1);
                break;

            case 'A':
                _bCol.center = new Vector3(-0.25f, 0, 0);
                _bCol.size = new Vector3(0.5f, 1, 1);
                break;

            case 'D':
                _bCol.center = new Vector3(0.25f, 0, 0);
                _bCol.size = new Vector3(0.5f, 1, 1);
                break;

            case 'Q':
                _bCol.center = new Vector3(-0.25f, 0.25f, 0);
                _bCol.size = new Vector3(0.5f, 0.5f, 1);
                break;

            case 'E':
                _bCol.center = new Vector3(0.25f, 0.25f, 0);
                _bCol.size = new Vector3(0.5f, 0.5f, 1);
                break;

            case 'Z':
                _bCol.center = new Vector3(-0.25f, -0.25f, 0);
                _bCol.size = new Vector3(0.5f, 0.5f, 1);
                break;

            case 'X':
                _bCol.center = new Vector3(0, -0.25f, 0);
                _bCol.size = new Vector3(1, 0.5f, 1);
                break;

            case 'C':
                _bCol.center = new Vector3(0.25f, -0.25f, 0);
                _bCol.size = new Vector3(0.5f, 0.5f, 1);
                break;

            default:
                _bCol.enabled = false;
                break;
        }
    }
}
