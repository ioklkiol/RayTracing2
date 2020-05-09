using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class  Hitable
{
    public abstract bool Hit(Ray r, float tMin, float tMax, out HitRecord rec) ;
    public abstract bool BoundingBox(float t0, float t1, out AABB box);
} 

