using System;
using OpenQA.Selenium.Remote;
using TestStack.Seleno.Configuration.Contracts;
using Page = TestStack.Seleno.BrowserStack.Core.Pages.Page;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public interface IBrowserHost : IDisposable
    {
        string SessionId { get; }

        void Run(Func<RemoteWebDriver> remoteWebDriverFactory, IWebServer webServer);

        TPage NavigateToInitialPage<TPage>(string url = "") where TPage : Page, new();

        BrowserConfiguration Configuration { get; }

    }
}