using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;
using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class PrivateLocalServer : ILifecycleTask
    {
        private readonly IBrowserStackLocalServer _localServer;
        private readonly IConfigurationProvider _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;
        public static readonly TimeSpan ServerStartTimeOut = new TimeSpan(0, 0, 0, 10);

        public PrivateLocalServer(IConfigurationProvider configuration) : this(new BrowserStackLocalServer(), configuration, new DateTimeProvider()) {  }

        public PrivateLocalServer(IBrowserStackLocalServer localServer, IConfigurationProvider configuration, IDateTimeProvider dateTimeProvider)
        {
            _localServer = localServer;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;
        }
       
     
        public void Start()
        {
            if (_localServer.IsRunning) return;

            _localServer.Start(
                new KeyValuePair<string, string>("key", _configuration.AccessKey),
                new KeyValuePair<string, string>("forcelocal", "true")
            );

            WaitUntilServerHasStarted();
        }

        public void Stop()
        {
            if (_localServer.IsRunning)
            {
                _localServer.Stop();
            }
        }

        internal virtual void WaitUntilServerHasStarted()
        {
            var waitUntil = _dateTimeProvider.Now + ServerStartTimeOut;
            while (!_localServer.IsRunning && _dateTimeProvider.Now < waitUntil) { }
        }
    }
}