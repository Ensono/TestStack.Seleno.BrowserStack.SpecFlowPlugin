using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.Configuration;
using TestStack.Seleno.Configuration.Contracts;
using TestStack.Seleno.Configuration.WebServers;
using TestStack.Seleno.BrowserStack.Core.Enums;
using TestStack.Seleno.BrowserStack.Core.Services.BrowserStack;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class BrowserHostFactory : IBrowserHostFactory
    {
        private readonly IConfigurationProvider _configurationProvider;

        public TimeSpan CommandTimeOut { get; set; }

        public BrowserHostFactory(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            CommandTimeOut = TimeSpan.FromMinutes(5);
        }
        
        public IBrowserHost CreateWithCapabilities(ICapabilities capabilities, BrowserConfiguration browserConfiguration = null)
        {
            var instance = CreateBrowserHost(browserConfiguration);

            instance.Run(CreateRemoteDriverWithCapabilities(capabilities), 
                         CreateInternetWebServer(_configurationProvider.RemoteUrl));

            return instance;
        }

     
        public virtual IBrowserHost CreateLocalWebDriver(BrowserEnum browser, BrowserConfiguration browserConfiguration = null)
        {
            var instance = CreateBrowserHost(browserConfiguration);

            instance.Configuration.IsLocalWebDriver = true;

            instance.Run(LocalWebBrowser(browser), CreateInternetWebServer(_configurationProvider.RemoteUrl));

            return instance;
        }

      
        #region internal Factories

        internal virtual Func<RemoteWebDriver> StartPrivateServerAndCreateRemoteDriverWithCapabilities(ICapabilities capabilities, IWebServer localServer)
        {
            return () =>
            {
                localServer.Start();
                return new RemoteWebDriver(new Uri(_configurationProvider.RemoteUrl), capabilities, CommandTimeOut);
            };
        }

        internal virtual IBrowserHost CreateBrowserHost(BrowserConfiguration browserConfiguration)
        {
            return new BrowserHost(new SelenoHost(), browserConfiguration);
        }


        internal virtual Func<RemoteWebDriver> CreateRemoteDriverWithCapabilities(ICapabilities capabilities)
        {
            return () => new RemoteWebDriver(new Uri(_configurationProvider.RemoteUrl), capabilities, CommandTimeOut);
        }


        internal virtual IWebServer CreateInternetWebServer(string remoteUrl)
        {
            return new InternetWebServer(remoteUrl);
        }

        internal virtual Func<RemoteWebDriver> LocalWebBrowser(BrowserEnum browser)
        {
            switch (browser)
            {
                case BrowserEnum.Firefox:
                    return BrowserFactory.FireFox;

                case BrowserEnum.InternetExplorer:
                    return BrowserFactory.InternetExplorer;
                case BrowserEnum.PhantomJs:
                    return BrowserFactory.PhantomJS;

                case BrowserEnum.Safari:
                    return BrowserFactory.Safari;


                case BrowserEnum.Chrome:
                default:
                    return BrowserFactory.Chrome;
            }

        }

        #endregion
    }
}