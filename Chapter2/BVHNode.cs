using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BVHNode : Hitable
{
    private Hitable left;
    private Hitable right;
    private AABB box;
    public BVHNode(List<Hitable> l, int n, double time0, double time1)
    {
        int axis = 3 * (int)RandomDouble();
        if (axis == 0)
            l.Sort((a, b) =>
            {
                AABB boxLeft, boxRight;
                if (!a.BoundingBox(0, 0, out boxLeft) || !b.BoundingBox(0, 0, out boxRight))
                    throw new Exception();
                return boxLeft.Min.X.CompareTo(boxRight.Min.X);
            });
        if (axis == 1)
            l.Sort((a, b) =>
            {
                AABB boxLeft, boxRight;
                if (!a.BoundingBox(0, 0, out boxLeft) || !b.BoundingBox(0, 0, out boxRight))
                    throw new Exception();
                return boxLeft.Min.Y.CompareTo(boxRight.Min.Y);
            });
        if (axis == 2)
            l.Sort((a, b) =>
            {
                AABB boxLeft, boxRight;
                if (!a.BoundingBox(0, 0, out boxLeft) || !b.BoundingBox(0, 0, out boxRight))
                    throw new Exception();
                return boxLeft.Min.Z.CompareTo(boxRight.Min.Z);
            });
        if (n == 1)
            left = right = l[0];
        else if (n == 2)
        {
            left = l[0];
            right = l[1];
        }
        else
        {
            left = new BVHNode(l, n / 2, time0, time1);
            right = new BVHNode(l.Skip(n / 2).ToList(), n - n / 2, time0, time1);
        }

    }
    public bool BoundingBox(double t0, double t1, out AABB box)
    {
        box = this.box;
        return true;
    }


    public bool Hit(Ray r, double tMin, double tMax, out HitRecord rec)
    {
        rec = new HitRecord();
        if (box.Hit(r, tMin, tMax))
        {
            HitRecord leftRec, rightRec;
            bool hitLeft = left.Hit(r, tMin, tMax, out leftRec);
            bool hitRight = right.Hit(r, tMin, tMax, out rightRec);
            if (hitLeft && hitRight)            //击中重叠部分
            {
                if (leftRec.t < rightRec.t)
                    rec = leftRec;              //左子树在前 
                else
                    rec = rightRec;             //右子树在前
                return true;
            }
            else if (hitLeft)
            {
                rec = leftRec;                  //击中左子树
                return true;
            }
            else if (hitRight)
            {
                rec = rightRec;                 //击中右子树
                return true;
            }
            else
                return false;
        }
        else
            return false;                       //未击中任何物体
    }
    //得到一个0到1的随机数
    private double RandomDouble()
    {
        var seed = Guid.NewGuid().GetHashCode();    //使用Guid类得到一个接近唯一的随机数种子
        Random r = new Random(seed);
        int i = r.Next(0, 100000);
        return (double)i / 100000;
    }

}