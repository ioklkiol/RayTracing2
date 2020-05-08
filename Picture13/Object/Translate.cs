using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

unsafe public class Translate : IHitable
{
    private IHitable ptr;
    private Vector3D offset;
    public Translate(IHitable p,Vector3D displacement)
    {
        ptr = p;
        offset = displacement;
    }

    public bool BoundingBox(double t0, double t1, out AABB box)
    {
        if (ptr.BoundingBox(t0, t1,out box))
        {
            box = new AABB(box.Min + offset, box.Max + offset);
            return true;
        }
        else
            return false;
    }

    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        Ray movedR = new Ray(r.Origin - offset, r.Direction, r.Time);
        if (ptr.Hit(movedR, tMin, tMax, out rec))
        {
            rec.p += offset;
            return true;
        }
        else
            return false;
    }
}