using System;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace CS_sandbox
{
    public struct Image_channels
    {
        public int width;
        public int height;
        public double[,] red_channel;
        public double[,] green_channel;
        public double[,] blue_channel;
    }
    class Program
    {
        static void Main(string[] args)
        {
            const int radius = 3;
            Bitmap bmp = new Bitmap("bitmap_sample2.jpg");
            bmp.Save("bitmap_sample2.png", ImageFormat.Png);
            Bitmap bitmap = new Bitmap("bitmap_sample2.png");
            //Bitmap bitmap = new Bitmap("Vaporwave_art_example.png");

            const double sigma = 10.0;
            var (gaussian_weights, sum_of_gaussian_weights) = CalculateGaussianWeights(radius, sigma);

            Image_channels ic = GetImageRGBChannelsValues(bitmap, radius);

            double average_red = 0.0;
            double average_green = 0.0;
            double average_blue = 0.0;

            for (int i = radius; i < ic.width - radius; i++)
            {
                for (int j = radius; j < ic.height - radius; j++)
                {
                    average_blue = average_green = average_red = 0.0;
                    for (int a = -radius; a <= radius; a++)
                    {
                        for (int b = -radius; b <= radius; b++)
                        {
                            average_red += ic.red_channel[i + a, j + b] * gaussian_weights[a + radius, b + radius];
                            average_blue += ic.blue_channel[i + a, j + b] * gaussian_weights[a + radius, b + radius];
                            average_green += ic.green_channel[i + a, j + b] * gaussian_weights[a + radius, b + radius];
                        }
                    }
                    average_red /= sum_of_gaussian_weights;
                    average_green /= sum_of_gaussian_weights;
                    average_blue /= sum_of_gaussian_weights;

                    ic.blue_channel[i, j] = Math.Floor(average_blue);
                    ic.green_channel[i, j] = Math.Floor(average_green);
                    ic.red_channel[i, j] = Math.Floor(average_red);
                }
            }

            GenerateAndSaveImage(bitmap, ic, radius, "image_copy.png");

            
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

        static void GenerateAndSaveImage(Bitmap bitmap, Image_channels ic, int radius, string file_name)
        {
            Color cl = Color.White;

            for (int i = radius; i < bitmap.Width + radius; i++)
            {
                for (int j = radius; j < bitmap.Height + radius; j++)
                {
                    cl = Color.FromArgb(bitmap.GetPixel(i - radius, j - radius).A,
                        (byte)ic.red_channel[i, j],
                        (byte)ic.green_channel[i, j],
                        (byte)ic.blue_channel[i, j]);
                    bitmap.SetPixel(i - radius, j - radius, cl);
                }
            }

            bitmap.Save(file_name, ImageFormat.Png);
        }

        static Image_channels GetImageRGBChannelsValues(Bitmap bmp, int radius)
        {
            Image_channels output;

            output.width = bmp.Width + (2 * radius);
            output.height = bmp.Height + (2 * radius);
            output.blue_channel = new double[output.width, output.height];
            output.red_channel = new double[output.width, output.height];
            output.green_channel = new double[output.width, output.height];

            Color temp = Color.White;

            for (int i = radius; i < output.width - radius; i++)
            {
                for (int j = radius; j < output.height - radius; j++)
                {
                    temp = bmp.GetPixel(i - radius, j - radius);
                    output.blue_channel[i, j] = temp.B;
                    output.red_channel[i, j] = temp.R;
                    output.green_channel[i, j] = temp.G;
                }
            }

            for (int i = radius; i < output.width - radius; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    output.blue_channel[i, j] = output.blue_channel[i, radius];
                    output.red_channel[i, j] = output.red_channel[i, radius];
                    output.green_channel[i, j] = output.green_channel[i, radius];

                    output.blue_channel[i, output.height - j - 1] = output.blue_channel[i, output.height - radius];
                    output.red_channel[i, output.height - j - 1] = output.red_channel[i, output.height - radius];
                    output.green_channel[i, output.height - j - 1] = output.green_channel[i, output.height - radius];
                }
            }

            for (int i = 0; i < output.height; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    output.blue_channel[j, i] = output.blue_channel[radius, i];
                    output.red_channel[j, i] = output.red_channel[radius, i];
                    output.green_channel[j, i] = output.green_channel[radius, i];

                    output.blue_channel[output.width - j - 1,i] = output.blue_channel[output.width - radius, i];
                    output.red_channel[output.width - j - 1, i] = output.red_channel[output.width - radius, i];
                    output.green_channel[output.width - j - 1, i] = output.green_channel[output.width - radius, i];
                }
            }
            return output;
        }
    }
}
