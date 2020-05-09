using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//该类用来保存击中点的信息
public class HitRecord
{
    public float t;           //公式中击中点对应的t值.同时也是视点与击中点之间的距离
    public Vector3D p;         //击中点的位置
    public Vector3D normal;    //法线 
    public Material matPtr;    //材质
    public float u;           //texute-u
    public float v;           //texute-v
}
