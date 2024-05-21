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
    
    public static float GetVecAngle(Vector3 from, Vector3 to)
    {
        var angle = Vector3.Angle(from, to);
        if (Vector3.Cross(from, to).y < 0)
        {
            angle *= -1;
        }

        return angle;
    }
}
