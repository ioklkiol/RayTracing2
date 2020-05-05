using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface  Hitable
{
    bool Hit(Ray r, double tMin, double tMax,out HitRecord rec);
    bool BoundingBox(double t0, double t1,out AABB box);
} 

