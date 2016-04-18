using OpenQA.Selenium;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration
{
    public interface IBrowserHostFactory
    {
        IBrowserHost CreateWithCapabilities(ICapabilities capabilities);
    }
}