using OpenQA.Selenium;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public interface IBrowserHostFactory
    {
        IBrowserHost CreateWithCapabilities(ICapabilities capabilities, BrowserConfiguration browserConfiguration = null);
    }
}