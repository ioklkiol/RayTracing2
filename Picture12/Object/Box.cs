using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Box : IHitable
{
    private Vector3D pMin;
    private Vector3D pMax;
    private HitableList listPtr;
    public Box(Vector3D p0,Vector3D p1,Material ptr)
    {
        pMin = p0;
        pMax = p1;
        List<IHitable> list = new List<IHitable>();
        list.Add(new XYRect(p0.X, p1.X, p0.Y, p1.Y, p1.Z, ptr));
        list.Add(new FlipNormals(new XYRect(p0.X, p1.X, p0.Y, p1.Y, p0.Z, ptr)));
        list.Add(new XZRect(p0.X, p1.X, p0.Z, p1.Z, p1.Y, ptr));
        list.Add(new FlipNormals(new XZRect(p0.X, p1.X, p0.Z, p1.Z, p0.Y, ptr)));
        list.Add(new YZRect(p0.Y, p1.Y, p0.Z, p1.Z, p1.X, ptr));
        list.Add(new FlipNormals(new YZRect(p0.Y, p1.Y, p0.Z, p1.Z, p0.X, ptr)));

        listPtr = new HitableList(list, list.Count);
    }

    public bool BoundingBox(double t0, double t1, out AABB box)
    {
        throw new NotImplementedException();
    }

    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        return listPtr.Hit(r, tMin, tMax,out rec);
    }
}
