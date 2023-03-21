using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InRoom : MonoBehaviour
{
    static public float ROOM_W = 16;
    static public float ROOM_H = 11;
    static public float WALL_T = 2;

    static public int MAX_RM_X = 9;
    static public int MAX_RM_Y = 9;

    static public Vector2[] DOORS = new Vector2[] { new Vector2(14, 5), new Vector2(7.5f, 9), new Vector2(1, 5), new Vector2(7.5f, 1) };

    [Header("Settings")]
    public bool _keepInRoom = true;
    public float _gridMult = 1;

    public Vector2 roomPos
    {
        get
        {
            Vector2 tPos = transform.position;
            tPos.x %= ROOM_W;
            tPos.y %= ROOM_H;
            return tPos;
        }
        set
        {
            Vector2 rm = roomNum;
            rm.x *= ROOM_W;
            rm.y *= ROOM_H;
            rm += value;
            transform.position = rm;
        }
    }

    public Vector2 roomNum
    {
        get
        {
            Vector2 tPos = transform.position;
            tPos.x = Mathf.Floor(tPos.x / ROOM_W);
            tPos.y = Mathf.Floor(tPos.y / ROOM_H);
            return tPos;
        }
        set
        {
            Vector2 rPos = roomPos;
            Vector2 rm = value;
            rm.x *= ROOM_W;
            rm.y *= ROOM_H;
            transform.position = rm + rPos; 
        }
    }

    private void LateUpdate()
    {
        if (_keepInRoom)
        {
            Vector2 rPos = roomPos;
            rPos.x = Mathf.Clamp(rPos.x, WALL_T, ROOM_W - 1 - WALL_T);
            rPos.y = Mathf.Clamp(rPos.y, WALL_T, ROOM_H - 1 - WALL_T);
            roomPos = rPos;
        }
    }

    public Vector2 GetRoomPosGrid(float mult = -1)
    {
        if(mult == -1)
        {
            mult = _gridMult;
        }

        Vector2 rPos = roomPos;
        rPos /= mult;
        rPos.x = Mathf.Round(rPos.x);
        rPos.y = Mathf.Round(rPos.y);
        rPos *= mult;

        return rPos;
    }
}
