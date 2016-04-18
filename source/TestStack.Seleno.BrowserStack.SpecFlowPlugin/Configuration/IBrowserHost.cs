using System;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.Configuration.Contracts;
using TestStack.Seleno.PageObjects;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration
{
    public interface IBrowserHost : IDisposable
    {
        string SessionId { get; }

        void Run(Func<RemoteWebDriver> remoteWebDriverFactory, IWebServer webServer);

        TPage NavigateToInitialPage<TPage>(string url = "") where TPage : UiComponent, new();

    }
}