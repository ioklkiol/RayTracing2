using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcDx;

public class Preview : DxWindow
{
    public override void Update()
    {
        for (int i = 0; i < Buff.Length; i++)
           Buff[i] = (byte)Tools.Range(Math.Sqrt(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4]), 0, 255);
    }
}

public class Renderer
{
    private int width = 512;
    private int height = 512;
    private HitableList world = new HitableList();
    private Preview preview= new Preview(); 


    public static Renderer main;
    public int samples = 1000;
    public double[] buff;
    public int[] changes;
    public Bitmap bmp;
    public Camera camera;
    public void Init()
    {
        main = this;
        buff = new double[width * height * 4];
        changes = new int[width * height];
        bmp = new Bitmap(width, height);
        InitScene();

       Start();

        preview.Run(new DxConfiguration("Preview", width, height));
        System.Environment.Exit(0);
    }
    private void InitScene()
    {
        Final();
    }
    private void Final()
    {

        Vector3D lookFrom = new Vector3D(578, 278, -800);
        Vector3D lookAt = new Vector3D(278, 278, 0);
        double diskToFocus = 10;
        double aperture = 0;
        double vfov = 40;
        camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
            (double)width / (double)height, aperture, diskToFocus, 0, 1);

        List<Hitable> list = new List<Hitable>();
        List<Hitable> boxList = new List<Hitable>();
        List<Hitable> boxList2 = new List<Hitable>();

        int nb = 20;
        Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73, 0.73, 0.73)));
        Material ground = new Lambertian(new ConstantTexture(new Vector3D(0.48, 0.83, 0.53)));
        Material light = new DiffuseLight(new ConstantTexture(new Vector3D(7, 7, 7)));

        for (int i = 0; i < nb; i++)
        {
            for (int j = 0; j < nb; j++)
            {
                double w = 100;
                double x0 = -1000 + i * w;
                double z0 = -1000 + j * w;
                double y0 = 0;
                double x1 = x0 + w;
                double y1 = 100 * (Tools.RandomDouble() + 0.01);
                double z1 = z0 + w;
                boxList.Add(new Box(new Vector3D(x0, y0, z0), new Vector3D(x1, y1, z1), ground));
            }
        }
        list.Add(new BVHNode(boxList, boxList.Count, 0, 1));
        list.Add(new XZRect(123, 423, 147, 412, 554, light));
        Vector3D center = new Vector3D(400, 400, 200);
        list.Add(new MovingSphere(center, center + new Vector3D(30, 0, 0), 0, 1, 50,
            new Lambertian(new ConstantTexture(new Vector3D(0.7, 0.3, 0.1)))));
        list.Add(new Sphere(new Vector3D(260, 150, 45), 50, new Dielectric(1.5)));
        list.Add(new Sphere(new Vector3D(0, 150, 145), 50, new Metal(
            new ConstantTexture(new Vector3D(0.8, 0.8, 0.9)), 10)));
        Hitable boundary = new Sphere(new Vector3D(360, 150, 145), 70, new Dielectric(1.5));
        list.Add(boundary);
        list.Add(new ConstantMedium(boundary, 0.2, new ConstantTexture(new Vector3D(0.2, 0.4, 0.9))));
        boundary = new Sphere(new Vector3D(0, 0, 0), 5000, new Dielectric(1.5));
        list.Add(new ConstantMedium(boundary, 0.0001, new ConstantTexture(new Vector3D(1, 1, 1))));

        Material emat = new Lambertian(new Imagetexture("Earth.jpg"));
        list.Add(new Sphere(new Vector3D(400, 200, 400), 100, emat));
        Texture pertext = new NoiseTexture(0.1);
        list.Add(new Sphere(new Vector3D(220, 280, 300), 80, new Lambertian(pertext)));
        int ns = 1000;
        for (int j = 0; j < ns; j++)
        {
            boxList2.Add(new Sphere(new Vector3D(165 * Tools.RandomDouble(), 165 * Tools.RandomDouble(), 165 * Tools.RandomDouble()), 10, white));
        }
        list.Add(new Translate(new RotateY(new BVHNode(boxList2, ns, 0, 1), 15), new Vector3D(-100, 270, 395)));



        world.list=list;
    }
    private class ScannerCofig
    {
        public int width;
        public int height;

        public ScannerCofig(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    private async void Start()
    {
        ThreadPool.SetMaxThreads(15, 15);
        await Task.Factory.StartNew(
            delegate { LinearScanner(new ScannerCofig(width,height)); });
        for (int i = 1; i < samples; i++)
        {
            ThreadPool.QueueUserWorkItem(LinearScanner, new ScannerCofig(width, height));
        }
    }
    private void LinearScanner(object o)
    {
        ScannerCofig config = (ScannerCofig)o;
        for (int j = 0; j < config.height; j++)
        {
            for (int i = 0; i < config.width; i++)
            {
                Vector3D color = new Vector3D(0, 0, 0);
                double u = (double)(i + Tools.RandomDouble()) / (double)width;
                double v = 1 - (double)(j + Tools.RandomDouble()) / (double)height;
                Ray ray = camera.GetRay(u, v);
                color = GetColor(ray, world, 0);      
                //color = new Vector3D(Math.Sqrt(color.X), Math.Sqrt(color.Y), Math.Sqrt(color.Z));//这里不应进行伽马校正 
                SetPixel(i, j, color);
            }
        }
    }


    private Vector3D GetColor(Ray r, HitableList world, int depth)
    {
        HitRecord rec;
        /*这里的0.001不能改为0，当tmin设0的时候会导致，遍历hitlist时候，ray的t求解出来是0，
         * hit的时候全走了else，导致递归到50层的时候，最后return的是0，* attenuation结果还是0。
         * 距离越远，散射用到random_in_unit_sphere生成的ray误差越大
         */
        if (world.Hit(r, 0.001, double.MaxValue, out rec))
        {
            Ray scattered;
            Vector3D attenuation;
            Vector3D emitted = rec.matPtr.Emitted(rec.u, rec.v, rec.p);
            if (depth < 4 && rec.matPtr.Scatter(r, rec, out attenuation, out scattered))   
            {
                Vector3D color = GetColor(scattered, world, depth + 1);      //每次光线衰减之后深度加一
                return emitted + new Vector3D(attenuation.X * color.X, attenuation.Y * color.Y, attenuation.Z * color.Z);
            }
            else
            {
                return emitted;
            }
        }
        else
        {
            //Vector3D unitDirection = r.Direction.UnitVector();
            //double t = 0.5 * (unitDirection.Y + 1);
            //return (1 - t) * new Vector3D(1, 1, 1) + t * new Vector3D(0.5, 0.7, 1);
            return new Vector3D(0, 0, 0);
        }
    }

    private void SetPixel(int x, int y, Vector3D color)
    {
        var i = width * 4 * y + x * 4;
        changes[width * y + x]++;
        buff[i] += color.X;
        buff[i + 1] += color.Y;
        buff[i + 2] += color.Z;
        buff[i + 3] += 1;
    }
}