using Kontur.ImageTransformer.Middlewares;
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
                        Task.Run(() => HandleContextAsync(context));
                    }
                    else Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    // TODO: log errors
                }
            }
        }

        public void AddMiddleware(Middleware middleware)
        {
            if (pipeline.Count != 0)
                pipeline[pipeline.Count - 1].NextMiddleware = middleware;

            pipeline.Add(middleware);
        }

        private async Task HandleContextAsync(HttpListenerContext listenerContext)
        {
            // TODO: implement request handling

            try
            {
                if (pipeline.Count != 0)
                    await pipeline[0].Handle(listenerContext);
            }
            catch (Exception ex)
            {
                listenerContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                byte[] buffer = Encoding.Default.GetBytes($"{ex.Message}\r\n{ex.StackTrace}");
                listenerContext.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }

            listenerContext.Response.Close();
        }

        private readonly HttpListener listener;

        private Thread listenerThread;
        private bool disposed;
        private volatile bool isRunning;
        private List<Middleware> pipeline = new List<Middleware>();
    }
}