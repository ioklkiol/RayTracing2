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
           Buff[i] = (byte)Mathf.Range(Mathf.Sqrt(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4]), 0, 255);
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
    public float[] buff;
    public int[] changes;
    public Bitmap bmp;
    public Camera camera;
    public void Init()
    {
        main = this;
        buff = new float[width * height * 4];
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
        float diskToFocus = 10;
        float aperture = 0;
        float vfov = 40;
        camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
            (float)width / (float)height, aperture, diskToFocus, 0, 1);

        List<Hitable> list = new List<Hitable>();
        List<Hitable> boxList = new List<Hitable>();
        List<Hitable> boxList2 = new List<Hitable>();

        int nb = 20;
        Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73f, 0.73f, 0.73f)));
        Material ground = new Lambertian(new ConstantTexture(new Vector3D(0.48f, 0.83f, 0.53f)));
        Material light = new DiffuseLight(new ConstantTexture(new Vector3D(7, 7, 7)));

        for (int i = 0; i < nb; i++)
        {
            for (int j = 0; j < nb; j++)
            {
                float w = 100;
                float x0 = -1000 + i * w;
                float z0 = -1000 + j * w;
                float y0 = 0;
                float x1 = x0 + w;
                float y1 = 100 * (Mathf.Randomfloat() + 0.01f);
                float z1 = z0 + w;
                boxList.Add(new Box(new Vector3D(x0, y0, z0), new Vector3D(x1, y1, z1), ground));
            }
        }
        list.Add(new BVHNode(boxList, boxList.Count, 0, 1));
        list.Add(new XZRect(123, 423, 147, 412, 554, light));
        Vector3D center = new Vector3D(400, 400, 200);
        list.Add(new MovingSphere(center, center + new Vector3D(30, 0, 0), 0, 1, 50,
            new Lambertian(new ConstantTexture(new Vector3D(0.7f, 0.3f, 0.1f)))));
        list.Add(new Sphere(new Vector3D(260, 150, 45), 50, new Dielectric(1.5f)));
        list.Add(new Sphere(new Vector3D(0, 150, 145), 50, new Metal(
            new ConstantTexture(new Vector3D(0.8f, 0.8f, 0.9f)), 10)));
        Hitable boundary = new Sphere(new Vector3D(360, 150, 145), 70, new Dielectric(1.5f));
        list.Add(boundary);
        list.Add(new ConstantMedium(boundary, 0.2f, new ConstantTexture(new Vector3D(0.2f, 0.4f, 0.9f))));
        boundary = new Sphere(new Vector3D(0, 0, 0), 5000, new Dielectric(1.5f));
        list.Add(new ConstantMedium(boundary, 0.0001f, new ConstantTexture(new Vector3D(1, 1, 1))));

        Material emat = new Lambertian(new Imagetexture("Earth.jpg"));
        list.Add(new Sphere(new Vector3D(400, 200, 400), 100, emat));
        Texture pertext = new NoiseTexture(0.1f);
        list.Add(new Sphere(new Vector3D(220, 280, 300), 80, new Lambertian(pertext)));
        int ns = 1000;
        for (int j = 0; j < ns; j++)
        {
            boxList2.Add(new Sphere(new Vector3D(165 * Mathf.Randomfloat(), 165 * Mathf.Randomfloat(), 165 * Mathf.Randomfloat()), 10, white));
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
                float u = (float)(i + Mathf.Randomfloat()) / (float)width;
                float v = 1 - (float)(j + Mathf.Randomfloat()) / (float)height;
                Ray ray = camera.GetRay(u, v);
                color = GetColor(ray, world, 0);      
                //color = new Vector3D(Mathf.Sqrt(color.X), Mathf.Sqrt(color.Y), Mathf.Sqrt(color.Z));//这里不应进行伽马校正 
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
        if (world.Hit(r, 0.001f, float.MaxValue, out rec))
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
            //float t = 0.5 * (unitDirection.Y + 1);
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