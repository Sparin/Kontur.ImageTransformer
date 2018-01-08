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
