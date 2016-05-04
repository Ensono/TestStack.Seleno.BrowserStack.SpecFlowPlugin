using System;
using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.Configuration;
using TestStack.Seleno.Configuration.Contracts;
using TestStack.Seleno.Configuration.WebServers;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class BrowserHostFactory : IBrowserHostFactory
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IObjectContainer _container;

        public TimeSpan CommandTimeOut { get; set; }

        public BrowserHostFactory(IConfigurationProvider configurationProvider, IObjectContainer container)
        {
            _configurationProvider = configurationProvider;
            _container = container;
            CommandTimeOut = TimeSpan.FromMinutes(5);
        }


        public IBrowserHost CreateWithCapabilities(ICapabilities capabilities)
        {
            var instance = CreateBrowserHost();

            instance.Run(CreateRemoteDriverWithCapabilities(capabilities), 
                         CreateWebServer(_configurationProvider.RemoteUrl));

            return instance;
        }

        public virtual IBrowserHost CreateBrowserHost()
        {
            return new BrowserHost(new SelenoHost(), _container);
        }

        public virtual Func<RemoteWebDriver> CreateRemoteDriverWithCapabilities(ICapabilities capabilities)
        {
            return () => new RemoteWebDriver(new Uri(_configurationProvider.RemoteUrl), capabilities, CommandTimeOut);
        }

        public virtual IWebServer CreateWebServer(string remoteUrl)
        {
            return new InternetWebServer(remoteUrl);
        }
    }
}