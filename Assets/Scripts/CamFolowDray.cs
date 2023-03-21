using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFolowDray : MonoBehaviour
{
    static public bool TRANSITIONING = false;

    [Header("Settings")]
    public InRoom _drayInRm;
    public float _transTime = 0.5f;

    private Vector3 p0, p1;

    private InRoom _inRoom;
    private float _transStart;

    private void Awake()
    {
        _inRoom = GetComponent<InRoom>();
    }

    private void Update()
    {
        if (TRANSITIONING)
        {
            float u = (Time.time - _transStart) / _transTime;
            if(u >= 1)
            {
                u = 1;
                TRANSITIONING = false;
            }

            transform.position = (1 - u) * p0 + u * p1;
        }
        else
        {
            if (_drayInRm.roomNum != _inRoom.roomNum)
                TransitionTo(_drayInRm.roomNum);
        }
    }

    void TransitionTo(Vector2 rm)
    {
        p0 = transform.position;
        _inRoom.roomNum = rm;
        p1 = transform.position + (Vector3.back * 10);
        transform.position = p0;

        _transStart = Time.time;
        TRANSITIONING = true;
    }
}
