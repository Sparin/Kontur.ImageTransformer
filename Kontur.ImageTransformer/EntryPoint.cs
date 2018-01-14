using Kontur.ImageTransformer.Controllers;
using Kontur.ImageTransformer.Middlewares;
using Kontur.ImageTransformer.Middlewares.Routing;
using System;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            using (var server = new AsyncHttpServer())
            {
                RoutingMiddleware routing = new RoutingMiddleware();
                routing.AddRoute<HealthController>("health");
                routing.AddRoute<HealthController>("/health/<value>");
                routing.AddRoute<ImageProcessingController>("process/<filter>/<coords>");

                server.AddMiddleware(routing);
                //server.AddMiddleware(new ImageProcessingMiddleware());
                server.Start("http://+:8080/");

                Console.ReadKey(true);
            }
        }
    }
}
