using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Material
{
    private Texture _albedo;       
    private float _refIdx;        //第一介质折射率ni和第二介质nt的比值，可能是ni/nt也可能是nt/ni
    public Texture Albedo { get => _albedo; set => _albedo = value; }
    public float RefIdx { get => _refIdx; set => _refIdx = value; }

    protected Material(Texture albedo)
    {
        Albedo = albedo;
    }
    protected Material(float ri) 
    {
        RefIdx = ri;
    }
    public abstract bool Scatter(Ray rIn, HitRecord rec, out Vector3D attenuation, out Ray scattered);
    public virtual Vector3D Emitted(float u, float v, Vector3D p)
    {
        return new Vector3D(0, 0, 0);
    }
    //得到反射光线
    public Vector3D Reflect(Vector3D v, Vector3D n)
    {
        return v - 2 * v * n * n;
    }
    //引入反射系数
    public  float Schlick(float cosine, float ref_idx)
    {
        float r0 = (1 - RefIdx) / (1 + RefIdx);
        r0 = r0 * r0;
        return r0 + (1 - r0) * Mathf.Pow((1 - cosine), 5);
    }
    public bool Refract(Vector3D v, Vector3D n, float ni_over_nt,out Vector3D refracted)
    {
        n.Normalize();                          //得到单位向量
        v.Normalize(); 
                                                //设入射角为a，折射角为b，
        float dt = v * n;                      //得到cos(PI-a)
        float discriminant = 1 - ni_over_nt * ni_over_nt*(1 - dt * dt);    //得到cos(b)，同时也是判别式
        if (discriminant > 0)                                               //如果判别式大于0则说明折射角小于90度，即没有发生全反射
        {
            refracted = ni_over_nt * (v -n*dt) - n * Mathf.Sqrt(discriminant);
            return true;
        }
        else
        {
            refracted = new Vector3D();
            return false;
        }
    }

    public  Vector3D RandomInUnitShpere()
    {
        Vector3D p = new Vector3D();
        do
        {
            p = 2 * new Vector3D(Mathf.Randomfloat(), Mathf.Randomfloat(), Mathf.Randomfloat()) - new Vector3D(1, 1, 1);
        } while (p.SquaredMagnitude() >= 1);
        return p;
    }
}
//Lambert材质
public class Lambertian : Material
{
    public Lambertian(Texture albedo) : base(albedo)
    {
    }

    public override bool Scatter(Ray rIn, HitRecord rec, out Vector3D attenuation, out Ray scattered)
    {
        Vector3D target = rec.p + rec.normal + RandomInUnitShpere();
        scattered = new Ray(rec.p, target - rec.p,rIn.Time);
        attenuation = Albedo.Value(rec.u,rec.v,rec.p);
        return true;
    }
}
//金属材质
public class Metal : Material
{
    private float _fuzz;      //镜面模糊系数

    public float Fuzz { get => _fuzz; set => _fuzz = value; }

    public Metal(Texture albedo,float f) : base(albedo)
    {
        Fuzz = Mathf.Min(f, 1);
    }


    public override bool Scatter(Ray rIn, HitRecord rec, out Vector3D attenuation, out Ray scattered)
    {
        Vector3D reflected = Reflect(rIn.Direction.UnitVector(), rec.normal);
        scattered = new Ray(rec.p, reflected+Fuzz*RandomInUnitShpere(), rIn.Time);     //模糊镜面反射 = 镜面反射 + 模糊系数 * 单位球随机点漫反射
        attenuation = Albedo.Value(0,0,rec.p);
        return scattered.Direction * rec.normal > 0;


    }
}
//电介质材质(折射)
public class Dielectric : Material
{
    

    public Dielectric(float ri) : base(ri)
    {
        
    }
    public override bool Scatter(Ray rIn, HitRecord rec, out Vector3D attenuation, out Ray scattered)
    {
        Vector3D outwardNormal;
        Vector3D reflected = Reflect(rIn.Direction, rec.normal);
        float ni_over_nt;      //第一介质与第二介质的折射率的比值ni/nt
        attenuation = new Vector3D(1, 1, 1);
        Vector3D refracted;

        float reflect_prob;
        float cosine;

        if (rIn.Direction * rec.normal > 0)    //如果大于0则说明法向量反了，需要翻转法向量
        {
            outwardNormal = -rec.normal;
            ni_over_nt = RefIdx;
            cosine = RefIdx * (rIn.Direction * rec.normal)/rIn.Direction.Length();
        }
        else
        {
            outwardNormal = rec.normal;
            ni_over_nt = 1 / RefIdx;
            cosine = -(rIn.Direction * rec.normal)/rIn.Direction.Length();
        }
        if (Refract(rIn.Direction, outwardNormal, ni_over_nt, out refracted))
        {
            reflect_prob = Schlick(cosine, RefIdx);
        }
        else
        {
            reflect_prob = 1;
        }
        if (Mathf.Randomfloat() < reflect_prob)
        {
            scattered = new Ray(rec.p, reflected, rIn.Time);
        }
        else
        {
            scattered = new Ray(rec.p, refracted, rIn.Time);
        }
        return true;
    }
}
//自发光材质
public class DiffuseLight : Material
{
    private Texture emit;
    public DiffuseLight(Texture albedo) : base(albedo)
    {
        emit = albedo;
    }

    public override bool Scatter(Ray rIn, HitRecord rec, out Vector3D attenuation, out Ray scattered)
    {
        attenuation = new Vector3D();
        scattered = new Ray(new Vector3D(),new Vector3D(),0);
        return false;
    }
    public override Vector3D Emitted(float u, float v, Vector3D p)
    {
        Vector3D a = emit.Value(u, v, p);
        return emit.Value(u, v, p);
    }
}
//各向异性材质
public class Isotropic : Material
{
    public Isotropic(Texture albedo) : base(albedo)
    {
    }

    public override bool Scatter(Ray rIn, HitRecord rec, out Vector3D attenuation, out Ray scattered)
    {
        scattered = new Ray(rec.p, RandomInUnitShpere(), rIn.Time);
        attenuation = Albedo.Value(rec.u, rec.v, rec.p);
        return true;
    }
}