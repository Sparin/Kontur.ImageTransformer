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

namespace Kontur.ImageTransformer.Controllers
{
    public class ImageProcessingController : Controller
    {
        // TODO: Implement checks of arguments and other logic
        public void Post(string filter, string coords)
        {
            //Context.Response.StatusCode = (int)HttpStatusCode.Found;

            Renderer.Renderer renderer = new Renderer.Renderer();
            //if(Context.Request.InputStream. != Context.Request.ContentLength64)
            //{
            //    Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return;
            //}
            Bitmap bodyBitmap = new Bitmap(Context.Request.InputStream);
            IRenderStrategy filterStrategy = null;
            switch (filter)
            {
                case "grayscale":
                    filterStrategy = new GrayscaleStrategy() { CroppingArea = new Rectangle(0, 0, 312, 212) };
                    break;
                default:
                    Context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    return;
            }

            if (filterStrategy != null)
            {

                bodyBitmap = renderer.RenderBitmap(bodyBitmap, filterStrategy).Result;
                //bodyBitmap.Save(@"H:\Desktop\image.png");
                    bodyBitmap.Save(Context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                byte[] buffer = Encoding.Default.GetBytes($"Filter: {filter}\r\nCoords: {coords}\r\n");
                Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
