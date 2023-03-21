using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public enum EMode { none, gOut, gInMiss, gInHit }

    [Header("Settings")]
    public float _grappleSpd = 10;
    public float _grappleLenth = 7;
    public float _grappleInLenth = 0.5f;
    public int _unsafeTileHealthPenalty = 2;
    public TextAsset _mapGrappleable;

    [Header("Parameters")]
    public EMode _mode = EMode.none;
    public List<int> _grappleTiles;
    public List<int> _unsafeTiles;

    private Dray _dray;
    private Rigidbody _rigid;
    private Animator _anim;
    private Collider _drayColld;

    private GameObject _grapHead;
    private LineRenderer _grapLine;
    Vector3 p0, p1;
    private int _facing;

    private Vector3[] _directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    private void Awake()
    {
        string gTiles = _mapGrappleable.text;
        gTiles = Utils.RemoveLineEndings(gTiles);
        _grappleTiles = new List<int>();
        _unsafeTiles = new List<int>();

        for(int i = 0; i<gTiles.Length; i++)
        {
            switch (gTiles[i])
            {
                case 'S':
                    _grappleTiles.Add(i);
                    break;

                case 'X':
                    _unsafeTiles.Add(i);
                    break;
            }
        }

        _dray = GetComponent<Dray>();
        _rigid = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _drayColld = GetComponent<Collider>();

        Transform trans = transform.Find("Grappler");
        _grapHead = trans.gameObject;
        _grapLine = _grapHead.GetComponent<LineRenderer>();
        _grapHead.SetActive(false);
    }

    private void Update()
    {
        if (!_dray._hasGrappler)
            return;

        switch (_mode)
        {
            case EMode.none:
                if (Input.GetKeyDown(KeyCode.X))
                {
                    StartGrapple();
                }
                break;
        }
    }

    void StartGrapple()
    {
        _facing = _dray.GetFacing();
        _dray.enabled = false;
        _anim.CrossFade("Dray_Attack_" + _facing, 0);
        _drayColld.enabled = false;
        _rigid.velocity = Vector3.zero;

        _grapHead.SetActive(true);

        p0 = transform.position + (_directions[_facing] * 0.5f);
        p1 = p0;

        _grapHead.transform.position = p1;
        _grapHead.transform.rotation = Quaternion.Euler(0, 0, 90 * _facing);

        _grapLine.positionCount = 2;
        _grapLine.SetPosition(0, p0);
        _grapLine.SetPosition(1, p1);
        _mode = EMode.gOut;
    }

    private void FixedUpdate()
    {
        switch (_mode)
        {
            case EMode.gOut:
                p1 += _directions[_facing] * _grappleSpd * Time.fixedDeltaTime;
                _grapHead.transform.position = p1;
                _grapLine.SetPosition(1, p1);

                int tileNum = TileCamera.GET_MAP(p1.x, p1.y);
                if(_grappleTiles.IndexOf(tileNum) != -1)
                {
                    _mode = EMode.gInHit;
                    break;
                }

                if((p1 - p0).magnitude >= _grappleLenth)
                {
                    _mode = EMode.gInMiss;
                }
                break;

            case EMode.gInMiss:
                p1 -= _directions[_facing] * 2 * _grappleSpd * Time.fixedDeltaTime;

                if(Vector3.Dot((p1 - p0), _directions[_facing]) > 0)
                {
                    _grapHead.transform.position = p1;
                    _grapLine.SetPosition(1, p1);
                }
                else
                {
                    StopGrapple();
                }
                break;

            case EMode.gInHit:
                float dist = _grappleInLenth + _grappleSpd * Time.fixedDeltaTime;
                if(dist > (p1 - p0).magnitude)
                {
                    p0 = p1 - (_directions[_facing] * _grappleInLenth);
                    transform.position = p0;
                    StopGrapple();
                    break;
                }

                p0 += _directions[_facing] * _grappleSpd * Time.fixedDeltaTime;
                transform.position = p0;
                _grapLine.SetPosition(0, p0);
                _grapHead.transform.position = p1;
                break;
        }
    }

    void StopGrapple()
    {
        _dray.enabled = true;
        _drayColld.enabled = true;

        int tileNum = TileCamera.GET_MAP(p0.x, p0.y);
        if(_mode == EMode.gInHit && _unsafeTiles.IndexOf(tileNum) != -1)
        {
            _dray.ResetInRoom(_unsafeTileHealthPenalty);
        }

        _grapHead.SetActive(false);
        _mode = EMode.none;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e == null)
            return;

        _mode = EMode.gInMiss;
    }
}
