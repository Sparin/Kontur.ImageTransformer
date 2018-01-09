using Kontur.ImageTransformer.Renderer.Strategies;
using Kontur.ImageTransformer.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace Kontur.ImageTransformer.Controllers
{
    public class ImageProcessingController : Controller
    {
        private static GrayscaleStrategy GrayscaleStrategy = new GrayscaleStrategy();
        private static SepiaStrategy SepiaStrategy = new SepiaStrategy();

        public void Post(string filter, string coords)
        {
            Bitmap bodyBitmap = null;
            Bitmap result = null;
            try
            {
                if (Context.Request.ContentLength64 > 102400)
                {
                    Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                Renderer.Renderer renderer = new Renderer.Renderer();
                bodyBitmap = new Bitmap(Context.Request.InputStream);
                Rectangle cropArea = ConvertToRectangle(coords);
                cropArea.Intersect(new Rectangle(0, 0, bodyBitmap.Width, bodyBitmap.Height));
                if (cropArea.Width == 0 || cropArea.Height == 0)
                {
                    Context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                    return;
                }

                IRenderStrategy filterStrategy = GetFilter(filter);

                result = renderer.RenderBitmap(bodyBitmap, cropArea, filterStrategy).Result;

                using (MemoryStream stream = new MemoryStream())
                {
                    result.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    Context.Response.OutputStream.Write(stream.ToArray(), 0, (int)stream.Length);
                }
            }
            catch (ArgumentException)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (bodyBitmap != null)
                    bodyBitmap.Dispose();
                if (result != null)
                    result.Dispose();
            }

        }

        private static Rectangle ConvertToRectangle(string coords)
        {
            if (!Regex.IsMatch(coords, @"\A([-\d]*,{1}){3}[-\d]*\z"))
                throw new ArgumentException("Incorrect value", "coords");

            int[] args = coords.Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToArray();

            return new Rectangle(args[0], args[1], args[2], args[3]);
        }

        private static IRenderStrategy GetFilter(string filter)
        {
            IRenderStrategy filterStrategy = null;

            switch (filter)
            {
                case "grayscale":
                    filterStrategy = GrayscaleStrategy;
                    break;

                case "sepia":
                    filterStrategy = SepiaStrategy;
                    break;

                default:
                    if (Regex.IsMatch(filter, @"threshold\(\d{1,3}\)"))
                    {
                        var threshold = Convert.ToInt32(Regex.Match(filter, @"\d{1,3}").Value);
                        filterStrategy = new ThresholdStrategy(threshold);
                    }
                    else
                        throw new ArgumentException("Filter is not found");
                    break;
            }

            return filterStrategy;
        }
    }
}
