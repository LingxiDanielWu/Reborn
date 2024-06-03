using System;
using UnityEngine;

public static class Utils
{
    //获取朝向，1前2后3左4右
    public static int GetOrientation(Vector3 forward, Vector3 toward)
    {
        float dot = Vector3.Dot(forward, toward);
        if (dot > 0)
        {
            return 1;
        }
        else if (dot < 0)
        {
            return 2;
        }
        else if (dot == 0)
        {
            var cross = Vector3.Cross(forward, toward);
            if (cross.y < 0)
            {
                return 3;
            }
            else if (cross.y > 0)
            {
                return 4;
            }
        }

        return 0;
    }

    public static string GetDirectionDesc(int dir)
    {
        switch (dir)
        {
            case 1:
                return "Forward";
            case 2:
                return "Backward";
            case 3:
                return "Left";
            case 4:
                return "Right";
        }

        return "";
    }

    public static float GetVecAngle(Vector3 from, Vector3 to)
    {
        var angle = Vector3.Angle(from, to);
        if (Vector3.Cross(from, to).y < 0)
        {
            angle *= -1;
        }

        return angle;
    }

    public static string GetEnumName(Type enumType, object value, bool isLower = true)
    {
        string name = Enum.GetName(enumType, value);
        return string.IsNullOrEmpty(name) ? "" : isLower ? name.ToLower() : name;
    }
}
