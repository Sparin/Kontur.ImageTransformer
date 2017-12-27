using Kontur.ImageTransformer.Middlewares;
using System;

namespace Kontur.ImageTransformer
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            using (var server = new AsyncHttpServer())
            {
                server.AddMiddleware(new ImageProcessingMiddleware());
                server.Start("http://+:8080/");

                Console.ReadKey(true);
            }
        }
    }
}
