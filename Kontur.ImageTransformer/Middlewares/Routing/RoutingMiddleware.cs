using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Middlewares;
using System.Text.RegularExpressions;
using Kontur.ImageTransformer.Middlewares.Routing.Models;
using Kontur.ImageTransformer.Middlewares.Routing.Enums;
using Kontur.ImageTransformer.Controllers;

namespace Kontur.ImageTransformer.Middlewares.Routing
{
    public class RoutingMiddleware : Middleware
    {
        internal const string UriPartsPattern = @"<{0,1}[\w\d\-%._~]{1,}>{0,1}";

        Dictionary<string, Type> routes = new Dictionary<string, Type>();

        public override async Task<HttpListenerContext> Handle(HttpListenerContext context)
        {
            var parts = GetUriParts(context.Request.RawUrl).ToArray();
            var keys = routes.Keys.Where(x => Regex.IsMatch(context.Request.RawUrl, x)).ToArray();

            foreach (var key in keys)
            {
                var controllerType = routes[key];

                //TODO: Implement transfer of control on initiated controller
                byte[] buffer = Encoding.Default.GetBytes(controllerType.FullName + "\r\n" + context.Request.RawUrl);
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }

            if (keys.Length == 0)
                context.Response.StatusCode = 404;

            return await Next(context);
        }

        public RoutingMiddleware AddRoute<T>(string pattern) where T : Controller
        {
            var parts = Regex.Matches(pattern, UriPartsPattern)
                .Cast<Match>()
                .Select(x => x.Value)
                .ToArray();

            var route = GetRoute(parts);
            routes.Add(route, typeof(T));

            return this;
        }

        private static string GetRoute(params string[] parts)
        {
            var uriParts = GetUriParts(parts);
            StringBuilder stringBuilder = new StringBuilder(@"\A");

            foreach (var part in uriParts)
                switch (part.Type)
                {
                    case UriPartType.Dynamic:
                        stringBuilder.Append('/' + UriPartsPattern);
                        break;
                    case UriPartType.Static:
                        stringBuilder.Append('/' + part.Value);
                        break;
                    default:
                        throw new ArgumentException("Unsupported type part of URI");

                }

            if (uriParts.Count() == 0)
                stringBuilder.Append("/");
            stringBuilder.Append(@"\z");

            return stringBuilder.ToString();
        }

        private static IEnumerable<UriPart> GetUriParts(params string[] parts)
        {
            return parts.Select(x => new UriPart(x));
        }

        private static IEnumerable<string> GetUriParts(string url)
        {
            return Regex.Matches(url, UriPartsPattern)
                .Cast<Match>()
                .Select(x => x.Value);
        }
    }
}
