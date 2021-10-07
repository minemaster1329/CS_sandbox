using System;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace CS_sandbox
{
    struct image_channels
    {
        int width;
        int height;
        double[,] red_channel;
        double[,] green_channel;
        double[,] blue_channel;
    }
    class Program
    {
        static void Main(string[] args)
        {
            const int radius = 1;
            Bitmap bmp = new Bitmap("bitmap_sample2.jpg");
            bmp.Save("bitmap_sample2.png", ImageFormat.Png);
            Bitmap bitmap = new Bitmap("bitmap_sample2.png");
            //Bitmap bitmap = new Bitmap("pnggrad16rgb.png");

            var (bitmap_red, bitmap_green, bitmap_blue) = GetDoubleArrayOfChannels(bitmap, radius);

            const double sigma = 3;
            var (gaussian_weights, sum_of_gaussian_weights) = CalculateGaussianWeights(1, sigma);

            double average_red = 0.0;
            double average_green = 0.0;
            double average_blue = 0.0;

            Console.WriteLine(bitmap_red[100, 100]);

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    average_blue = average_green = average_red = 0.0;
                    for (int a = -1; a < 2; a++)
                    {
                        for (int b = -1; b < 2; b++)
                        {
                            average_red += bitmap_red[i + a, j + b] * gaussian_weights[a + 1, b + 1];
                            average_blue += bitmap_blue[i + a, j + b] * gaussian_weights[a + 1, b + 1];
                            average_green += bitmap_green[i + a, j + b] * gaussian_weights[a + 1, b + 1];
                        }
                    }
                    average_red /= sum_of_gaussian_weights;
                    average_green /= sum_of_gaussian_weights;
                    average_blue /= sum_of_gaussian_weights;

                    bitmap_blue[i, j] = Math.Floor(average_blue);
                    bitmap_green[i, j] = Math.Floor(average_green);
                    bitmap_red[i, j] = Math.Floor(average_red);
                }
            }

            Console.WriteLine(bitmap_red[100, 100]);

            Color cl = Color.White;

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    cl = Color.FromArgb(bitmap.GetPixel(i, j).A, (byte)bitmap_red[i, j], (byte)bitmap_green[i, j], (byte)bitmap_blue[i, j]);
                    bitmap.SetPixel(i, j, cl);
                }
            }

            bitmap.Save("bitmap_copy.png", ImageFormat.Png);
            Console.ReadKey();
        }

        static double CalculateGaussianWeight(int x, int y, double sigma) => 1.0 / (2.0 * Math.PI * Math.Pow(sigma, 2.0)) * Math.Exp(-1.0 * (Math.Pow(x, 2.0) + Math.Pow(y, 2.0))/(2.0 * Math.Pow(sigma, 2.0)));

        static (double[,], double) CalculateGaussianWeights(int radius, double sigma)
        {
            int dims = 2 * radius + 1;
            double[,] weights_matrix = new double[2 * radius + 1, 2 * radius + 1];
            double sum_of_weights = 0.0;
            double temp = 0.0;

            for (int i = 0; i < dims; i++)
            {
                for (int j = 0; j < dims; j++)
                {
                    temp = CalculateGaussianWeight(i - radius, j - radius, sigma);
                    weights_matrix[i, j] = temp;
                    sum_of_weights += temp;
                }
            }

            return (weights_matrix, sum_of_weights);
        }

        static (double[,], double[,], double[,]) GetDoubleArrayOfChannels(Bitmap bitmap, int radius)
        {
            double[,] bitmap_red = new double[bitmap.Width, bitmap.Height];
            double[,] bitmap_green = new double[bitmap.Width, bitmap.Height];
            double[,] bitmap_blue = new double[bitmap.Width, bitmap.Height];

            Color cl = Color.White;

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    cl = bitmap.GetPixel(i, j);

                    bitmap_red[i, j] = cl.R;
                    bitmap_green[i, j] = cl.G;
                    bitmap_blue[i, j] = cl.B;
                }
            }

            return (bitmap_red, bitmap_blue, bitmap_green);
        }

        static void GenerateAndSaveImage(Bitmap bmp, image_channels image_channels)
        {

        }
    }
}
