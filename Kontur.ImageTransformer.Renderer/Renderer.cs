using Kontur.ImageTransformer.Renderer.Strategies;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Renderer
{
    /// <summary>
    /// Provides methods for representation of input images
    /// </summary>
    public class Renderer
    {
        /// <summary>
        /// Crops and renders image using <see cref="IRenderStrategy"/>
        /// </summary>
        /// <param name="bitmap">Source image</param>
        /// <param name="croppingArea">Crop region</param>
        /// <param name="strategy">Renders strategy</param>
        /// <returns>Resulted image</returns>
        public async Task<Bitmap> RenderBitmap(Bitmap bitmap, Rectangle croppingArea, IRenderStrategy strategy)
        {
            return await strategy.Process(bitmap, croppingArea);
        }
    }
}
