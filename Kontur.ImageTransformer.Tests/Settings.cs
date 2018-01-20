using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Tests
{
    public class Settings
    {
        /// <summary>
        /// Gets or sets base address of mcc_server.exe
        /// </summary>
        public string BaseAddress { get; set; } = "http://localhost:8080";

        /// <summary>
        /// Gets or sets timeout on request in seconds
        /// </summary>
        public int RequestTimeout { get; set; } = 30;
    }
}
