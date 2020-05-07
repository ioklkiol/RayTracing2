using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Picture9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
            if (world.Hit(r, 0.001, double.MaxValue, out rec))
            {
                Ray scattered;
                Vector3D attenuation;
                Vector3D emitted = rec.matPtr.Emitted(rec.u, rec.v, rec.p);
                if (depth < 50 && rec.matPtr.Scatter(r, rec, out attenuation, out scattered))   //只递归50次，避免无谓的性能浪费
                {
                    Vector3D color = GetColor(scattered, world, depth + 1);      //每次光线衰减之后深度加一
                    return emitted+ new Vector3D(attenuation.X * color.X, attenuation.Y * color.Y, attenuation.Z * color.Z);
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
            int nx = 800;                       
            int ny = 400;
            int ns = 100;
            
            Bitmap bmp = new Bitmap(nx, ny);

            Vector3D lookAt = new Vector3D(0, 0, 0);
            Vector3D lookFrom = new Vector3D(13, 2, 3);
            double diskToFocus = 10;
            double aperture = 0;
            Camera cam = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), 20,
                (double)nx / (double)ny, aperture, diskToFocus, 0, 1);

            HitableList world = SimpleLight();
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
                    int r = (int)(255 * color.X) ;
                    int g = (int)(255 * color.Y) ;
                    int b = (int)(255 * color.Z) ;
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

        private HitableList SimpleLight()
        {
            Texture perText = new NoiseTexture(4);
            List<IHitable> list = new List<IHitable>();

            list.Add(new Sphere(new Vector3D(0, -1000, 0), 1000, new Lambertian(perText)));    
            list.Add(new Sphere(new Vector3D(0, 2, 0), 2, new Lambertian(perText)));    
            list.Add(new Sphere(new Vector3D(0, 7, 0), 2, new DiffuseLight(new ConstantTexture(new Vector3D(4,4,4)))));    
            list.Add(new XYRect(3,5,1,3,-2, new DiffuseLight(new ConstantTexture(new Vector3D(4,4,4)))));    
    
            return new HitableList(list, list.Count);

        }

    }
}
