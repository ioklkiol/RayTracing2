using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Picture14
{
    public partial class Form1 : Form
    {
        public static Form1 main;
        private Renderer renderer = new Renderer();

        public Form1()
        {
            InitializeComponent();
            main = this;
            renderer.Init();
            //this.WindowState = FormWindowState.Minimized;
            //this.ShowInTaskbar = false;
        }
        public Vector3D RandomInUnitShpere()
        {
            Vector3D p = new Vector3D();
            do
            {
                //RandomDouble生成的是0~1的随机数，然后乘以2再减去1，得到的p的每一个分量均位于-1到1，其实它的范围是一个正方体，
                //但我们要求的是球内随机点，所以判断一下x*x+y*y+z*z是否大于1
                p = 2 * new Vector3D(Tools.RandomDouble(), Tools.RandomDouble(), Tools.RandomDouble()) - new Vector3D(1, 1, 1);
            } while (p.SquaredMagnitude() >= 1);       //如果x*x+y*y+z*z大于1，则说明这个点在正方体内但不在球内，需要重新找
            return p;
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
                if (depth < 50 && rec.matPtr.Scatter(r, rec, out attenuation, out scattered))   //只递归50次，避免无谓的性能浪费
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

        private void Form1_Load(object sender, EventArgs e)
        {
            int nx = 100;
            int ny = 100;
            int ns = 100;

            Bitmap bmp = new Bitmap(nx, ny);

            Vector3D lookFrom = new Vector3D(278, 278, -800);
            Vector3D lookAt = new Vector3D(278, 278, 0);
            double diskToFocus = 10;
            double aperture = 0;
            double vfov = 40;
            Camera cam = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
                (double)nx / (double)ny, aperture, diskToFocus, 0, 1);

            HitableList world = CornellBox();
            for (int j = 0; j < ny; j++)
            {
                for (int i = 0; i < nx; i++)
                {
                    Vector3D color = new Vector3D(0, 0, 0);
                    for (int s = 0; s < ns; s++)
                    {
                        double u = (double)(i + Tools.RandomDouble()) / (double)nx;
                        double v = 1 - (double)(j + Tools.RandomDouble()) / (double)ny;
                        Ray ray = cam.GetRay(u, v);
                        color += GetColor(ray, world, 0);      //将所有采样点的颜色相加
                    }
                    color /= ns;                            //除以采样点的数量得到平均值
                    color = new Vector3D(Math.Sqrt(color.X), Math.Sqrt(color.Y), Math.Sqrt(color.Z));//进行伽马校正
                    int r = (int)(255 * color.X);
                    int g = (int)(255 * color.Y);
                    int b = (int)(255 * color.Z);
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;
                    bmp.SetPixel(i, j, Color.FromArgb(r, g, b));

                }

            }
            pictureBox1.BackgroundImage = bmp;
        }

        private HitableList CornellBox()
        {
            HitableList world = new HitableList();
            List<Hitable> list = new List<Hitable>();

            Material red = new Lambertian(new ConstantTexture(new Vector3D(0.65, 0.05, 0.05)));
            Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73, 0.73, 0.73)));
            Material green = new Lambertian(new ConstantTexture(new Vector3D(0.12, 0.45, 0.15)));
            Material light = new DiffuseLight(new ConstantTexture(new Vector3D(55, 55, 55)));

            list.Add(new FlipNormals(new YZRect(0, 555, 0, 555, 555, green)));
            list.Add(new YZRect(0, 555, 0, 555, 0, red));
            list.Add(new XZRect(213, 343, 227, 332, 554, light));
            list.Add(new FlipNormals(new XZRect(0, 555, 0, 555, 555, white)));
            list.Add(new XZRect(0, 555, 0, 555, 0, white));
            list.Add(new FlipNormals(new XYRect(0, 555, 0, 555, 555, white)));
            list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
               new Vector3D(165, 165, 165), white), -18), new Vector3D(130, 0, 65)));
            list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
                new Vector3D(165, 330, 165), white), 15), new Vector3D(265, 0, 295)));
            world.list.Add(new BVHNode(list, list.Count, 0, 1));
            return world;

        }
    }

}

