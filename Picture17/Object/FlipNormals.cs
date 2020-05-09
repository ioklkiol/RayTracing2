using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FlipNormals : Hitable
{
    private Hitable ptr;
    public FlipNormals(Hitable p)
    {
        ptr = p;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(), new Vector3D());
        return ptr.BoundingBox(t0, t1,out box);
    }

    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
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