using System.Collections.Generic;
using System.Linq;
using BrowserStack;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class PrivateLocaleServer : IWebServer
    {
        private readonly IConfigurationProvider _configuration;
        private readonly Local _locale = new Local();

        public PrivateLocaleServer(IConfigurationProvider configuration)
        {
            _configuration = configuration;
        }
        public void Start()
        {
            StartServerWithOptions(
                new KeyValuePair<string, string>("key", _configuration.AccessKey),
                new KeyValuePair<string, string>("forcelocal", "true") /*,
                new KeyValuePair<string, string>("binarypath", @"C:\Users\Franck\.browserstack\BrowserStackLocal.exe"),
                new KeyValuePair<string, string>("logfile", @"C:\Users\Franck\.browserstack\local.log")*/
            );
        }

        public void Stop()
        {
            StopServer();
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