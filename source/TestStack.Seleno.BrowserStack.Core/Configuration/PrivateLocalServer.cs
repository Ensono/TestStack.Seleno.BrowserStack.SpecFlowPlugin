using System.Collections.Generic;
using System.Linq;
using BrowserStack;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class PrivateLocalServer : IWebServer
    {
        private readonly IConfigurationProvider _configuration;
        private readonly Local _locale = new Local();

        internal virtual bool IsRunning
        {
            get { return _locale.isRunning(); }
        }

        public PrivateLocalServer(IConfigurationProvider configuration)
        {
            _configuration = configuration;
        }
        public void Start()
        {
            if (IsRunning) return;

            StartServerWithOptions(
                new KeyValuePair<string, string>("key", _configuration.AccessKey),
                new KeyValuePair<string, string>("forcelocal", "true")
            );
        }

        public void Stop()
        {
            if (IsRunning)
            {
                StopServer();
            }
        }

        public string BaseUrl
        {
            get { return _configuration.RemoteUrl; }
        }

        internal virtual void StartServerWithOptions(params KeyValuePair<string, string>[] options)
        {
            _locale.start(options.ToList());
        }

        internal virtual void StopServer()
        {
            _locale.stop();
        }
    }
}