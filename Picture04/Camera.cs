using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Camera
{
    private Vector3D origin = new Vector3D(0, 0, 0);
    private Vector3D lowerLeft;
    private Vector3D horizontal;
    private Vector3D vertical;
    private Vector3D u, v, w;
    private double lensRadius;      //镜片半径
    private double _time0;          //增加开始时间和结束时间
    private double _time1;

    public Vector3D Origin { get => origin; set => origin = value; }
    public Vector3D LowerLeft { get => lowerLeft; set => lowerLeft = value; }
    public Vector3D Horizontal { get => horizontal; set => horizontal = value; }
    public Vector3D Vertical { get => vertical; set => vertical = value; }
    public double Time0 { get => _time0; set => _time0 = value; }
    public double Time1 { get => _time1; set => _time1 = value; }

    public Camera(Vector3D lookFrom,Vector3D lookAt,Vector3D vup, double vfov,double aspect,
        double aperture,double focusDist,double t0,double t1)                  //aperture:孔径  focusDIst:焦距
    {
        Time0 = t0;
        Time1 = t1;
        lensRadius = aperture / 2;
        double theta = vfov * Math.PI / 180;        //vfov即相机在垂直方向上从屏幕顶端扫描到底部所岔开的视角角度
        double halfHeight = Math.Tan(theta / 2);    //aspect：屏幕宽高比
        double halfWidth = aspect * halfHeight;

        origin = lookFrom;
        w = (lookFrom - lookAt).UnitVector();
        u = (Vector3D.Cross(vup, w)).UnitVector();
        v = Vector3D.Cross(w, u);

        lowerLeft = origin - halfWidth * focusDist * u - halfHeight*focusDist * v - focusDist* w;
        horizontal = 2*halfWidth*u*focusDist; 
        vertical = 2*halfHeight*v*focusDist;
    }
    public Ray GetRay(double s, double t)
    {
        Vector3D rd = lensRadius * RandomInUnitDisk();
        Vector3D offset = u * rd.X + v * rd.Y;
        double time = Time0 + RandomDouble() * (Time1 - Time0);
        return new Ray(origin + offset,lowerLeft + s * horizontal + t * vertical - origin - offset,time);
    }
    public double RandomDouble()
    {
        var seed = Guid.NewGuid().GetHashCode();
        Random r = new Random(seed);
        int i = r.Next(0, 100000);
        return (double)i / 100000;
    }
    //在单位圆里取随机点
    private Vector3D RandomInUnitDisk()
    {
        Vector3D p;
        do
        {
            p = 2 * new Vector3D(RandomDouble(), RandomDouble(), 0) - new Vector3D(1, 1, 0);
        } while (p * p >= 1);
        return p;
    }
}