using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HitableList
{
    public List<Hitable> list=new List<Hitable>();


    public HitableList() { }
    public HitableList(List<Hitable> list)
    {
        this.list = list;
    }

    public virtual bool Hit(Ray r, float tMin, float tMax,out HitRecord rec)
    {
        rec = new HitRecord();
        HitRecord tempRec=new HitRecord();
        bool hitAnything = false;
        float closestSoFar = tMax;       //没有物体遮挡的情况下我们可以看无限远
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Hit(r, tMin, closestSoFar, out tempRec))
            {
                hitAnything = true;
                closestSoFar = tempRec.t;   //当有物体遮挡视线之后视线可达到的最远处就是上一个击中点
                rec = tempRec;
            }
        }
        return hitAnything;
    }
}