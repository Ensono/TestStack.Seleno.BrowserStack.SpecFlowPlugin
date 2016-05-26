using System;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.Configuration;
using TestStack.Seleno.Configuration.Contracts;
using TestStack.Seleno.BrowserStack.Core.Pages;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class BrowserHost : IBrowserHost
    {
        private SelenoHost _selenoHost;

        public BrowserHost(SelenoHost selenoHost, BrowserConfiguration browserConfiguration = null)
        {
            _selenoHost = selenoHost;
            Configuration = browserConfiguration ?? new BrowserConfiguration();
        }

        public void Run(Func<RemoteWebDriver> remoteWebDriverFactory, IWebServer webServer)
        {
            StartHost(remoteWebDriverFactory, webServer);
            _selenoHost.Application.Browser.Manage().Window.Maximize();
        }

        public virtual void StartHost(Func<RemoteWebDriver> remoteWebDriverFactory, IWebServer webServer)
        {
            _selenoHost.Run(config => config.WithRemoteWebDriver(remoteWebDriverFactory).WithWebServer(webServer));          
        }

        public TPage NavigateToInitialPage<TPage>(string url = "") where TPage : Page, new()
        {
            return _selenoHost.NavigateToInitialPage<TPage>(url);
        }

        public BrowserConfiguration Configuration { get; internal set; }

        public string SessionId
        {
            get
            {
                var sessionId = string.Empty;

                var application = _selenoHost.Application;
                if (application != null)
                {
                    var remoteWebDriver = application.Browser as IHasSessionId;

                    if (remoteWebDriver != null)
                    {
                        sessionId = remoteWebDriver.SessionId.ToString();
                    }
                }

                return sessionId;
            }
        }

        public void Dispose()
        {
            if (_selenoHost == null || _selenoHost.Application == null) return;

            _selenoHost.Dispose();
            _selenoHost = null;
        }
    }
}