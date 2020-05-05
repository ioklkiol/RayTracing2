using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chapter1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //得到一个0到1的随机数
        private double RandomDouble()
        {
            var seed = Guid.NewGuid().GetHashCode();    //使用Guid类得到一个接近唯一的随机数种子
            Random r = new Random(seed);
            int i = r.Next(0, 100000);
            return (double)i / 100000;
        }

        public Vector3D RandomInUnitShpere()
        {
            Vector3D p = new Vector3D();
            do
            {
                //RandomDouble生成的是0~1的随机数，然后乘以2再减去1，得到的p的每一个分量均位于-1到1，其实它的范围是一个正方体，
                //但我们要求的是球内随机点，所以判断一下x*x+y*y+z*z是否大于1
                p = 2 * new Vector3D(RandomDouble(), RandomDouble(), RandomDouble()) - new Vector3D(1, 1, 1);
            } while (p.SquaredMagnitude() >= 1);       //如果x*x+y*y+z*z大于1，则说明这个点在正方体内但不在球内，需要重新找
            return p;
        }

        private Vector3D GetColor(Ray r, HitableList world, int depth)
        {
            HitRecord rec;
            if (world.Hit(r, 0.001, double.MaxValue, out rec))
            {
                Ray scattered;
                Vector3D attenuation;
                if (depth < 50 && rec.matPtr.Scatter(r, rec, out attenuation, out scattered))   //只递归50次，避免无谓的性能浪费
                {
                    Vector3D color = GetColor(scattered, world, depth + 1);      //每次光线衰减之后深度加一
                    return new Vector3D(attenuation.X * color.X, attenuation.Y * color.Y, attenuation.Z * color.Z);
                }
                else
                {
                    return new Vector3D(0, 0, 0);
                }
            }
            else
            {
                Vector3D unitDirection = r.Direction.UnitVector();
                double t = 0.5 * (unitDirection.Y + 1);
                return (1 - t) * new Vector3D(1, 1, 1) + t * new Vector3D(0.5, 0.7, 1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int nx = 800;                       //渲染这一章的图需要的时间比较长，可能需要四五个小时，建议在relese模式下进行，也可以改小分辨率和采样率
            int ny = 400;
            int ns = 100;

            Bitmap bmp = new Bitmap(nx, ny);

            Vector3D lookFrom = new Vector3D(13, 2, 3);
            Vector3D lookAt = new Vector3D(0, 0, 0);
            double diskToFocus = (lookFrom - lookAt).Length();
            double aperture = 0;
            Camera cam = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), 20,
                (double)nx / (double)ny, aperture, 0.7 * diskToFocus,0,1);


            HitableList world = RandomScene();
            for (int j = 0; j < ny; j++)
            {
                for (int i = 0; i < nx; i++)
                {
                    Vector3D color = new Vector3D(0, 0, 0);
                    for (int s = 0; s < ns; s++)
                    {
                        double u = (double)(i + RandomDouble()) / (double)nx;
                        double v = 1 - (double)(j + RandomDouble()) / (double)ny;
                        Ray ray = cam.GetRay(u, v);
                        color += GetColor(ray, world, 0);      //将所有采样点的颜色相加
                    }
                    color /= ns;                            //除以采样点的数量得到平均值
                    color = new Vector3D(Math.Sqrt(color.X), Math.Sqrt(color.Y), Math.Sqrt(color.Z));//进行伽马校正
                    int r = (int)(255 * color.X);
                    int g = (int)(255 * color.Y);
                    int b = (int)(255 * color.Z);
                    bmp.SetPixel(i, j, Color.FromArgb(r, g, b));

                }

            }
            pictureBox1.BackgroundImage = bmp;
        }

        private HitableList RandomScene()
        {
            List<Hitable> list = new List<Hitable>();
            list.Add(new Sphere(new Vector3D(0, -1000, 0), 1000, new Lambertian(new Vector3D(0.5, 0.5, 0.5))));
            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    double chooseMat = RandomDouble();
                    Vector3D center = new Vector3D(a + 0.9 * RandomDouble(), 0.2, b + 0.9 * RandomDouble());
                    if ((center - new Vector3D(4, 0.2, 0)).Length() > 0.9)
                    {
                        if (chooseMat < 0.8)
                        {
                            list.Add(new MovingSphere(center,center+new Vector3D(0,0.5*RandomDouble(),0),0,1, 0.2, 
                                new Lambertian(new Vector3D(
                                                            RandomDouble() * RandomDouble(),
                                                            RandomDouble() * RandomDouble(),
                                                            RandomDouble() * RandomDouble()))));
                        }
                        else if (chooseMat < 0.95)
                        {
                            list.Add(new Sphere(center, 0.2, new Metal(new Vector3D(
                                                                0.5 * (1 + RandomDouble()),
                                                                0.5 * (1 + RandomDouble()),
                                                                0.5 * (1 + RandomDouble())),
                                                                0.5 * (1 + RandomDouble()))));
                        }
                        else
                        {
                            list.Add(new Sphere(center, 0.2, new Dielectric(1.5)));
                        }
                    }
                }
            }
            list.Add(new Sphere(new Vector3D(0, 1, 0), 1, new Dielectric(1.5)));
            list.Add(new Sphere(new Vector3D(-4, 1, 0), 1, new Lambertian(new Vector3D(0.4, 0.2, 0.1))));
            list.Add(new Sphere(new Vector3D(4, 1, 0), 1, new Metal(new Vector3D(0.7, 0.6, 0.5), 0)));
            return new HitableList(list, list.Count);

        }

    }
}
