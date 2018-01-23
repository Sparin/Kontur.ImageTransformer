using Amib.Threading;
using Kontur.ImageTransformer.Middlewares;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer
{
    internal class AsyncHttpServer : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly SmartThreadPool threadPool = new SmartThreadPool(new STPStartInfo() { MaxWorkerThreads = 25, MaxQueueLength = 50});
        private readonly HttpListener listener;

        private Thread listenerThread;
        private bool disposed;
        private volatile bool isRunning;
        private List<Middleware> pipeline = new List<Middleware>();

        public AsyncHttpServer()
        {
            listener = new HttpListener();
        }

        public void Start(string prefix)
        {
            lock (listener)
            {
                if (!isRunning)
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();

                    listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };
                    listenerThread.Start();

                    isRunning = true;
                }
            }
        }

        public void Stop()
        {
            lock (listener)
            {
                if (!isRunning)
                    return;

                listener.Stop();

                listenerThread.Abort();
                listenerThread.Join();

                isRunning = false;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            Stop();

            listener.Close();
        }

        private void Listen()
        {
            while (true)
            {
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        if (threadPool.MaxQueueLength == threadPool.CurrentWorkItemsCount)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                            context.Response.Close();
                        }
                        else
                            threadPool.QueueWorkItem(new WorkItemInfo() { Timeout = 1000 }, HandleContextAsync, context);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Unexpected error occurred");
                }
            }
        }

        public void AddMiddleware(Middleware middleware)
        {
            if (pipeline.Count != 0)
                pipeline[pipeline.Count - 1].NextMiddleware = middleware;

            pipeline.Add(middleware);
        }

        private object HandleContextAsync(object listenerContext)
        {
            var context = (HttpListenerContext)listenerContext;
            logger.Info($"#{context.Request.RequestTraceIdentifier} {context.Request.HttpMethod} {context.Request.Url} received from {context.Request.RemoteEndPoint}");
            try
            {
                if (pipeline.Count != 0)
                    pipeline[0].Handle(context).Wait();
                else
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                logger.Error(ex, "Unexpected error occurred");
            }

            logger.Info($"#{context.Request.RequestTraceIdentifier} {context.Request.HttpMethod} {context.Request.Url} responsed with code {context.Response.StatusCode}");
            context.Response.Close();
            return null;
        }
    }
}