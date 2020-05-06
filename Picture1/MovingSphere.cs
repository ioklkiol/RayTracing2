using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MovingSphere:Hitable
{
    private Vector3D _center0;
    private Vector3D _center1;
    private double time0;
    private double time1;
    private double radius;
    private Material mat_ptr;

    public Vector3D Center0 { get => _center0; set => _center0 = value; }
    public Vector3D Center1 { get => _center1; set => _center1 = value; }
    public double Time0 { get => time0; set => time0 = value; }
    public double Time1 { get => time1; set => time1 = value; }
    public double Radius { get => radius; set => radius = value; }
    public Material Mat_ptr { get => mat_ptr; set => mat_ptr = value; }

    public MovingSphere(Vector3D center0, Vector3D center1, double t0, double t1, double radius, Material mat_ptr)
    {
        Center0 = center0;
        Center1 = center1;
        time0 = t0;
        Time1 = t1;
        Radius = radius;
        Mat_ptr = mat_ptr;
    }

    public MovingSphere() { }

    public Vector3D Center(double time)
    {
        return Center0 + ((time - time0) / (time1 - time0)) * (Center1 - Center0);
    }
    public virtual bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        Vector3D oc = r.Origin - Center(r.Time);
        double a = r.Direction * r.Direction;
        double b = oc * r.Direction;
        double c = oc * oc - radius * radius;
        double discriminant = b * b - a * c;
        rec = new HitRecord();
        if (discriminant > 0)
        {
            double temp = (-b - Math.Sqrt(discriminant)) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.t = temp;
                rec.p = r.GetPoint(rec.t);
                rec.normal = (rec.p - Center(r.Time)) / radius;
                rec.matPtr = mat_ptr;
                return true;
            }
            temp = (-b + Math.Sqrt(discriminant)) / a;
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
}
