using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//AABB包围盒  axis-aligned bounding boxes
public class AABB
{
    private Vector3D _min;      //三维区间左端点
    private Vector3D _max;      //三维区间右端点

    public Vector3D Min { get => _min; set => _min = value; }
    public Vector3D Max { get => _max; set => _max = value; }

    public AABB()
    {
        Min = new Vector3D();
        Max = new Vector3D();
    }
    public AABB(Vector3D min, Vector3D max)
    {
        Min = min;
        Max = max;
    }
    public bool Hit(Ray r, double tMin, double tMax)
    {
        for (int a = 0; a < 3; a++)
        {
            double invD = 1 / r.Direction[a];
            double t0 = Math.Min((Min[a] - r.Origin[a]) * invD,     //tx0,ty0,tz0
                                (Max[a] - r.Origin[a]) * invD);
            double t1 = Math.Max((Min[a] - r.Origin[a]) * invD,     //tx1,ty1,tz1
                                (Max[a] - r.Origin[a]) * invD);
            //if (invD < 0)
            //{
            //    double temp = t0;                                 //这里貌似是书上写错了
            //    t0 = t1;
            //    t1 = temp;
            //}
            tMin = Math.Max(t0, tMin);
            tMax = Math.Min(t1, tMax);
            if (tMax <= tMin)                                        //区间没有发生重叠，则与光线没有交点
                return false;
        }
        return true;
    }
}