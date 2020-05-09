using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 public class Translate : Hitable
{
    private Hitable ptr;
    private Vector3D offset;
    public Translate(Hitable p,Vector3D displacement)
    {
        ptr = p;
        offset = displacement;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        if (ptr.BoundingBox(t0, t1,out box))
        {
            box = new AABB(box.Min + offset, box.Max + offset);
            return true;
        }
        else
            return false;
    }

    public override bool  Hit(Ray r, float tMin, float tMax, out HitRecord rec)
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