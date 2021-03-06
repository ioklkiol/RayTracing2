﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Ray
{
    private Vector3D _origin;
    private Vector3D _direction;
    private double _time;   //光的时间戳

    public Vector3D Origin { get => _origin; set => _origin = value; }
    public Vector3D Direction { get => _direction; set => _direction = value; }
    public double Time { get => _time; set => _time = value; }

    public Ray(Vector3D origin, Vector3D direction,double time)
    {
        Origin = origin;
        Direction = direction;
        Time = time;
    } 
    //根据距离得到射线上的点
    public Vector3D GetPoint(double distance)
    {
        return Origin + distance * Direction;
    }
}
