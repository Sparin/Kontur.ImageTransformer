using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Controllers
{
    /// <summary>
    /// A base class for providing HTTP methods 
    /// </summary>
    public abstract class Controller
    {
        /// <summary>
        /// Gets the listener's HTTP context for this instance
        /// </summary>
        public HttpListenerContext Context { get; /*private set;*/ }
    }
}
