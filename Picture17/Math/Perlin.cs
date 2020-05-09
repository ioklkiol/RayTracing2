using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Perlin
{
    private static Vector3D[] ranVec = PerlinGenerate();
    private static int[] permX = PerlinGeneratePerm();
    private static int[] permY = PerlinGeneratePerm();
    private static int[] permZ = PerlinGeneratePerm();

    public static float Noise(Vector3D p)
    {
        float u = p.X - Mathf.Floor(p.X);
        float v = p.Y - Mathf.Floor(p.Y);
        float w = p.Z - Mathf.Floor(p.Z);
        int i = (int)Mathf.Floor(p.X);
        int j = (int)Mathf.Floor(p.Y);
        int k = (int)Mathf.Floor(p.Z);
        Vector3D[,,] c = new Vector3D[2,2,2];
        for (int di = 0; di < 2; di++)
            for (int dj = 0; dj < 2; dj++)
                for (int dk = 0; dk < 2; dk++)
                    c[di,dj,dk] = ranVec[permX[(i + di) & 255] ^
                      permY[(j + dj) & 255] ^ permZ[(k + dk) & 255]];
        return PerlinInterp(c, u, v, w);
    }

    public static void Permute(int[] p, int n)
    {
        for (int i = n - 1; i > 0; i--)
        {
            int target = (int)(Mathf.Randomfloat() * (i + 1));
            int temp = p[i];
            p[i] = p[target];
            p[target] = temp;
        }
        return;
    }

    public static Vector3D[] PerlinGenerate()
    {
        Vector3D[] p = new Vector3D[256];
        for (int i = 0; i < 256; i++)
            p[i] = new Vector3D(-1+2*Mathf.Randomfloat(),
                -1+2 * Mathf.Randomfloat(),-1+ 2 * Mathf.Randomfloat()).UnitVector();
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
    //perlin插值
    public static float PerlinInterp(Vector3D[,,] c, float u, float v, float w)
    {
        float uu = u * u * (3 - 2 * u);
        float vv = v * v * (3 - 2 * v);
        float ww = w * w * (3 - 2 * w);
        float accum = 0;
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                for (int k = 0; k < 2; k++)
                {
                    accum += (i * uu + (1 - i) * (1 - uu)) *
                        (j * vv + (1 - j) * (1 - vv)) *
                        (k * ww + (1 - k) * (1 - ww)) * 
                        (c[i,j,k]* new Vector3D(u - i, v - j, w - k));
                }
        return accum;
        //return Mathf.Abs(accum);                   
    }
    //噪声扰动
    public static float Turb(Vector3D p, int depth = 7)
    {
        float accum = 0;
        Vector3D temp = p;
        float weight = 1;
        for (int i = 0; i < depth; i++)
        {
            accum += weight * Noise(temp);
            weight *= 0.5f;
            temp *= 2;
        }
        return Mathf.Abs(accum);
    }

}