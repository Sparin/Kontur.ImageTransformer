using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Controllers
{
    public abstract class Controller
    {
        public HttpListenerContext Context { get; private set; }
    }
}
