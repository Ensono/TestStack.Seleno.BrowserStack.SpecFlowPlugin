using System;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class InvalidDesktopResolution : Exception
    {
        public string Resolution { get; }

        public InvalidDesktopResolution(string resolution)
            : base(
                $@"{resolution} is not a valid resolution. A valid one looks like {BrowserConfiguration
                    .DefaultDesktopResolution}")
        {
            Resolution = resolution;
        }
    }
}