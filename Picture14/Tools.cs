using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Tools
{
    //得到一个0到1的随机数
    public static double RandomDouble()
    {
        var seed = Guid.NewGuid().GetHashCode();    //使用Guid类得到一个接近唯一的随机数种子
        Random r = new Random(seed);
        int i = r.Next(0, 100000);
        return (double)i / 100000;
    }
    //限制数的范围
    public static double Range(double v, double min, double max)
    {
        return (v <= min) ? min : v >= max ? max : v;
    }
}