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

namespace Kontur.ImageTransformer.Middlewares.Routing
{
    public class RoutingMiddleware : Middleware
    {
        internal const string UriPartsPattern = @"<{0,1}[\w\d\-%._~]{1,}>{0,1}";

        Dictionary<string, Type> routes = new Dictionary<string, Type>();

        public override async Task<HttpListenerContext> Handle(HttpListenerContext context)
        {
            return await Next(context);
        }

        public RoutingMiddleware AddRoute(string pattern, Type controller)
        {
            var parts = Regex.Matches(pattern, UriPartsPattern)
                .Cast<Match>()
                .Select(x=>x.Value)
                .ToArray();

            var route = GetRoute(parts);
            routes.Add(route, controller);

            return this;
        }

        private static string GetRoute(params string[] parts)
        {
            var uriParts = GetUriParts(parts);
            StringBuilder stringBuilder = new StringBuilder(string.Empty);

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

            return stringBuilder.ToString();
        }

        private static IEnumerable<UriPart> GetUriParts(params string[] parts)
        {
            return parts.Select(x => new UriPart(x));
        }
    }
}
