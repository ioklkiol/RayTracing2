using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class  Hitable
{
    public abstract bool Hit(Ray r, double tMin, double tMax, out HitRecord rec) ;
    public abstract bool BoundingBox(double t0, double t1, out AABB box);
} 

