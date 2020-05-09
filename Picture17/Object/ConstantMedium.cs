using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//体恒量介质
public class ConstantMedium : Hitable
{
    private Hitable boundary;
    private float density;
    private Material phaseFunction;    //材质为各向异性材质

    public ConstantMedium(Hitable boundary, float density, Texture a)
    {
        this.boundary = boundary;
        this.density = density;
        this.phaseFunction = new Isotropic(a);
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        return boundary.BoundingBox(t0, t1, out box);
    }

    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        HitRecord rec1, rec2;
        if (boundary.Hit(r, float.MinValue, float.MaxValue, out rec1))
        {
            if (boundary.Hit(r, rec1.t + 0.0001f, float.MaxValue, out rec2))
            {
                rec1.t = Mathf.Max(rec1.t, tMin);
                rec2.t = Mathf.Min(rec2.t, tMax);
                if (rec1.t >= rec2.t)
                    return false;
                rec1.t = Mathf.Max(rec1.t, 0);
                float distanceInsideBoundary = (rec2.t - rec1.t) * r.Direction.Length();
                float hitDistance = -(1 / density) * Mathf.Log(Mathf.Randomfloat());
                if (hitDistance < distanceInsideBoundary)
                {
                    rec.t = rec1.t + hitDistance / r.Direction.Length();
                    rec.p = r.GetPoint(rec.t);
                    rec.normal = new Vector3D(1, 0, 0);
                    rec.matPtr = phaseFunction;
                    return true;
                }
            }
        }
        return false;
    }
}