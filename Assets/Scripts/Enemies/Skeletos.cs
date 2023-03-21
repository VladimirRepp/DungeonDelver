using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeletos : Enemy, IFacingMover
{
    [Header("Settings: Skeletos")]
    public int _speed = 2;
    public float _timeThinkMin = 1f;
    public float _timeThinkMax = 4f;

    [Header("Parameters: Skeletos")]
    public int _facing = 0;
    public float _timeNextDecision = 0;

    private InRoom _inRoom;

    public float gridMult
    {
        get
        {
            return _inRoom._gridMult;
        }
    }

    public Vector2 roomPos
    {
        get
        {
            return _inRoom.roomPos;
        }
        set
        {
            _inRoom.roomPos = value;
        }
    }

    public Vector2 roomNum
    {
        get
        {
            return _inRoom.roomNum;
        }
        set
        {
            _inRoom.roomNum = value;
        }
    }

    public bool moving
    {
        get
        {
            return true;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _inRoom = GetComponent<InRoom>();
    }

    protected override void Update()
    {
        base.Update();
        if (_knockback)
            return;

        if(Time.time >= _timeNextDecision)
        {
            DecideDirection();
        }

        _rigid.velocity = _directions[_facing] * _speed;
    }

    void DecideDirection()
    {
        _facing = Random.Range(0, 4);
        _timeNextDecision = Time.time + Random.Range(_timeThinkMin, _timeThinkMax);
    }

    public int GetFacing()
    {
        return _facing;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        return _inRoom.GetRoomPosGrid(mult);
    }
}
