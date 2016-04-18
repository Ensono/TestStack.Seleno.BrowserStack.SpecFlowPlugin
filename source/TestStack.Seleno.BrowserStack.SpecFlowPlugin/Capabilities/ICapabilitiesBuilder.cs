using OpenQA.Selenium;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin.Configuration;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Capabilities
{
    public interface ICapabilitiesBuilder
   {
       ICapabilitiesBuilder WithTestSpecification(TestSpecification testSpecification);

       ICapabilitiesBuilder WithCredentials(string userName, string accessKey);

       ICapabilitiesBuilder WithBrowserConfiguration(BrowserConfiguration browserConfiguration);

        ICapabilitiesBuilder WithBuildNumber(string buildNumber);

       ICapabilities Build();
   }
}