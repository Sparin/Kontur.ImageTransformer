using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Tests.Fixtures
{
    public class ServerFixture : IDisposable
    {
        public HttpClient Client { get; set; }
        public Settings Settings { get; private set; }

        public ServerFixture()
        {
            var json = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\Settings.json");
            Settings = JsonConvert.DeserializeObject<Settings>(json);

            Client = new HttpClient();
            Client.BaseAddress = new Uri(Settings.BaseAddress);
            Client.Timeout = TimeSpan.FromSeconds(Settings.RequestTimeout);
        }

        public virtual void Dispose()
        {
            if (Client != null)
                Client.Dispose();
        }
    }
}
