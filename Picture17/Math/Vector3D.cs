using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Vector3D
{
    private float[] XYZ=new float[3];
    public float X { get => XYZ[0]; set => XYZ[0] = value; }
    public float Y { get => XYZ[1]; set => XYZ[1] = value; }
    public float Z { get => XYZ[2]; set => XYZ[2] = value; }

    public float this[int i]
    {
        get => XYZ[i];
        set =>XYZ[i] = value;
    }

    public Vector3D(float x = 0, float y = 0, float z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public Vector3D(Vector3D v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }
    //规范化向量
    public void Normalize()
    {
        float len = Length();
        X = X / len;
        Y = Y / len;
        Z = Z / len;
    }
    //得到单位向量
    public Vector3D UnitVector()
    {
        float len = Length();
        return new Vector3D(X / len, Y / len, Z / len);
    }

    public float Length()
    {
        return Mathf.Sqrt(X * X + Y * Y + Z * Z);

    }

    public float SquaredMagnitude()
    {
        return (X * X + Y * Y + Z * Z);
    }
    //取相反向量
    public static Vector3D operator -(Vector3D v)
    {
        return new Vector3D(-v.X, -v.Y, -v.Z);
    }
    //向量加法
    public static Vector3D operator +(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }
    //向量减法
    public static Vector3D operator -(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }
    //向量点乘
    public static float operator *(Vector3D v1, Vector3D v2)
    {
        return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    }
    //向量叉乘
    public static Vector3D Cross(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.Y * v2.Z - v1.Z * v2.Y,
         v1.Z * v2.X - v1.X * v2.Z,
         v1.X * v2.Y - v1.Y * v2.X);
    }

    public static Vector3D operator *(float d, Vector3D v)
    {
        return new Vector3D(d * v.X, d * v.Y, d * v.Z);
    }
    public static Vector3D operator *(Vector3D v, float d)
    {
        return new Vector3D(d * v.X, d * v.Y, d * v.Z);
    }
    public static Vector3D operator /(Vector3D v, float d)
    {
        return new Vector3D(v.X / d, v.Y / d, v.Z / d);
    }
}
