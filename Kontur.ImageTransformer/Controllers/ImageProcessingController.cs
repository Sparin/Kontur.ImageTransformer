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
using NLog;

namespace Kontur.ImageTransformer.Controllers
{
    public class ImageProcessingController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

        private static GrayscaleStrategy GrayscaleStrategy = new GrayscaleStrategy();
        private static SepiaStrategy SepiaStrategy = new SepiaStrategy();

        public void Post(string filter, string coords)
        {
            Bitmap bodyBitmap = null;
            Bitmap result = null;
            MemoryStream stream = null;
            try
            {
                IRenderStrategy filterStrategy = GetFilter(filter);
                if (Context.Request.ContentLength64 > 102400 || Context.Request.ContentLength64 <= 8 || filterStrategy == null)
                {
                    Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }
                byte[] file = new byte[Context.Request.ContentLength64];

                Context.Request.InputStream.Read(file, 0, 8);
                for (int i = 0; i < 8; i++)
                    if (file[i] != PngSignature[i])
                    {
                        Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    }
                Context.Request.InputStream.Read(file, 8, (int)Context.Request.ContentLength64 - 8);
                stream = new MemoryStream(file);

                Renderer.Renderer renderer = new Renderer.Renderer();
                bodyBitmap = new Bitmap(stream);
                Rectangle cropArea = ConvertToRectangle(coords);                
                cropArea.Intersect(new Rectangle(0, 0, bodyBitmap.Width, bodyBitmap.Height));
                logger.Trace($"#{Context.Request.RequestTraceIdentifier} Rectangle of target bitmap: {cropArea.X}, {cropArea.Y}, {cropArea.Width}, {cropArea.Height}");

                if (cropArea.Width == 0 || cropArea.Height == 0)
                {
                    Context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                    return;
                }

                result = renderer.RenderBitmap(bodyBitmap, cropArea, filterStrategy).Result;

                stream.Dispose(); stream = new MemoryStream();
                result.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                Context.Response.OutputStream.Write(stream.ToArray(), 0, (int)stream.Length);

            }
            catch (OverflowException e) when (e.Message == "Value was either too large or too small for an Int32.")
            {
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            catch (ArgumentException)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bodyBitmap != null)
                    bodyBitmap.Dispose();
                if (result != null)
                    result.Dispose();
                if (stream != null)
                    stream.Dispose();
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
                    if (Regex.IsMatch(filter, @"\Athreshold\(\d{1,3}\)\z"))
                    {
                        var threshold = Convert.ToInt32(Regex.Match(filter, @"\d{1,3}").Value);
                        if (threshold >= 0 && threshold <= 100)
                            filterStrategy = new ThresholdStrategy(threshold);
                    }
                    break;
            }

            return filterStrategy;
        }
    }
}
