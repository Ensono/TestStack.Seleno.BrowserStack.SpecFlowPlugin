using OpenQA.Selenium;
using TestStack.Seleno.BrowserStack.Core.Enums;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public interface IBrowserHostFactory
    {
        IBrowserHost CreateWithCapabilities(ICapabilities capabilities, BrowserConfiguration browserConfiguration = null);
        IBrowserHost CreateLocalWebDriver(BrowserEnum browser, BrowserConfiguration browserConfiguration = null);
        IBrowserHost CreatePrivateLocalServer(ICapabilities capabilities, BrowserConfiguration browserConfiguration = null);
    }
}