using System;
using UnityEngine;

public static class Game
{
    public static Vector3 boxSize = new Vector3(0.47f, 0.47f, 0.47f);
    public static Vector3 smallBoxSize = new Vector3(0.25f, 0.47f, 0.25f);
}

public static class Convert
{
    public static Vector3 Align(this Vector3 v)
    {
        return new Vector3( (float)Mathf.RoundToInt(v.x), (float)Mathf.RoundToInt(v.y), (float)Mathf.RoundToInt(v.z));
    }
    public static Vector3 V2IntToV3(this Vector2Int v)
    {
        return new Vector3(v.x,0,v.y);
    }
    public static Vector2Int V3ToV2Int(this Vector3 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z));
    }

    internal static Vector2Int PosToStep(Vector3 pos,Vector2Int step)
    {
        if (step.x - pos.x > 0.8f) return Vector2Int.right;
        else if (step.x - pos.x < -0.8f) return Vector2Int.left;
        else if (step.y - pos.y > 0.8f) return Vector2Int.up;
        else if (step.y - pos.y < -0.8f) return Vector2Int.down;
        else return Vector2Int.zero;
    }
}
