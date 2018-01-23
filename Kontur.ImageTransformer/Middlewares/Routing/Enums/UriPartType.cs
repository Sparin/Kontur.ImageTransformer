using System;
using System.Collections.Generic;
using System.Text;

namespace Kontur.ImageTransformer.Middlewares.Routing.Enums
{
    /// <summary>
    /// Describes type of Uri type
    /// </summary>
    public enum UriPartType
    {
        /// <summary>
        /// Not a arguement. Static part of Uri
        /// </summary>
        Static,
        /// <summary>
        /// Arguement. Dynamic part of Uri
        /// </summary>
        Dynamic
    }
}
