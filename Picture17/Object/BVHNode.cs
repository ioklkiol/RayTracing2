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

    public override string ToString()
    {  
        return "<" + left + "," + right + ">";
    }

    public BVHNode(List<Hitable> l, int n, float time0, float time1)
    {    
        int Compare(Hitable a,Hitable b,int i)
        {
            AABB boxL = new AABB();
            AABB boxR = new AABB();
            if (!a.BoundingBox(0, 0, out boxL) || !b.BoundingBox(0, 0, out boxR))
                throw new Exception();
            return boxL.Min[i] - boxR.Min[i] < 0 ? -1 : 1;

        }
        List<Hitable> SplitList(List<Hitable> source, int startIndex, int endIndex)
        {
            List<Hitable> result = new List<Hitable>();
            for (int i = 0; i <= endIndex - startIndex; i++)
                result.Add(source[i + startIndex]);
            return result;
        }

        int method = (int)(3 * Mathf.Randomfloat());
        l.Sort((a, b) => Compare(a, b, method));
        switch (n)
        {
            case 1:
                left = right = l[0];
                break;
            case 2:
                left = l[0];
                right = l[1];
                break;
            default:
                left = new BVHNode(SplitList(l, 0, n / 2-1), n / 2, time0, time1);
                right = new BVHNode(SplitList(l, n / 2, n - 1), n - n / 2, time0, time1);
                break;
        }
        AABB boxLeft = new AABB();
        AABB boxRight = new AABB();
        if (!left.BoundingBox(time0, time1, out boxLeft) ||
            !right.BoundingBox(time0, time1, out boxRight))
            throw new Exception();
        box = SurroundingBox(boxLeft, boxRight);

    }
    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        box = this.box;
        return true;
    }


    public override bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
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

    public AABB SurroundingBox(AABB box0, AABB box1)
    {
        Vector3D small = new Vector3D(
            Mathf.Min(box0.Min.X, box1.Min.X),
            Mathf.Min(box0.Min.Y, box1.Min.Y),
            Mathf.Min(box0.Min.Z, box1.Min.Z));
        Vector3D big = new Vector3D(
            Mathf.Max(box0.Max.X, box1.Max.X),
            Mathf.Max(box0.Max.Y, box1.Max.Y),
            Mathf.Max(box0.Max.Z, box1.Max.Z));
        return new AABB(small, big);
    }
}