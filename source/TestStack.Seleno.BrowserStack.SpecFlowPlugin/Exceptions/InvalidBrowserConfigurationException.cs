using System;

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin.Exceptions
{
    public class InvalidBrowserConfigurationException : Exception
    {
        public InvalidBrowserConfigurationException(string message) : base(message)
        {
        }
    }
}