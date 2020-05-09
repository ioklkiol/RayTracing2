using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MovingSphere:Hitable
{
    private Vector3D _center0;
    private Vector3D _center1;
    private float time0;
    private float time1;
    private float radius;
    private Material mat_ptr;

    public Vector3D Center0 { get => _center0; set => _center0 = value; }
    public Vector3D Center1 { get => _center1; set => _center1 = value; }
    public float Time0 { get => time0; set => time0 = value; }
    public float Time1 { get => time1; set => time1 = value; }
    public float Radius { get => radius; set => radius = value; }
    public Material Mat_ptr { get => mat_ptr; set => mat_ptr = value; }

    public MovingSphere(Vector3D center0, Vector3D center1, float t0, float t1, float radius, Material mat_ptr)
    {
        Center0 = center0;
        Center1 = center1;
        time0 = t0;
        Time1 = t1;
        Radius = radius;
        Mat_ptr = mat_ptr;
    }

    public MovingSphere() { }

    public Vector3D Center(float time)
    {
        return Center0 + ((time - time0) / (time1 - time0)) * (Center1 - Center0);
    }
    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        Vector3D oc = r.Origin - Center(r.Time);
        float a = r.Direction * r.Direction;
        float b = oc * r.Direction;
        float c = oc * oc - radius * radius;
        float discriminant = b * b - a * c;
        rec = new HitRecord();
        if (discriminant > 0)
        {
            float temp = (-b - Mathf.Sqrt(discriminant)) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.t = temp;
                rec.p = r.GetPoint(rec.t);
                rec.normal = (rec.p - Center(r.Time)) / radius;
                rec.matPtr = mat_ptr;
                return true;
            }
            temp = (-b + Mathf.Sqrt(discriminant)) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.t = temp;
                rec.p = r.GetPoint(rec.t);
                rec.normal = (rec.p - Center(r.Time)) / radius;
                rec.matPtr = mat_ptr;
                return true;
            }
        }
        return false;
    }

    public AABB SurroundingBox(AABB box0, AABB box1)
    {
        Vector3D small = new Vector3D(
            Mathf.Min(box0.Min.X, box1.Min.X),
            Mathf.Min(box0.Min.Y, box1.Min.Y),
            Mathf.Min(box0.Min.Z, box1.Min.Z));
        Vector3D big = new Vector3D(
            Mathf.Max(box0.Max.X, box1.Max.X),
            Mathf.Max(box0.Max.Y, box1.Max.Y),
            Mathf.Max(box0.Max.Z, box1.Max.Z));
        return new AABB(small, big);
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = SurroundingBox
            (new AABB(Center(t0)-new Vector3D(radius,radius,radius),
                Center(t0)+new Vector3D(radius,radius ,radius)),
             new AABB(Center(t1) - new Vector3D(radius, radius, radius),
                Center(t1) + new Vector3D(radius, radius, radius)));
        return true;
    }
}
