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
    public class ThresholdStrategy : IRenderStrategy
    {
        public PixelFormat PixelFormat { get; } = PixelFormat.Format32bppArgb;
        public Rectangle CroppingArea { get; set; }

        public Task<Bitmap> Process(Bitmap bitmap)
        {

            throw new NotImplementedException();
        }

        public Task<Bitmap> Process(Bitmap bitmap, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}
