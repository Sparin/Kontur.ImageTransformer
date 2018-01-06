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
    public class GrayscaleStrategy : IRenderStrategy
    {
        public PixelFormat PixelFormat { get; } = PixelFormat.Format32bppArgb;
        public Rectangle CroppingArea { get; set; }

        BitmapData imageData;

        public async Task<Bitmap> Process(Bitmap bitmap)
        {
            return await Process(bitmap, new CancellationToken());
        }

        public async Task<Bitmap> Process(Bitmap bitmap, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                Bitmap resultBitmap = bitmap.Clone(CroppingArea, PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat) / 8;
                int length = bytesPerPixel * CroppingArea.Width * CroppingArea.Height;

                imageData = resultBitmap.LockBits(CroppingArea, ImageLockMode.ReadWrite, PixelFormat);
                unsafe
                {
                    byte* dataPointer = (byte*)imageData.Scan0.ToPointer();

                    for (int i = 0; i < length; i += bytesPerPixel)
                    {
                        dataPointer[i] = dataPointer[i];
                        //byte b = dataPointer[i], g = dataPointer[i + 1], r = dataPointer[i + 2], a = dataPointer[i + 3];
                        byte intensity = (byte)((dataPointer[i + 0] + dataPointer[i + 1] + dataPointer[i + 2]) / 3);
                        dataPointer[i] = intensity;
                        dataPointer[i + 1] = intensity;
                        dataPointer[i + 2] = intensity;

                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }

                resultBitmap.UnlockBits(imageData);

                return resultBitmap;
            });
        }
    }
}
