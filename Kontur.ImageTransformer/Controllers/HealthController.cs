using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Controllers
{
#pragma warning disable CS1591 // Missing XML Comment for publicy visible type of member
    public class HealthController : Controller
    {
        public void Get()
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Found;
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via GET request of http protocol\r\n");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Post(string value)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Found;

            byte[] buffer = Encoding.Default.GetBytes($"Hello, my name is {value}. And this is my controller. You called me via POST request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Post()
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Found;
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via POST request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Put()
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Found;
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via PUT request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Delete()
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Found;
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via DELETE request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

    }
#pragma warning restore CS1591 // Missing XML Comment for publicy visible type of member
}
