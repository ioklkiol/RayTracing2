using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//XY平面的矩形
public class XYRect : Hitable
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

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(x0, y0, k - 0.0001f), new Vector3D(x1, y1, k + 0.0001f));
        return true;
    }
    //判断是否击中
    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        float t = (k - r.Origin.Z) / r.Direction.Z;
        if (t < tMin || t > tMax)
            return false;
        float x = r.Origin.X + t * r.Direction.X;
        float y = r.Origin.Y + t * r.Direction.Y;
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
//XZ平面上的矩形
public class XZRect : Hitable
{
    private float x0;
    private float x1;
    private float z0;
    private float z1;
    private float k;
    private Material mat;
    public XZRect() { }
    public XZRect(float x0, float x1, float z0, float z1, float k, Material mat)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.z0 = z0;
        this.z1 = z1;
        this.k = k;
        this.mat = mat;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(x0, k - 0.0001f,z0), new Vector3D(x1, k + 0.0001f,z1));
        return true;
    }
    //判断是否击中
    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        float t = (k - r.Origin.Y) / r.Direction.Y;
        if (t < tMin || t > tMax)
            return false;
        float x = r.Origin.X + t * r.Direction.X;
        float z = r.Origin.Z + t * r.Direction.Z;
        if (x < x0 || x > x1 || z < z0 || z > z1)
            return false;
        rec.u = (x - x0) / (x1 - x0);
        rec.v = (z - z0) / (z1 - z0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(0, 1, 0);
        return true;
    }
}
//YZ平面上的矩形
public class YZRect : Hitable
{
    private float y0;
    private float y1;
    private float z0;
    private float z1;
    private float k;
    private Material mat;
    public YZRect() { }
    public YZRect(float y0, float y1, float z0, float z1, float k, Material mat)
    {
        this.y0 = y0;
        this.y1 = y1;
        this.z0 = z0;
        this.z1 = z1;
        this.k = k;
        this.mat = mat;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = new AABB(new Vector3D(k - 0.0001f,y0, z0 ), new Vector3D(k + 0.0001f,y1, z1));
        return true;
    }
    //判断是否击中
    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        float t = (k - r.Origin.X) / r.Direction.X;
        if (t < tMin || t > tMax)
            return false;
        float y = r.Origin.Y + t * r.Direction.Y;
        float z = r.Origin.Z + t * r.Direction.Z;
        if (y < y0 || y > y1 || z < z0 || z > z1)
            return false;
        rec.u = (y - y0) / (y1 - y0);
        rec.v = (z - z0) / (z1 - z0);
        rec.t = t;
        rec.matPtr = mat;
        rec.p = r.GetPoint(t);
        rec.normal = new Vector3D(1, 0, 0);
        return true;
    }
}
