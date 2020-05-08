using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AcDx;
using Picture14;

public class Preview : DxWindow
{
    public override void Update()
    {
        for (int i = 0; i < Buff.Length; i++)
            Buff[i] = (byte)Tools.Range(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4] + 0.5f, 0, 255);
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
        
    }
    private void InitScene()
    {
        CornellBox();
    }
    private void CornellBox()
    {

        Vector3D lookFrom = new Vector3D(278, 278, -800);
        Vector3D lookAt = new Vector3D(278, 278, 0);
        double diskToFocus = 10;
        double aperture = 0;
        double vfov = 40;
        camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
            (double)width / (double)height, aperture, diskToFocus, 0, 1);

        List<Hitable> list = new List<Hitable>();

        Material red = new Lambertian(new ConstantTexture(new Vector3D(0.65, 0.05, 0.05)));
        Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73, 0.73, 0.73)));
        Material green = new Lambertian(new ConstantTexture(new Vector3D(0.12, 0.45, 0.15)));
        Material light = new DiffuseLight(new ConstantTexture(new Vector3D(255, 255, 255)));

        list.Add(new XZRect(213, 343, 227, 332, 554, light));
        list.Add(new FlipNormals(new YZRect(0, 555, 0, 555, 555, green)));
        list.Add(new YZRect(0, 555, 0, 555, 0, red));
        list.Add(new FlipNormals(new XZRect(0, 555, 0, 555, 555, white)));
        list.Add(new XZRect(0, 555, 0, 555, 0, white));
        list.Add(new FlipNormals(new XYRect(0, 555, 0, 555, 555, white)));
        list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
           new Vector3D(165, 165, 165), white), -18), new Vector3D(130, 0, 65)));
        list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
            new Vector3D(165, 330, 165), white), 15), new Vector3D(265, 0, 295)));
        BVHNode b = new BVHNode(list, list.Count, 0, 1);
        world.list.Add(b);
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
        ThreadPool.SetMaxThreads(16, 16);
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
                color = new Vector3D(Math.Sqrt(color.X), Math.Sqrt(color.Y), Math.Sqrt(color.Z));//进行伽马校正
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