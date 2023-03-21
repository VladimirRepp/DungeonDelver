using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] _directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    [Header("Settings: Enemy")]
    public float _maxHelth = 1;
    public float _knockbackSpeed = 10;
    public float _knockbackDuration = 0.25f;
    public float _invinvibleDuration = 0.5f;
    public GameObject _guaranteedItemDrop = null;
    public GameObject[] _randomItemDrops;

    [Header("Parameters: Enemy")]
    public float _helth;
    public bool _invincible = false;
    public bool _knockback = false;

    private float _knockbackDone = 0;
    private float _invincibleDone = 0;
    private Vector3 _knockbackVel;

    protected Animator _animator;
    protected Rigidbody _rigid;
    protected SpriteRenderer _sRend;

    protected virtual void Awake()
    {
        _helth = _maxHelth;
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
        _sRend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if(_invincible && Time.time > _invincibleDone)
        {
            _invincible = false;
        }
        
        _sRend.color = _invincible ? Color.red : Color.white;
        if (_knockback)
        {
            _rigid.velocity = _knockbackVel;
            if (Time.time < _knockbackDone)
                return;
        }

        _animator.speed = 1;
        _knockback = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_invincible)
            return;

        DamageEffect dEf = other.gameObject.GetComponent<DamageEffect>();
        if (dEf == null)
            return;

        _helth -= dEf.damage;
        if(_helth <= 0)
        {
            Die();
        }

        _invincible = true;
        _invincibleDone = Time.time + _invinvibleDuration;

        if (dEf.knockback)
        {
            Vector3 delta = transform.position - other.transform.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
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

            _knockback = true;
            _knockbackDone = Time.time + _knockbackDuration;
            _animator.speed = 0;
        }
    }

    void Die()
    {
        GameObject go;
        if(_guaranteedItemDrop != null)
        {
            go = Instantiate<GameObject>(_guaranteedItemDrop);
            go.transform.position = transform.position;
        }
        else if(_randomItemDrops.Length > 0)
        {
            int n = Random.Range(0, _randomItemDrops.Length);
            GameObject prefab = _randomItemDrops[n];
            if(prefab != null)
            {
                go = Instantiate<GameObject>(prefab);
                go.transform.position = transform.position;
            }
        }
        Destroy(gameObject);
    }
}
