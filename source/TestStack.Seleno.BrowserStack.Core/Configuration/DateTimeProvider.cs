using System;

namespace TestStack.Seleno.BrowserStack.Core.Configuration
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}