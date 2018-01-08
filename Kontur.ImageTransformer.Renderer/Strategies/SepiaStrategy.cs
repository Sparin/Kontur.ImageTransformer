using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Renderer.Strategies
{
    public class SepiaStrategy : IRenderStrategy
    {
        public PixelFormat PixelFormat { get; } = PixelFormat.Format32bppArgb;

        public async Task<Bitmap> Process(Bitmap bitmap, Rectangle croppingArea)
        {
            return await Process(bitmap, croppingArea, new CancellationToken());
        }

        public async Task<Bitmap> Process(Bitmap bitmap, Rectangle croppingArea, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                Bitmap resultBitmap = bitmap.Clone(croppingArea, PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat) / 8;
                int length = bytesPerPixel * croppingArea.Width * croppingArea.Height;

                BitmapData imageData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.ReadWrite, PixelFormat);
                unsafe
                {
                    byte* dataPointer = (byte*)imageData.Scan0.ToPointer();

                    for (int i = 0; i < length; i += bytesPerPixel)
                    {
                        float b = dataPointer[i], g = dataPointer[i + 1], r = dataPointer[i + 2], a = dataPointer[i + 3];

                        //Blue
                        float buffer = r * 0.272f + g * 0.534f + b * 0.131f;
                        if (buffer > 255)
                            dataPointer[i] = 255;
                        else
                            dataPointer[i] = Convert.ToByte(buffer);

                        //Green
                        buffer = r * 0.349f + g * 0.686f + b * 0.168f;
                        if (buffer > 255)
                            dataPointer[i + 1] = 255;
                        else
                            dataPointer[i + 1] = Convert.ToByte(buffer);

                        //Red
                        buffer = r * 0.393f + g * 0.769f + b * 0.189f;
                        if (buffer > 255)
                            dataPointer[i + 2] = 255;
                        else
                            dataPointer[i + 2] = Convert.ToByte(buffer);

                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }

                resultBitmap.UnlockBits(imageData);

                return resultBitmap;
            });
        }


    }
}
