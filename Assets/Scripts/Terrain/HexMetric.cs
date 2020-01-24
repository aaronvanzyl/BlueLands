using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection { E, NE, NW, W, SW, SE };

public static class HexMetric
{
    //static Vector2Int[] evenSteps = { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int() };
    public static float hexOuterRatio = 1.1547f;

    public static Vector3 WorldCoords(int x, int y, float height = 0)
    {
        Vector3 v = new Vector3();
        v.x = x + (y % 2) * 0.5f + 0.5f;
        v.y = height;
        v.z = y / hexOuterRatio + 0.5f;
        return v;
    }

    //XY
    public static Vector3 WorldCoords(Vector2Int v)
    {
        return WorldCoords(v.x, v.y);
    }

    //XY
    public static Vector2Int Step(Vector2Int pos, HexDirection dir)
    {

        if (dir == HexDirection.W)
        {
            return new Vector2Int(-1, 0);
        }
        if (dir == HexDirection.E)
        {
            return new Vector2Int(1, 0);
        }
        Vector2Int s = Vector2Int.zero;
        s.y = dir == HexDirection.NE || dir == HexDirection.NW ? 1 : -1;
        s.x = dir == HexDirection.NE || dir == HexDirection.SE ? 0 : -1;
        if (pos.y % 2 == 1)
        {
            s.x += 1;
        }
        return s;
    }

    public static Vector2Int HexToXY(Vector2Int hexCoords)
    {
        Vector2Int result = new Vector2Int();
        result.y = hexCoords.y;
        result.x = hexCoords.x + hexCoords.y / 2;
        return result;
    }

    public static Vector2Int XYToHex(Vector2Int xyCoords)
    {
        Vector2Int result = new Vector2Int();
        result.y = xyCoords.y;
        result.x = xyCoords.x - xyCoords.y / 2;
        return result;
    }

    //XY
    public static HexDirection DirectionBetween(Vector2Int from, Vector2Int to)
    {
        Vector3 fromPos = WorldCoords(from);
        Vector3 toPos = WorldCoords(to);
        float angle = Mathf.Atan2(toPos.z - fromPos.z, toPos.x - fromPos.x);
        if (angle < 0)
        {
            angle = 2 * Mathf.PI + angle;
        }
        //Debug.DrawRay(fromPos + Vector3.up * 30, new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 0.48f, Color.red, 10000);
        angle = angle / (2 * Mathf.PI);
        int dir = (int)(angle * 6 + 0.5f) % 6;
        //Debug.Log(Mathf.Rad2Deg*angle);
        //if (angle<0) {
        //Debug.Log("very bad");
        //}

        //int dir = (int)(Mathf.RoundToInt(angle / (2 * Mathf.PI) * 6f + 0.5f));
        //Debug.Log(angle / (2 * Mathf.PI) * 6f);
        return (HexDirection)dir;
    }
}
