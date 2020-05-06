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
        u = u * u * (3 - 2 * u);
        v = v * v * (3 - 2 * v);
        w = w * w * (3 - 2 * w);
        int i = (int)Math.Floor(p.X);
        int j = (int)Math.Floor(p.Y);
        int k = (int)Math.Floor(p.Z);
        double[][][] c = new double[2][][];
        for (int a = 0; a < 2; a++)
        {
            c[a] = new double[2][];
            for (int b = 0; b < 2; b++)
            {
                c[a][b] = new double[2];
            }
        }
        for (int di = 0; di < 2; di++)
            for (int dj = 0; dj < 2; dj++)
                for (int dk = 0; dk < 2; dk++)
                    c[di][dj][dk] = ranDouble[permX[(i + di) & 255] ^
                      permY[(j + dj) & 255] ^ permZ[(k + dk) & 255]];
        return TrilinearInterp(c, u, v, w);
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
    public static double TrilinearInterp(double[][][] c, double u, double v, double w)
    {
        double accum = 0;
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                for (int k = 0; k < 2; k++)
                    accum += (i * u + (1 - i) * (1 - u)) *
                        (j * v + (1 - j) * (1 - v)) *
                        (k * w + (1 - k) * (1 - w)) * c[i][j][k];
        return accum;
    }
}