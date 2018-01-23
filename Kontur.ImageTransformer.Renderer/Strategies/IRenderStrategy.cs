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
    /// A base interface for implementation of render strategies
    /// </summary>
    public interface IRenderStrategy
    {
        /// <summary>
        /// Gets the <see cref="PixelFormat"/> of bitmap after processing bitmap
        /// </summary>       
        PixelFormat PixelFormat { get; }

        /// <summary>
        /// Creates new <see cref="Bitmap"/> with specified strategy
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="croppingArea">Crop region</param>
        /// <returns><see cref="Bitmap"/> with specified strategy</returns>
        Task<Bitmap> Process(Bitmap bitmap, Rectangle croppingArea);

        /// <summary>
        /// Creates new <see cref="Bitmap"/> with specified strategy
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="croppingArea">Crop region</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="Bitmap"/> with specified strategy</returns>
        Task<Bitmap> Process(Bitmap bitmap, Rectangle croppingArea, CancellationToken cancellationToken);
    }
}
