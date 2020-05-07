﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Vector3D
{
    private double _x;
    private double _y;
    private double _z;
    private double[] _xyz=new double[3];

    public double X
    {
        get => _x;
        set
        {
            _x = value;
            _xyz[0] = _x;
        }
    }
    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            _xyz[1] = _y;
        }
    }
    public double Z
    {
        get => _z;
        set
        {
            _z = value;
            _xyz[2] = _z;
        }
    }

    public double[] XYZ
    {
        get => _xyz;
        set
        {
            _xyz = value;
            _x = _xyz[0];
            _y = _xyz[1];
            _z = _xyz[2];
        }

    }

    public Vector3D(double x = 0, double y = 0, double z = 0)
    {
        X = x;
        Y = y;
        Z = z;
        XYZ = new double[3] { x, y, z };
    }
    //规范化向量
    public void Normalize()
    {
        double len = Length();
        X = X / len;
        Y = Y / len;
        Z = Z / len;
    }
    //得到单位向量
    public Vector3D UnitVector()
    {
        double len = Length();
        return new Vector3D(X / len, Y / len, Z / len);
    }

    public double Length()
    {
        return Math.Sqrt(X * X + Y * Y + Z * Z);

    }

    public double SquaredMagnitude()
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
    public static double operator *(Vector3D v1, Vector3D v2)
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

    public static Vector3D operator *(double d, Vector3D v)
    {
        return new Vector3D(d * v.X, d * v.Y, d * v.Z);
    }
    public static Vector3D operator *(Vector3D v, double d)
    {
        return new Vector3D(d * v.X, d * v.Y, d * v.Z);
    }
    public static Vector3D operator /(Vector3D v, double d)
    {
        return new Vector3D(v.X / d, v.Y / d, v.Z / d);
    }
}
