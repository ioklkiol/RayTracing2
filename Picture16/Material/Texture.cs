﻿using System;
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

    public NoiseTexture(){}
    public NoiseTexture(double scale)
    {
        this._scale = scale;
    }

    public override Vector3D Value(double u, double v, Vector3D p)
    {
        //return new Vector3D(1, 1, 1) * noise.Noise(_scale * p);
        return new Vector3D(1, 1, 1) * 0.5*(1+(Math.Sin(_scale*p.X)+ 10* Perlin.Turb(p)));
    }
}

public class Imagetexture : Texture
{
    private int width;
    private int height;
    private byte[] data;
    private double scale;

    public Imagetexture(string file,double s = 1)
    {
        scale = s;
        Bitmap bitmap = new Bitmap(Image.FromFile(file));
        data = new byte[bitmap.Width * bitmap.Height * 3];
        width = bitmap.Width;
        height = bitmap.Height;
        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                Color c = bitmap.GetPixel(j,i);
                data[3 * j + 3 * width * i] = c.R;
                data[3 * j + 3 * width * i+1] = c.G;
                data[3 * j + 3 * width * i+2] = c.B;
            }
        }
    }

    public override Vector3D Value(double u, double v, Vector3D p)
    {
        u = u * scale % 1;
        v = v * scale % 1;

        int i = (int)(Tools.Range((u * width), 0, width - 1));
        int j = (int)(Tools.Range(((1-v)*height-0.001), 0, height - 1));

        double r =(double)data[3*i+3*width*j]/255;
        double g = (double)data[3*i+3*width*j+1]/255;
        double b = (double)data[3*i+3*width*j+2]/255;
        return new Vector3D(r, g, b);
    }
}