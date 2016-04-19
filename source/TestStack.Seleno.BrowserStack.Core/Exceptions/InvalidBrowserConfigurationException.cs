using System;

namespace TestStack.Seleno.BrowserStack.Core.Exceptions
{
    public class InvalidBrowserConfigurationException : Exception
    {
        public InvalidBrowserConfigurationException(string message) : base(message)
        {
        }
    }
}