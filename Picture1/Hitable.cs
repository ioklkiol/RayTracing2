using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface  Hitable
{
    bool Hit(Ray r, double tMin, double tMax,out HitRecord rec);
} 

