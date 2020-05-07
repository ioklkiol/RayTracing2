using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Texture
{
    public abstract Vector3D Value(double u, double v, Vector3D p);
}

//固体纹理
public class ConstantTexture : Texture
{
    private Vector3D _color;

    public Vector3D Color { get => _color; set => _color = value; }

    public ConstantTexture(Vector3D c)
    {
        Color = c;
    }

    public override Vector3D Value(double u, double v, Vector3D p)
    {
        return Color;
    }
}
//棋盘纹理
public class CheckerTexture : Texture
{
    private Texture _odd;
    private Texture _even;

    public Texture Odd { get => _odd; set => _odd = value; }
    public Texture Even { get => _even; set => _even = value; }

    public CheckerTexture(Texture t0, Texture t1)
    {
        Odd = t0;
        Even = t1;
    }

    public override Vector3D Value(double u, double v, Vector3D p)
    {
        double sines = Math.Sin(10 * p.X) * Math.Sin(10 * p.Y) * Math.Sin(10 * p.Z);
        if (sines < 0)
            return Odd.Value(u, v, p);
        else
            return Even.Value(u, v, p);
    }
}
//噪声纹理
public class NoiseTexture : Texture
{
    private double _scale;          //颜色变化的频率
    private Perlin noise = new Perlin();

    public NoiseTexture(){}
    public NoiseTexture(double scale)
    {
        this._scale = scale;
    }

    public override Vector3D Value(double u, double v, Vector3D p)
    {
        //return new Vector3D(1, 1, 1) * noise.Noise(_scale * p);
        return new Vector3D(1, 1, 1) * 0.5*(1+Math.Sin(_scale*p.Z+ 10* noise.Turb(p)));
    }
}

public class Imagetexture : Texture
{
    private int nx;
    private int ny;
    private Bitmap data;

    public Bitmap Data { get => data; set => data = value; }

    public Imagetexture() { }
    public Imagetexture(Bitmap pixels)
    {
        Data = pixels;
        nx = pixels.Width;
        ny = pixels.Height;
    }


    public override Vector3D Value(double u, double v, Vector3D p)
    {
        int i = (int)((u+0.5)%1 * nx);
        int j = (int)((1 - v) * ny);
        if (i < 0) i = 0;
        if (j < 0) j = 0;
        if (i > nx - 1) i = nx - 1;
        if (j > ny - 1) j = ny - 1;
        Color color = Data.GetPixel(i, j);
        double r = color.R / (double)255;
        double g = color.G / (double)255;
        double b = color.B / (double)255;
        return new Vector3D(r, g, b);
    }
}