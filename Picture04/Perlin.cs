using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Perlin
{
    private static double[] ranDouble = PerlinGenerate();
    private static int[] permX = PerlinGeneratePerm();
    private static int[] permY = PerlinGeneratePerm();
    private static int[] permZ = PerlinGeneratePerm();

    public double Noise(Vector3D p)
    {
        double u = p.X - Math.Floor(p.X);
        double v = p.Y - Math.Floor(p.Y);
        double w = p.Z - Math.Floor(p.Z);
        int i = (int)(4 * p.X) & 255;
        int j = (int)(4 * p.Y) & 255;
        int k = (int)(4 * p.Z) & 255;
        return ranDouble[permX[i]^permY[j]^permZ[k]];
    }
    public static void Permute(int[] p, int n)
    {
        for (int i = n - 1; i > 0; i--)
        {
            int target = (int)(Tools.RandomDouble() * (i + 1));
            int temp = p[i];
            p[i] = p[target];
            p[target] = temp;
        }
        return;
    }

    public static double[] PerlinGenerate()
    {
        double[] p = new double[256];
        for (int i = 0; i < 256; i++)
            p[i] = Tools.RandomDouble();
        return p;
    }
    public static int[] PerlinGeneratePerm()
    {
        int[] p = new int[256];
        for (int i = 0; i < 256; i++)
            p[i] = i;
        Permute(p, 256);
        return p;
    }
}