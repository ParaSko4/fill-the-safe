using UnityEngine;

public static class VectorExtension
{
    public static Vector3 ChangeZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
    
    public static Vector3 ChangeX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector3 ChangeY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector3 ToVector3(this Vector2 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
}