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
    public interface IRenderStrategy
    {
        PixelFormat PixelFormat { get; }
        Rectangle CroppingArea { set; }

        Task<Bitmap> Process(Bitmap bitmap);
        Task<Bitmap> Process(Bitmap bitmap, CancellationToken cancellationToken);
    }
}
