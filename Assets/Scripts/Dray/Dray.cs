using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour, IFacingMover, IKeyMaster
{
    public enum EMode { idle, move, attack, transition, knockback}

    [Header("Settings")]
    public float _speed = 5f;
    public float _attackDuration = 0.25f;
    public float _attackDelay = 0.5f;
    public float _transitionDelay = 0.5f;

    public int _maxHealth;
    public float _knockbackSpeed = 10;
    public float _knockbackDuration = 0.25f;
    public float _invinvibleDuration = 0.5f;

    [Header("Parameters")]
    public int _dirHeld = -1;
    public int _facing = 1;
    public EMode _mode = EMode.idle;
    public int _numKeys = 0;
    public bool _invincible = false;
    public bool _hasGrappler = false;
    public Vector3 _lastSafeLoc;
    public int _lastSafeFacing;

    private float _transitionDone = 0;
    private Vector2 _transitionPos;

    private float _timeAtkDone = 0;
    private float _timeAtkNext = 0;
    private InRoom _inRoom;

    private SpriteRenderer _sRend;
    private Rigidbody _rigid;

    private Vector3[] _directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };
    private KeyCode[] _keys = new KeyCode[] { KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow };
    private Animator _animator;

    private float _knockbackDone = 0;
    private float _invincibleDone = 0;
    private Vector3 _knockbackVel;

    [SerializeField] private int _health;

    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    public int keyCount
    {
        get
        {
            return _numKeys;
        }
        set
        {
            _numKeys = value;
        }
    }

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
            return (_mode == EMode.move);
        }
    }

    private void Awake()
    {
        _sRend = GetComponent<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _inRoom = GetComponent<InRoom>();
        
        _health = _maxHealth;

        _lastSafeLoc = transform.position;
        _lastSafeFacing = _facing;
    }

    private void Update()
    {
        if (_invincible && Time.time > _invincibleDone)
            _invincible = false;

        _sRend.color = _invincible ? Color.red : Color.white;

        if(_mode == EMode.knockback)
        {
            _rigid.velocity = _knockbackVel;
            if (Time.time < _knockbackDone)
                return;
        }

        if(_mode == EMode.transition)
        {
            _rigid.velocity = Vector3.zero;
            _animator.speed = 0;
            roomPos = _transitionPos;

            if (Time.time < _transitionDone)
                return;

            _mode = EMode.idle;
        }

        _dirHeld = -1;

        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKey(_keys[i]))
                _dirHeld = i;
        }

        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= _timeAtkNext)
        {
            _mode = EMode.attack;
            _timeAtkDone = Time.time + _attackDuration;
            _timeAtkNext = Time.time + _attackDelay;
        }

        if(Time.time >= _timeAtkDone)
        {
            _mode = EMode.idle;
        }

        if(_mode != EMode.attack)
        {
            if(_dirHeld == -1)
            {
                _mode = EMode.idle;
            }
            else
            {
                _facing = _dirHeld;
                _mode = EMode.move;
            }
        }

        Vector3 vel = Vector3.zero;
        switch (_mode)
        {
            case EMode.attack:
                _animator.CrossFade("Dray_Attack_" + _facing, 0);
                _animator.speed = 0;
                break;

            case EMode.idle:
                _animator.CrossFade("Dray_Walk_" + _facing, 0);
                _animator.speed = 0;
                break;

            case EMode.move:
                vel = _directions[_dirHeld];
                _animator.CrossFade("Dray_Walk_" + _facing, 0);
                _animator.speed = 1;
                break;
        }

        _rigid.velocity = vel * _speed;
    }

    void LateUpdate()
    {
        Vector2 rPos = GetRoomPosOnGrid(0.5f);

        int doorNum;
        for(doorNum = 0; doorNum<4; doorNum++)
        {
            if (rPos == InRoom.DOORS[doorNum])
                break;
        }

        if (doorNum > 3 || doorNum != _facing)
            return;

        Vector2 rm = roomNum;
        switch (doorNum)
        {
            case 0:
                rm.x += 1;
                break;

            case 1:
                rm.y += 1;
                break;

            case 2:
                rm.x -= 1;
                break;

            case 3:
                rm.y -= 1;
                break;
        }

        if(rm.x >= 0 && rm.x <= InRoom.MAX_RM_X)
        {
            if(rm.y >= 0 && rm.y <= InRoom.MAX_RM_Y)
            {
                roomNum = rm;
                _transitionPos = InRoom.DOORS[(doorNum + 2) % 4];
                roomPos = _transitionPos;
                
                _lastSafeLoc = transform.position;
                _lastSafeFacing = _facing;

                _mode = EMode.transition;
                _transitionDone = Time.time + _transitionDelay;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_invincible)
            return;

        DamageEffect dEf = collision.gameObject.GetComponent<DamageEffect>();
        if (dEf == null)
            return;

        _health -= dEf.damage;
        _invincible = true;
        _invincibleDone = Time.time + _invinvibleDuration;

        if (dEf.knockback)
        {
            Vector3 delta = transform.position - collision.transform.position;
            if(Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            }
            else
            {
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            _knockbackVel = delta * _knockbackSpeed;
            _rigid.velocity = _knockbackVel;

            _mode = EMode.knockback;
            _knockbackDone = Time.time + _knockbackDuration;
            _animator.speed = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PickUp pup = other.gameObject.GetComponent<PickUp>();
        if (pup == null)
            return;

        switch (pup._itemType)
        {
            case PickUp.EType.health:
                _health = Mathf.Min(_health + 2, _maxHealth);
                break;

            case PickUp.EType.key:
                keyCount++;
                break;

            case PickUp.EType.grappler:
                _hasGrappler = true;
                break;
        }

        Destroy(other.gameObject);
    }

    public void ResetInRoom(int healthLoss = 0)
    {
        transform.position = _lastSafeLoc;
        _facing = _lastSafeFacing;
        _health -= healthLoss;

        _invincible = true;
        _invincibleDone = Time.time + _invinvibleDuration;
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
