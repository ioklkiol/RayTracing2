using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//AABB包围盒  axis-aligned bounding boxes
public class AABB
{
    private Vector3D _min;
    private Vector3D _max;

    public Vector3D Min { get => _min; set => _min = value; }
    public Vector3D Max { get => _max; set => _max = value; }

    public AABB(Vector3D min, Vector3D max)
    {
        Min = min;
        Max = max;
    }
    public bool Hit(Ray r, double tMin, double tMax)
    {
        for (int a = 0; a < 3; a++)
        {
            double invD = 1 / r.Direction.XYZ[a];
            double t0 = Math.Min((Min.XYZ[a] - r.Origin.XYZ[a]) * invD,
                                (Max.XYZ[a] - r.Origin.XYZ[a]) * invD);
            double t1 = Math.Max((Min.XYZ[a] - r.Origin.XYZ[a]) * invD,
                                (Max.XYZ[a] - r.Origin.XYZ[a]) * invD);
            if (invD < 0)
            {
                double temp = t0;
                t0 = t1;
                t1 = temp;
            }
            tMin = Math.Max(t0, tMin);
            tMax = Math.Min(t1, tMax);
            if (tMax <= tMin)
                return false;
        }
        return true;
    }
}