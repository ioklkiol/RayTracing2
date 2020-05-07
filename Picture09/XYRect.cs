using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//XY平面的矩形
public class XYRect : IHitable
{
    private float x0;
    private float x1;
    private float y0;
    private float y1;
    private float k;
    private Material mat;
    public XYRect() { }
    public XYRect(float x0, float x1, float y0, float y1, float k, Material mat)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.y0 = y0;
        this.y1 = y1;
        this.k = k;
        this.mat = mat;
    }

    public bool BoundingBox(double t0, double t1, out AABB box)
    {
        box = new AABB(new Vector3D(x0, y0, k - 0.0001), new Vector3D(x1, y1, k + 0.0001));
        return true;
    }
    //判断是否击中
    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        double t = (k - r.Origin.Z) / r.Direction.Z;
        if (t < tMin || t > tMax)
            return false;
        double x = r.Origin.X + t * r.Direction.X;
        double y = r.Origin.Y + t * r.Direction.Y;
        if (x < x0 || x > x1 || y < y0 || y > y1)
            return false;
        rec.u = (x - x0) / (x1 - x0);
        rec.v = (y - y0) / (y1 - y0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(0, 0, 1);
        return true;
    }
}