using System;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}