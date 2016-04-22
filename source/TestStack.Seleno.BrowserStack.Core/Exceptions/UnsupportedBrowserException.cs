using System;
using TestStack.Seleno.BrowserStack.Core.Configuration;

namespace TestStack.Seleno.BrowserStack.Core.Exceptions
{
    public class UnsupportedBrowserException : Exception
    {
        public BrowserConfiguration Browser { get; private set; }

        public UnsupportedBrowserException(BrowserConfiguration browser) : base(string.Format("{0} is not supported",browser))
        {
            Browser = browser;
        }
    }
}