using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Middlewares
{
    public abstract class Middleware
    {
        internal Middleware NextMiddleware = null;

        public async Task<HttpListenerContext> Next(HttpListenerContext context)
        {
            if (NextMiddleware is null)
                return context;
            else
                return await NextMiddleware.Handle(context);
        }

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
