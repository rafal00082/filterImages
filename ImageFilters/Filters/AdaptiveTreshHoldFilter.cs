using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ImageFilters.Filters
{
    public class AdaptiveTreshHoldFilter
    {
        int s;
        float t;

        public AdaptiveTreshHoldFilter(int s, float t)
        {
            this.s = s;
            this.t = t;
        }

        public Bitmap Applay(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            var integralImage = CreateIntegralImage(image);
            Bitmap outImage = (Bitmap)image.Clone();
            var outData = outImage.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            var bytes = new byte[outData.Height * outData.Stride];
            Marshal.Copy(outData.Scan0, bytes, 0, bytes.Length);

            int S = s / 2;
            float T = 1.0f - t;

            Parallel.For(0, height, y =>
             {
                 int y1 = y - S;
                 int y2 = y + S;

                 if (y1 < 0) y1 = 0;
                 if (y2 >= height - 1) y2 = height - 1;

                 for (int x = 0; x < width; x++)
                 {
                     int x1 = x - S;
                     int x2 = x + S;

                     if (x1 < 0) x1 = 0;
                     if (x2 >= width - 1) x2 = width - 1;

                     var pixelValue = (float)((integralImage[y2, x2] + integralImage[y1, x1] - integralImage[y2, x1] - integralImage[y1, x2]) / (double)((x2 - x1) * (y2 - y1)));
                     bytes[y * outData.Stride + x] = (byte)((bytes[y * outData.Stride + x] < (int)(pixelValue * T)) ? 0 : 255);
                 }
             });
            Marshal.Copy(bytes, 0, outData.Scan0, bytes.Length);
            outImage.UnlockBits(outData);

            return outImage;
        }

        private uint[,] CreateIntegralImage(Bitmap srcImage)
        {
            int width = srcImage.Width;
            int height = srcImage.Height;
            uint[,] integralImage = new uint[height + 1, width + 1];


            BitmapData srcData = srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            //int offset = srcData.Stride - width;

            var bytes = new byte[srcImage.Height * srcData.Stride];
            Marshal.Copy(srcData.Scan0, bytes, 0, bytes.Length);
            srcImage.UnlockBits(srcData);

            for (int y = 1; y < height; y++)
            {
                uint rowSum = 0;
                for (int x = 1; x < width; x++)
                {
                    rowSum += bytes[y * srcData.Stride + x];
                    integralImage[y, x] = rowSum + integralImage[y - 1, x];
                }
            }

            return integralImage;
        }

    }
}
