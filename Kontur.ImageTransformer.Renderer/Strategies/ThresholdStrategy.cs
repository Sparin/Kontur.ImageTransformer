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
    /// <summary>
    /// Provides threshold filter for images/renderer
    /// </summary>
    public class ThresholdStrategy : IRenderStrategy
    {
        /// <summary>
        /// Gets the pixel format of bitmap after processing bitmap
        /// </summary>       
        public PixelFormat PixelFormat { get; } = PixelFormat.Format32bppArgb;

        private int threshold = 0;
        /// <summary>
        /// Gets or sets threshold
        /// </summary>
        public int Threshold
        {
            get { return threshold; }
            set
            {
                if (value >= 0 && value <= 100)
                    threshold = value;
                else
                    throw new ArgumentOutOfRangeException("Threshold must be between 0 and 100");
            }
        }
        
        /// <summary>
        /// Initialize new instance of the <see cref="ThresholdStrategy"/>
        /// </summary>
        public ThresholdStrategy() : this(0) { }

        /// <summary>
        /// Initialize new instance of the <see cref="ThresholdStrategy"/> with <paramref name="threshold"/> value
        /// </summary>
        /// <param name="threshold">Threshold value</param>
        public ThresholdStrategy(int threshold)
        {
            Threshold = threshold;
        }

        /// <summary>
        /// Creates new cropped <see cref="Bitmap"/> with threshold filter
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="croppingArea">Crop region</param>
        /// <returns>Cropped <see cref="Bitmap"/> with sepia filter</returns>
        public async Task<Bitmap> Process(Bitmap bitmap, Rectangle croppingArea)
        {
            return await Process(bitmap, croppingArea, new CancellationToken());
        }

        /// <summary>
        /// Creates new cropped <see cref="Bitmap"/> with threshold filter
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="croppingArea">Crop region</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Cropped <see cref="Bitmap"/> with sepia filter</returns>
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
                    int thresholdSensitivity = 255 * Threshold / 100;
                    for (int i = 0; i < length; i += bytesPerPixel)
                    {
                        byte intensity = (byte)((dataPointer[i + 0] + dataPointer[i + 1] + dataPointer[i + 2]) / 3);

                        if (intensity >= thresholdSensitivity)
                        {
                            dataPointer[i] = 255;
                            dataPointer[i + 1] = 255;
                            dataPointer[i + 2] = 255;
                        }
                        else
                        {
                            dataPointer[i] = 0;
                            dataPointer[i + 1] = 0;
                            dataPointer[i + 2] = 0;
                        }

                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }

                resultBitmap.UnlockBits(imageData);

                return resultBitmap;
            });
        }
    }
}
