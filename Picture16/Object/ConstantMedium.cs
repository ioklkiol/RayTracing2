using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//体恒量介质
public class ConstantMedium : Hitable
{
    private Hitable boundary;
    private double density;
    private Material phaseFunction;    //材质为各向异性材质

    public ConstantMedium(Hitable boundary, double density, Texture a)
    {
        this.boundary = boundary;
        this.density = density;
        this.phaseFunction = new Isotropic(a);
    }

    public override bool BoundingBox(double t0, double t1, out AABB box)
    {
        return boundary.BoundingBox(t0, t1, out box);
    }

    public override bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        HitRecord rec1, rec2;
        if (boundary.Hit(r, double.MinValue, double.MaxValue, out rec1))
        {
            if (boundary.Hit(r, rec1.t + 0.0001, double.MaxValue, out rec2))
            {
                rec1.t = Math.Max(rec1.t, tMin);
                rec2.t = Math.Min(rec2.t, tMax);
                if (rec1.t >= rec2.t)
                    return false;
                rec1.t = Math.Max(rec1.t, 0);
                double distanceInsideBoundary = (rec2.t - rec1.t) * r.Direction.Length();
                double hitDistance = -(1 / density) * Math.Log(Tools.RandomDouble());
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