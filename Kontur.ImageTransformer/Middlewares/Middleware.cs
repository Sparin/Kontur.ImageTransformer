using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Middlewares
{

    /// <summary>
    /// Provides a base class for implementation parts of pipeline
    /// </summary>
    public abstract class Middleware
    {
        internal Middleware NextMiddleware = null;

        /// <summary>
        /// Transfers control of <see cref="HttpListenerContext"/> to the next middleware (handler) of pipeline
        /// </summary>
        /// <param name="context">Context to handle</param>
        /// <returns>Same context with changes after other pipelines</returns>
        public async Task<HttpListenerContext> Next(HttpListenerContext context)
        {
            if (NextMiddleware is null)
                return context;
            else
                return await NextMiddleware.Handle(context);
        }

        /// <summary>
        /// Handles the <see cref="HttpListenerContext"/> and commiting changes for request
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Same context with changes after other pipelines</returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task<HttpListenerContext> Handle(HttpListenerContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            context.Response.Close();
            return null;
        }
    }
}
