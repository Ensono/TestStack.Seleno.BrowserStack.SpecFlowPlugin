using System;
using BoDi;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.BrowserStack.Core.Extensions;
using TestStack.Seleno.Configuration;
using TestStack.Seleno.Configuration.Contracts;
using TestStack.Seleno.PageObjects;
using TestStack.Seleno.PageObjects.Actions;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class BrowserHost : IBrowserHost
    {
        private SelenoHost _selenoHost;
        private readonly IObjectContainer _container;

        public BrowserHost(SelenoHost selenoHost, IObjectContainer container)
        {
            _selenoHost = selenoHost;
            _container = container;
        }

        public void Run(Action<IAppConfigurator> configure)
        {
            _selenoHost.Run(configure);
        }

        public void Run(Func<RemoteWebDriver> remoteWebDriverFactory, IWebServer webServer)
        {
            _selenoHost.Run(config => config.WithRemoteWebDriver(remoteWebDriverFactory).WithWebServer(webServer));
        }

        public TPage NavigateToInitialPage<TPage>(string url = "") where TPage : Page, new()
        {
            var page = NavigateTo<TPage>(url);
            _container.RegisterInstance(page);
            page.Container = _container;
            return page;
        }

        public virtual TPage NavigateTo<TPage>(string url) where TPage : Page, new()
        {
            return _selenoHost.NavigateToInitialPage<TPage>(url);
        }

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