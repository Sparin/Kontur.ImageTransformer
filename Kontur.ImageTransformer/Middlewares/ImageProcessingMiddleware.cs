using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using NLog;

namespace Kontur.ImageTransformer.Middlewares
{
    internal class ImageProcessingMiddleware : Middleware
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private int maxRequests = 0;
        internal int MaxRequests
        {
            get { return maxRequests; }
            private set
            {
                //TODO: Implement adding/remove avaiable threads on semaphore for dynamic control of perfomance
                int maxPortThreads, maxThreads;
                ThreadPool.GetMaxThreads(out maxThreads, out maxPortThreads);
                if (value < 1)
                    maxRequests = 1;
                else if (value > maxThreads)
                    maxRequests = maxThreads;
                else
                    maxRequests = value;
            }
        }


        private SemaphoreSlim semaphore;
        private ConcurrentQueue<HttpListenerContext> processingQueue = new ConcurrentQueue<HttpListenerContext>();

        internal ImageProcessingMiddleware()
        {
            int maxPortThreads = 0;
            ThreadPool.GetMaxThreads(out maxRequests, out maxPortThreads);
            semaphore = new SemaphoreSlim(MaxRequests, MaxRequests);
        }

        internal override async Task<HttpListenerContext> Handle(HttpListenerContext context)
        {
            semaphore.Wait();

            logger.Debug("{0} tasks can enter the semaphore.", semaphore.CurrentCount);

            byte[] buffer = Encoding.Default.GetBytes(String.Format("Hello, chain of middlewares! {0} tasks can enter the semaphore.", semaphore.CurrentCount));
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);

            await Task.Delay(10000);

            semaphore.Release();

            return await Next(context);
        }
    }
}
