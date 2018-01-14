using Kontur.ImageTransformer.Controllers;
using Kontur.ImageTransformer.Middlewares;
using Kontur.ImageTransformer.Middlewares.Routing;
using NLog;
using System;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            AsyncHttpServer server = null;
            try
            {
                server = new AsyncHttpServer();

                RoutingMiddleware routing = new RoutingMiddleware();
                routing.AddRoute<HealthController>("health");
                routing.AddRoute<HealthController>("/health/<value>");
                routing.AddRoute<ImageProcessingController>("process/<filter>/<coords>");

                server.AddMiddleware(routing);
                server.Start("http://+:8080/");

                Console.ReadKey(true);

            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                LogManager.Flush();
                throw ex;
            }
            finally
            {
                if (server != null)
                    server.Dispose();
            }
        }
    }
}
