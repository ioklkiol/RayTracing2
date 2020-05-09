using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Sphere : Hitable
{
    private Vector3D _center;
    private double _radius;
    private Material _matPtr;

    public Vector3D Center { get => _center; set => _center = value; }
    public double Radius { get => _radius; set => _radius = value; }
    public Material MatPtr { get => _matPtr; set => _matPtr = value; }

    public Sphere(Vector3D cen, double r,Material m)
    {
        Center = cen;
        Radius = r; 
        MatPtr = m;
    }
    private void GetSphereUV(Vector3D p, out double u, out double v)
    {
        double phi = Math.Atan2(p.Z, p.X);                  //x = cos(phi)*con(theta)
        double theta = Math.Asin(p.Y);                      //z = sin(phi)*cos(theta)
                                                            //y = sin(theta)

        u = 1 - (phi + Math.PI) / (2 * Math.PI);            //将u和v规格化为0到1之间
        v = (theta + Math.PI / 2) / Math.PI;
    }

    public override  bool Hit(Ray r, double tMin, double tMax,out HitRecord rec)
    {
        rec = new HitRecord();
        Vector3D oc = r.Origin - Center;
        double a = r.Direction * r.Direction;
        double b = oc * r.Direction;                
        double c = oc * oc - Radius * Radius;       
        double discriminant = b * b -  a * c;
        if (discriminant > 0)
        {
            double temp = (-b - Math.Sqrt(discriminant)) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.t = temp;
                rec.p = r.GetPoint(rec.t);
                rec.normal = (rec.p - Center) / Radius;
                rec.matPtr = MatPtr;
                GetSphereUV((rec.p - Center) / Radius,out rec.u,out rec.v);
                return true;

            }
            temp = (-b + Math.Sqrt(discriminant)) / a;
            if (temp < tMax && temp > tMin)
            {
                rec.t = temp;
                rec.p = r.GetPoint(rec.t);
                rec.normal = (rec.p - Center) / Radius;
                rec.matPtr = MatPtr;
                GetSphereUV((rec.p - Center) / Radius,out rec.u,out rec.v);
                return true;
            }

        }
        return false;
    }

    public override bool BoundingBox(double t0, double t1, out AABB box)
    {
        box = new AABB(Center - new Vector3D(Radius, Radius, Radius), Center + new Vector3D(Radius, Radius, Radius));
        return true;
    }
}

