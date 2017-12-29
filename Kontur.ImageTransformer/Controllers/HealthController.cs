using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Controllers
{
    public class HealthController : Controller
    {
        public void Get()
        {
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via GET request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Post()
        {
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via POST request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Put()
        {
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via PUT request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public void Delete()
        {
            byte[] buffer = Encoding.Default.GetBytes("Hello, my name is Health. And this is my controller. You called me via DELETE request of http protocol");
            Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

    }
}
