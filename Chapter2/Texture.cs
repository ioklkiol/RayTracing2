using System;
using System.Collections.Generic;
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