using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FlipNormals : IHitable
{
    private IHitable ptr;
    public FlipNormals(IHitable p)
    {
        ptr = p;
    }

    public bool BoundingBox(double t0, double t1, out AABB box)
    {
        box = new AABB(new Vector3D(), new Vector3D());
        return ptr.BoundingBox(t0, t1,out box);
    }

    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        if (ptr.Hit(r, tMin, tMax, out rec))
        {
            rec.normal = -rec.normal;
            return true;
        }
        else
            return false;
    }
}