using Kontur.ImageTransformer.Renderer.Strategies;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Renderer
{
    public class Renderer
    {
        public async Task<Bitmap> RenderBitmap(Bitmap bitmap, IRenderStrategy strategy)
        {
            return await strategy.Process(bitmap);
        }
    }
}
