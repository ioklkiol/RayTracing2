using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RotateY : Hitable
{
    private Hitable ptr;
    private float sinTheta;
    private float cosTheta;
    private bool hasBox;
    private AABB bBox;
    public RotateY(Hitable p, float angle)
    {
        ptr = p;
        float radians = (Mathf.PI / 180) * angle;
        sinTheta = Mathf.Sin(radians);
        cosTheta = Mathf.Cos(radians);
        hasBox = ptr.BoundingBox(0, 1, out bBox);
        Vector3D min = new Vector3D(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3D max = new Vector3D(-float.MaxValue, -float.MaxValue, -float.MaxValue);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    float x = i * bBox.Max.X + (1 - i) * bBox.Min.X;
                    float y = j * bBox.Max.Y + (1 - j) * bBox.Min.Y;
                    float z = k * bBox.Max.Z + (1 - k) * bBox.Min.Z;
                    float newx = cosTheta * x + sinTheta * z;
                    float newz = -sinTheta * x + cosTheta * z;
                    Vector3D tester = new Vector3D(newx, y, newz);
                    for (int c = 0; c < 3; c++)
                    {
                        if (tester[c] > max[c])
                            max[c] = tester[c];
                        if (tester[c] < min[c])
                            min[c] = tester[c];
                    }
                }
            }
        }
        bBox = new AABB(min, max);
    }
      
    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = bBox;
        return hasBox;
    }

    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
    {
        Vector3D origin = new Vector3D(r.Origin);
        Vector3D direction = new Vector3D(r.Direction);
        origin.X = cosTheta * r.Origin.X - sinTheta * r.Origin.Z;
        origin.Z = sinTheta * r.Origin.X + cosTheta * r.Origin.Z;
        direction.X = cosTheta * r.Direction.X - sinTheta * r.Direction.Z;
        direction.Z = sinTheta * r.Direction.X + cosTheta * r.Direction.Z;
        Ray rotatedR = new Ray(origin, direction, r.Time);
        if (ptr.Hit(rotatedR, tMin, tMax, out rec))
        {
            Vector3D p = new Vector3D(rec.p);
            Vector3D normal = new Vector3D(rec.normal);
            p.X = cosTheta * rec.p.X + sinTheta * rec.p.Z;
            p.Z = -sinTheta * rec.p.X + cosTheta * rec.p.Z;
            normal.X = cosTheta * rec.normal.X + sinTheta * rec.normal.Z;
            normal.Z = -sinTheta * rec.normal.X + cosTheta * rec.normal.Z;
            rec.p = p;
            rec.normal = normal;
            return true;
        }
        else
            return false;
    }
}