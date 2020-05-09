using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Tools
{
    private static long seed = 1;
    
    //得到一个0到1的随机数
    public static double RandomDouble()
    {
        //var seed = Guid.NewGuid().GetHashCode();    //使用Guid类得到一个接近唯一的随机数种子
        //Random r = new Random(seed);
        //int i = r.Next(0, 100000);
        //return (double)i / 100000;
        seed = (0x5DEECE66DL * seed + 0xB16) & 0xFFFFFFFFFFFFL;
        return (seed >> 16) / (float)0x100000000L;
    }
    //限制数的范围
    public static double Range(double v, double min, double max)
    {
        return (v <= min) ? min : v >= max ? max : v;
    }
}