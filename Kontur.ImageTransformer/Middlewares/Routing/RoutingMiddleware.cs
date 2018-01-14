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
using System.Reflection;

namespace Kontur.ImageTransformer.Middlewares.Routing
{
    public class RoutingMiddleware : Middleware
    {
        internal const string UriPartsPattern = @"<{0,1}[\w\d\-%.(),_~]{1,}>{0,1}";

        Dictionary<string, Type> routes = new Dictionary<string, Type>();
        Dictionary<string, UriPart[]> contracts = new Dictionary<string, UriPart[]>();

        public override async Task<HttpListenerContext> Handle(HttpListenerContext context)
        {
            var parts = GetUriParts(context.Request.RawUrl).ToArray();
            var keys = routes.Keys.Where(x => Regex.IsMatch(context.Request.RawUrl, x)).ToArray();
            bool notFound = true;

            foreach (var key in keys)
            {
                var controllerType = routes[key];
                
                var controller = Activator.CreateInstance(controllerType);
                var contextField = typeof(Controller).GetField("<Context>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                contextField.SetValue(controller, context);

                MethodInfo method = GetMethod(controllerType, context.Request.HttpMethod, contracts[key]);
                if (method != null)
                    notFound = false;
                else
                    continue;
                method.Invoke(controller, GetParameters(method, context.Request.RawUrl, contracts[key]));

                //byte[] buffer = Encoding.Default.GetBytes(controllerType.FullName + "\r\n" + context.Request.RawUrl);
                //context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }

            if (keys.Length == 0 || notFound)
                context.Response.StatusCode = 404;

            return await Next(context);
        }

        public RoutingMiddleware AddRoute<T>(string pattern) where T : Controller
        {
            var parts = Regex.Matches(pattern, UriPartsPattern)
                .Cast<Match>()
                .Select(x => x.Value)
                .ToArray();

            var uriParts = GetUriParts(parts).ToArray();
            var route = GetRoute(uriParts);
            routes.Add(route, typeof(T));
            contracts.Add(route, uriParts);

            return this;
        }

        private static string GetRoute(params UriPart[] parts)
        {
            StringBuilder stringBuilder = new StringBuilder(@"\A");

            foreach (var part in parts)
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
            
            stringBuilder.Append("/{0,1}");
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

        private static MethodInfo GetMethod(Type controller, string httpMethod, params UriPart[] contract)
        {
            var contractParameters = contract.Where(x => x.Type == UriPartType.Dynamic).ToArray();
            var methods = controller.GetMethods()
                .Where(x => String.Compare(x.Name, httpMethod, true) == 0 && x.GetParameters().Length == contractParameters.Length)
                .Select(x => x)
                .ToArray();
            foreach (var method in methods)
            {
                bool isValid = true;
                var parameters = method.GetParameters();

                for (int i = 0; i < parameters.Length && i < contractParameters.Length && isValid; i++)
                    if (parameters[i].Name != contractParameters[i].Value)
                        isValid = false;

                if (isValid)
                    return method;
            }

            return null;
        }

        private static object[] GetParameters(MethodInfo methodInfo, string url, params UriPart[] contract)
        {
            var contractParameters = contract.Where(x => x.Type == UriPartType.Dynamic).ToArray();
            var parameters = methodInfo.GetParameters();
            object[] result = new object[parameters.Length];
            var parts = GetUriParts(url).ToArray();

            for (int i = 0; i < result.Length && i< contractParameters.Length; i++)
            {
                var index = contract.ToList().IndexOf(contractParameters[i]);
                result[i] = parts[index];
            }

            return result;
        }
    }
}
