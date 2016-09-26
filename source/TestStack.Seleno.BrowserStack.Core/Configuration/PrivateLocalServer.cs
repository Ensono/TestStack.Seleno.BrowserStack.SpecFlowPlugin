using System.Collections.Generic;
using System.Linq;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class PrivateLocalServer : IWebServer
    {
        private readonly IBrowserStackLocalServer _localServer;
        private readonly IConfigurationProvider _configuration;

        public PrivateLocalServer(IBrowserStackLocalServer localServer, IConfigurationProvider configuration)
        {
            _localServer = localServer;
            _configuration = configuration;
        }
       
     
        public void Start()
        {
            if (_localServer.IsRunning) return;

            _localServer.Start(
                new KeyValuePair<string, string>("key", _configuration.AccessKey),
                new KeyValuePair<string, string>("forcelocal", "true")
            );
        }

        public void Stop()
        {
            if (_localServer.IsRunning)
            {
                _localServer.Stop();
            }
        }

        public string BaseUrl
        {
            get { return _configuration.RemoteUrl; }
        }
    }
}